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
        private readonly string _connectionString;

        public ChatHub(ApplicationDBContext context )
        {
            _context = context;
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

        public async Task JoinGroup(string group, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            var user = await _context.Users.FindAsync(userId);
            await Clients.Group(group).SendAsync("UserJoined", user.UserName);
        }



        public override async Task OnConnectedAsync()
        {
            var UserId = Context.UserIdentifier; // Assuming you use authentication and UserIdentifier is set
            var connectionId = Context.ConnectionId;

            var clientConnection = new UserConnection
            {
                ApplicationUserId = UserId,
                ConnectionId = connectionId,
                IsConnected = true,
                LastUpdated = DateTime.UtcNow
            };

           await _context.UserConnections.AddAsync(clientConnection);
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            var clientConnection = await _context.UserConnections
                .FirstOrDefaultAsync(cc => cc.ConnectionId == connectionId);

            if (clientConnection != null)
            {
                clientConnection.IsConnected = false;
                clientConnection.LastUpdated = DateTime.UtcNow;

                 _context.UserConnections.Update(clientConnection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }



        //Notifying Users with new Receive message Notification
        public async Task NotifyUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", message);
        }


       


      
    }
}
