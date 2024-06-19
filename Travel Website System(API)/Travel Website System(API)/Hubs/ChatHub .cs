namespace Travel_Website_System_API_.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class ChatHub : Hub
    {
        // Method to broadcast messages to all connected clients
        public async Task SendMessage(string user, string message)
        {
            // send the message to all clients including the sender.

            Clients.All.SendAsync("ReceiveMessage", user, message);

            // Send the message to all clients except the sender
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            // Example: Notify all clients about the new connection
            string user = Context.User.Identity.Name; // Get the username or identifier of the connected user
            await Clients.All.SendAsync("UserConnected", user);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Example: Notify all clients about the disconnection
            string user = Context.User.Identity.Name; // Get the username or identifier of the disconnected user
            await Clients.All.SendAsync("UserDisconnected", user);

            await base.OnDisconnectedAsync(exception);
        }
        public async Task NotifyUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", message);
        }
    }
}
