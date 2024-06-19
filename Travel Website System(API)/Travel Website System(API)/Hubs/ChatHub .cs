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

            _ = Clients.All.SendAsync("ReceiveMessage", user, message);

            // Send the message to all clients except the sender
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }


        public override async Task OnConnectedAsync()
        {

            // Get the username or identifier of the connected user

            string user = Context.User.Identity.Name; 

            // Notify all clients about the new connection

            await Clients.All.SendAsync("UserConnected", user);

            // Add the client to a group named after the user (optional)

            await Groups.AddToGroupAsync(Context.ConnectionId, user);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Get the username or identifier of the disconnected user

            string user = Context.User.Identity.Name; 

            // Notify all clients about the disconnection

            await Clients.All.SendAsync("UserDisconnected", user);

            // Remove the client from all groups they were part of

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user);

            await base.OnDisconnectedAsync(exception);
        }


        //Notifying Users with new Receive message Notification
        public async Task NotifyUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", message);
        }
    }
}
