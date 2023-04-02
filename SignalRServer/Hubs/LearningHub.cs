﻿using Microsoft.AspNetCore.SignalR;


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