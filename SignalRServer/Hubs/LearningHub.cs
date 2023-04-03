﻿using BlazorClient.Pages;
using Microsoft.AspNetCore.SignalR;


namespace SignalRServer.Hubs {
    public class LearningHub : Hub<ILearningHubClient> {
        public async Task BroadcastMessage(string message) {
            await Clients.All.ReceiveMessage(message);
        }

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception) {
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendToOthers(string message) {
            await Clients.Others.ReceiveMessage(message);
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