// See https://aka.ms/new-console-template for more information
using MessagePack;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

Console.WriteLine("Hello, World!");
Console.WriteLine("Please specify the URL of SignalR Hub");
var url = "https://localhost:7169/learningHub";// Console.ReadLine();
Console.WriteLine("Please specify the access token");

//var token = Console.ReadLine();
var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6InlHb3BkZ09DMExtZDhwemo3TXhBMnciLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE2ODA1OTMxNTAsImV4cCI6MTY4MDU5Njc1MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImNsaWVudF9pZCI6IndlYkFwcENsaWVudCIsInN1YiI6IjdkOTBiYTZmLTIwYTUtNDgzNi1iM2I4LTk1NmJiZDQzNmFhOSIsImF1dGhfdGltZSI6MTY4MDU5MzE0OSwiaWRwIjoibG9jYWwiLCJlbWFpbCI6InVzZXIxQG1haWwuY29tIiwibmFtZSI6InVzZXIxIiwicm9sZSI6WyJhZG1pbiIsInVzZXIiXSwiYWRtaW4iOiJ0cnVlIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSJdLCJhbXIiOlsicHdkIl19.aphMq7jzvj_EZlS5i7oxxyLh3jue0Q8DH_oS2rdqXP_mCdfYmvMtkjrOHLAKVmE_cKWZJuvysuJSCA5pIih8XnHnRR99UrF8qkxfsB9tXxwxWa1PNB9uFIqkvUl4hXoM1muAQPObqIoYuRUd6jERmOTsg0IMoatbMTxk2z9WQfRpUu5c418WtbCAYFOV_UtsFyWnt1bg6AtHewB3aJDO9heM4jo_YnkayVoQhjTlSbtbKGMQbMrwlR-FsF7yU96B8wPDsZwtlgJScuftHrI5vk7TABU7sNZt1RQxfQLJ-Tz01EoBSFok3fzxJQo5elWjfqH701SL5yWT1szAD8My2g";
var hubConnection = new HubConnectionBuilder()
                         .WithUrl(url,
                            HttpTransportType.WebSockets,
                            options => {
                                options.AccessTokenProvider = () => Task.FromResult(token);
                                options.HttpMessageHandlerFactory = null;
                                options.Headers["CustomData"] = "value";
                                options.SkipNegotiation = true;
                                options.ApplicationMaxBufferSize = 1_000_000;
                                options.ClientCertificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
                                options.CloseTimeout = TimeSpan.FromSeconds(5);
                                options.Cookies = new System.Net.CookieContainer();
                                options.DefaultTransferFormat = TransferFormat.Text;
                                options.Credentials = null;
                                options.Proxy = null;
                                options.UseDefaultCredentials = true;
                                options.TransportMaxBufferSize = 1_000_000;
                                options.WebSocketConfiguration = null;
                                options.WebSocketFactory = null;
                            })
                         .ConfigureLogging(logging => {
                             logging.SetMinimumLevel(LogLevel.Information);
                             //logging.AddConsole();
                             
                         })
                         .AddMessagePackProtocol(options => {
                             options.SerializerOptions = MessagePackSerializerOptions.Standard
                                .WithSecurity(MessagePackSecurity.UntrustedData)
                                .WithCompression(MessagePackCompression.Lz4Block)
                                .WithAllowAssemblyVersionMismatch(true)
                                .WithOldSpec()
                                .WithOmitAssemblyVersion(true);
                         })
                         .Build();

hubConnection.HandshakeTimeout = TimeSpan.FromSeconds(15);
hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(10);

hubConnection.On<string>("ReceiveMessage",
    message => Console.WriteLine($"SignalR Hub Message: {message}"));

try {
    await hubConnection.StartAsync();

    var running = true;

    while (running) {
        var message = string.Empty;
        var groupName = string.Empty;

        Console.WriteLine("Please specify the action:");
        Console.WriteLine("0 - broadcast to all");
        Console.WriteLine("1 - send to others");
        Console.WriteLine("2 - send to self");
        Console.WriteLine("3 - send to individual");
        Console.WriteLine("4 - send to a group");
        Console.WriteLine("5 - add user to a group");
        Console.WriteLine("6 - remove user from a group");
        Console.WriteLine("7 - trigger a server stream");
        Console.WriteLine("exit - Exit the program");

        var action = Console.ReadLine();

        if (action != "5" && action != "6") {
            Console.WriteLine("Please specify the message:");
            message = Console.ReadLine();
        }

        if (action == "4" || action == "5" || action == "6") {
            Console.WriteLine("Please specify the group name:");
            groupName = Console.ReadLine();
        }

        switch (action) {
            case "0":
                if (message?.Contains(';') ?? false) {
                    var channel = Channel.CreateBounded<string>(10);
                    await hubConnection.SendAsync("BroadcastStream", channel.Reader);

                    foreach (var item in message.Split(';')) {
                        await channel.Writer.WriteAsync(item);
                    }

                    channel.Writer.Complete();
                } else {
                    hubConnection.SendAsync("BroadcastMessage", message).Wait();
                }
                break;
            case "1":
                await hubConnection.SendAsync("SendToOthers", message);
                break;
            case "2":
                await hubConnection.SendAsync("SendToCaller", message);
                break;
            case "3":
                Console.WriteLine("Please specify the connection id:");
                var connectionId = Console.ReadLine();
                await hubConnection.SendAsync("SendToIndividual", connectionId, message);
                break;
            case "4":
                hubConnection.SendAsync("SendToGroup", groupName, message).Wait();
                break;
            case "5":
                hubConnection.SendAsync("AddUserToGroup", groupName).Wait();
                break;
            case "6":
                hubConnection.SendAsync("RemoveUserFromGroup", groupName).Wait();
                break;
            case "7":
                Console.WriteLine("Please specify the number of jobs to execute.");
                var numberOfJobs = int.Parse(Console.ReadLine() ?? "0");
                var cancellationTokenSource = new CancellationTokenSource();
                var stream = hubConnection.StreamAsync<string>(
                "TriggerStream", numberOfJobs, cancellationTokenSource.Token);

                await foreach (var reply in stream) {
                    Console.WriteLine(reply);
                }
                break;
            case "exit":
                running = false;
                break;
            default:
                Console.WriteLine("Invalid action specified");
                break;
        }
    }
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}