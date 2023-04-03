using BlazorClient.Pages;
using Microsoft.AspNetCore.SignalR;


namespace SignalRServer.Hubs {
    public class LearningHub : Hub<ILearningHubClient> {
        

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception) {
            await base.OnDisconnectedAsync(exception);
        }
        public async Task BroadcastMessage(string message) {
            await Clients.All.ReceiveMessage(GetMessageToSend(message));
        }

        public async Task SendToOthers(string message) {
            await Clients.Others.ReceiveMessage(GetMessageToSend(message));
        }

        public async Task SendToCaller(string message) {
            await Clients.Caller.ReceiveMessage(GetMessageToSend(message));
        }


        private string GetMessageToSend(string originalMessage) {
            return $"User connection id: {Context.ConnectionId}. Message: {originalMessage}";
        }
    }
}

//namespace SignalRServer.Hubs {
    //public class LearningHub : Hub {
    //    public async Task BroadcastMessage(string message) {
    //        await Clients.All.SendAsync("ReceiveMessage", message);
    //    }

    //    public override async Task OnConnectedAsync() {
    //        await base.OnConnectedAsync();
    //    }

    //    public override async Task OnDisconnectedAsync(Exception? exception) {
    //        await base.OnDisconnectedAsync(exception);
    //    }
    //}
//}