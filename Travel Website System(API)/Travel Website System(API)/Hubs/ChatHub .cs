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





        }
}
