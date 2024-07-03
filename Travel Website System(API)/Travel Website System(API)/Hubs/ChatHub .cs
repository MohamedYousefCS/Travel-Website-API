namespace Travel_Website_System_API_.Hubs
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Travel_Website_System_API.Models;
    using Travel_Website_System_API_.DTO;
 
    public class ChatHub : Hub
    {
        private readonly ApplicationDBContext _context;
        private readonly string _connectionString;

        public ChatHub(ApplicationDBContext context )
        {
            _context = context;
;
        }

        

        public async Task SendMessageToClient(ApplicationUser user, string message, string connectionId)
        {
            // Store the message in the database
            var newMessage = new Message
            {
                UserId = user.Id,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Send the message to the specified client
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
        }



        public async Task SendMessageToCustomerService(string message, string userId)
        {
            var user = await _context.CustomerServices
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (user != null)
            {
                // Store the message in the database
                var newMessage = new Message
                {
                    UserId = userId,
                    Content = message,
                    Timestamp = DateTime.UtcNow
                };
                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                // Send the message to the customer service
                await Clients.User(user.Id).SendAsync("ReceiveMessageFromClient", message);
            }
            else
            {
                // Handle case where customer service is not connected
                await Clients.Caller.SendAsync("NoCustomerServiceAvailable");
            }
        }


        public async Task SendMessageToAll(ApplicationUser user, string message)
        {
            // Store the message in the database
            var newMessage = new Message
            {
                UserId = user.Id,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string group, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            var user = await _context.Users.FindAsync(userId);
            await Clients.Group(group).SendAsync("UserJoined", user.UserName);
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


       


        public override async Task OnConnectedAsync()
        {
            var clientId = Context.UserIdentifier; // Assuming you use authentication and UserIdentifier is set
            var connectionId = Context.ConnectionId;

            var clientConnection = new ClientConnection
            {
                ClientId = clientId,
                ConnectionId = connectionId,
                IsConnected = true,
                LastUpdated = DateTime.UtcNow
            };

            _context.ClientConnections.Add(clientConnection);
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            var clientConnection = await _context.ClientConnections
                .FirstOrDefaultAsync(cc => cc.ConnectionId == connectionId);

            if (clientConnection != null)
            {
                clientConnection.IsConnected = false;
                clientConnection.LastUpdated = DateTime.UtcNow;

                _context.ClientConnections.Update(clientConnection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }



        //Notifying Users with new Receive message Notification
        public async Task NotifyUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", message);
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


        public async Task NotifyTyping(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await Clients.All.SendAsync("UserTyping", userId);
            }
        }
    }
}
