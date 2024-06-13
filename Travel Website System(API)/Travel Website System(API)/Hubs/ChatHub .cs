namespace Travel_Website_System_API_.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class ChatHub : Hub
    {
        // Method to broadcast messages to all connected clients
        public async Task SendMessage(string user, string message)
        {
            // Clients.All.SendAsync("ReceiveMessage", user, message);
            // Uncomment the above line if you want to send the message to all clients including the sender.

            // Send the message to all clients except the sender
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            // Implement your logic when a client connects
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Implement your logic when a client disconnects
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NotifyUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", message);
        }
    }
}
