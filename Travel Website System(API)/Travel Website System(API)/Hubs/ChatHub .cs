namespace Travel_Website_System_API_.Hubs
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using Travel_Website_System_API.Models;
    using Travel_Website_System_API_.DTO;
 
    public class ChatHub : Hub
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ApplicationDBContext context , ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
;
        }


        public async Task SendMessageToAll(ApplicationUser user, string message)
        {
            // Store the message in the database
            var newMessage = new Message
            {
                ReceiverId = user.Id,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            // Broadcast the message to all connected clients
            //await Clients.All.SendAsync("ReceiveMessage", user, message);

            var users = new List<string>() { user.Id };
            var userConnections = _context.UserConnections.AsNoTracking().Where(x => users.Contains(x.ApplicationUserId)).Select(x => x.ConnectionId.ToString());

            await Clients.Clients(userConnections.ToArray<string>()).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(newMessage));

        }


        public async Task JoinGroup(UserConnection conn)
        {
            
            await Clients.All.SendAsync("ReceiveMessage", "CustomerService", $"{conn.ApplicationUser}has joined the group");
        }



        public override async Task OnConnectedAsync()
        {
            var UserId = Context.UserIdentifier; // Assuming you use authentication and UserIdentifier is set
            var connectionId = Context.ConnectionId;

            var UserConnection = new UserConnection
            {
                ApplicationUserId = UserId,
                ConnectionId = connectionId,
                IsConnected = true,
                LastUpdated = DateTime.UtcNow
            };

           await _context.UserConnections.AddAsync(UserConnection);
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            var UserConnection = await _context.UserConnections
                .FirstOrDefaultAsync(cc => cc.ConnectionId == connectionId);

            if (UserConnection != null)
            {
                UserConnection.IsConnected = false;
                UserConnection.LastUpdated = DateTime.UtcNow;

                 _context.UserConnections.Update(UserConnection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }



        //Notifying Users with new Receive message Notification
        public async Task NotifyUserAsync(string user, string message)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                _logger.LogWarning("NotifyUserAsync: User is null or empty.");
                throw new ArgumentException("User cannot be null or empty.", nameof(user));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("NotifyUserAsync: Message is null or empty.");
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }

            try
            {
                _logger.LogInformation($"Notifying user '{user}' with message: {message}");
                await Clients.User(user).SendAsync("ReceiveNotification", message);
                _logger.LogInformation("Notification sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while notifying the user.");
                throw; // Re-throw the exception to ensure it can be handled further up the call stack if needed.
            }
        }




        }
}
