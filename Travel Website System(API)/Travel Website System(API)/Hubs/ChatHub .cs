namespace Travel_Website_System_API_.Hubs
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Travel_Website_System_API.Models;
    using Travel_Website_System_API_.DTO;
 
    public class ChatHub : Hub
    {
        private readonly ApplicationDBContext _context;

        public ChatHub(ApplicationDBContext context)
        {
            _context = context;
        }


        public async Task SendMessageToClient(ApplicationUser user, string message, string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
        }


        // Method to broadcast messages to all connected clients
        public async Task SendMessageToAll(ApplicationUser user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        public async Task SendMessageToOthers(ApplicationUser user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }


        public async Task SendMessageToCaller(ApplicationUser user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }


        public async Task SendMessageToGroup(ApplicationUser user, string message, string groupName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }




        public async Task SaveMessageToDatabase(ApplicationUser user, string message, ApplicationUser sender, ApplicationUser receiver)
        {
            var newMessage = new Message
            {
                User = user,
                Content = message,
                Timestamp = DateTime.UtcNow,
                IsRead = false,
                Sender = sender,
                Receiver = receiver
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
        }



        public async Task JoinGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, groupName);

            // Notify the group that a new user has joined
            var user = Context.User.Identity.Name; // You can replace this with a more appropriate user identifier
            await Clients.Group(groupName).SendAsync("UserJoined", user, groupName);

            // Optionally notify the user that they have joined the group
            await Clients.Caller.SendAsync("JoinedGroup", groupName);
        }




        public async Task UpdateUserStatus(int userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Status = status;
                user.LastSeen = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await Clients.All.SendAsync("UserStatusUpdated", userId, status);
            }
        }


        public async Task MarkMessageAsRead(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                await Clients.User(message.Sender.ToString()).SendAsync("MessageRead", messageId);
            }
        }


        public async Task DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("MessageDeleted", messageId);
            }
        }


        public async Task NotifyTyping(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await Clients.All.SendAsync("UserTyping", userId);
            }
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



        public async Task<List<MessageDto>> GetMessages(string user1, string user2)
        {
            var messages = await _context.Messages
                .Where(m => (m.Sender.UserName == user1 && m.Receiver.UserName == user2) ||
                            (m.Sender.UserName == user2 && m.Receiver.UserName == user1))
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto
                {
                    User = m.User,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                })
                .ToListAsync();

            return messages;
        }
    }
}
