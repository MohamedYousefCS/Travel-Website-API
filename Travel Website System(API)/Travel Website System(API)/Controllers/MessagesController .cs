using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Hubs;
using Travel_Website_System_API_.Models;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ApplicationDBContext _context;
        private readonly string _connectionString;

        public MessagesController(IHubContext<ChatHub> hubContext, ApplicationDBContext context , IConfiguration configuration)
        {
            _hubContext = hubContext;
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }




        [HttpPost("SendMessageToClient")]
        public async Task<IActionResult> SendMessageToClient([FromBody] ClientMessageDto clientMessageDto)
        {
            var user = await _context.Clients.FirstOrDefaultAsync(u => u.Id == clientMessageDto.clientId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToClient", user, clientMessageDto.Content, clientMessageDto.ConnectionId);
            return Ok();
        }


        [HttpPost("SendMessageToCustomerService")]
        public async Task<IActionResult> SendMessageToCustomerService([FromBody] ClientMessageDto clientMessageDto)
        {
            // Find customer service user (you may have a specific role or identifier for customer service)
            var customerService = await _context.Users.FirstOrDefaultAsync(u => u.Role == "customerService");

            if (customerService == null)
            {
                return BadRequest("Customer service not found");
            }

            // Send message to customer service using SignalR
            await _hubContext.Clients.User(customerService.Id.ToString()).SendAsync("ReceiveMessageFromClient", clientMessageDto.Content, clientMessageDto.clientId);

            return Ok();
        }




        [HttpPost("SendMessageToAll")]
        public async Task<IActionResult> SendMessageToAll([FromBody] List<ClientMessageDto> clientMessageDtos)
        {
            foreach (var clientMessageDto in clientMessageDtos)
            {
                var user = await _context.Clients.FirstOrDefaultAsync(u => u.Id == clientMessageDto.clientId);
                if (user == null)
                {
                    return BadRequest($"User not found for clientId: {clientMessageDto.clientId}");
                }

                await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToAll", user, clientMessageDto.Content);
            }

            return Ok();

}


        [HttpPost("SendMessageToGroup")]
        public async Task<IActionResult> SendMessageToGroup([FromBody] GroupMessageDto groupMessageDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == groupMessageDto.User.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToGroup", user, groupMessageDto.Content, groupMessageDto.GroupName);
            return Ok();
        }




        [HttpPost("JoinGroup")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupJoinDto groupJoinDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == groupJoinDto.UserName);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.User(user.Fname.ToString()).SendAsync("JoinGroup", groupJoinDto.GroupName);
            return Ok();
        }





        [HttpPost("NotifyUser")]
        public async Task<IActionResult> NotifyUser([FromBody] NotificationDto notification)
        {

            List<ApplicationUser> users = new List<ApplicationUser>();
            foreach(var userName in notification.UserNames) 
            {

                var userr  =  _context.Users.Where(user => user.UserName == userName).FirstOrDefault();
                users.Add(userr);
            }
           

            foreach(var user in users)
            {
                await _hubContext.Clients.User(user.Id.ToString()).SendAsync("ReceiveNotification", notification.Content);

            }

            return Ok();
        }



        [HttpPost("Connect")]
        public async Task<IActionResult> Connect(string userId, string connectionId)
        {

            var clientConnection = new ClientConnection
            {
                ClientId = userId,
                ConnectionId = connectionId,
                IsConnected = true,
                LastUpdated = DateTime.UtcNow
            };

            _context.ClientConnections.Add(clientConnection);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group("ChatHub").SendAsync("OnUserConnected", userId, connectionId);

            return Ok();
        }

        [HttpPost("Disconnect")]
        public async Task<IActionResult> Disconnect(string connectionId)
        {
            var clientConnection = await _context.ClientConnections
                .FirstOrDefaultAsync(cc => cc.ConnectionId == connectionId);

            if (clientConnection != null)
            {
                clientConnection.IsConnected = false;
                clientConnection.LastUpdated = DateTime.UtcNow;

                _context.ClientConnections.Update(clientConnection);
                await _context.SaveChangesAsync();
            }
            await _hubContext.Clients.Group("ChatHub").SendAsync("OnUserDisconnected", clientConnection.ClientId, connectionId);

            return Ok();
        }



        [HttpGet("online")]
        public async Task<IActionResult> GetOnlineClients()
        {
            var clients = await _context.ClientConnections
                                        .Where(c => c.IsConnected)
                                        .ToListAsync();

            return Ok(clients);
        }



        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UserStatusDto statusDto)
        {
            var user = await _context.Users.FindAsync(statusDto.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.Status = statusDto.Status;
            user.LastSeen = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("UserStatusUpdated", user.Id, user.Status);
            return Ok();
        }





        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromBody] int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return BadRequest("Message not found");
            }

            message.IsRead = true;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(message.Sender.ToString()).SendAsync("MessageRead", messageId);
            return Ok();
        }



        [HttpDelete("DeleteMessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            // Notify clients via SignalR to delete the message
            await _hubContext.Clients.All.SendAsync("DeleteMessage", messageId);

            // Delete the message from the database
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok();
        }



        [HttpPost("NotifyTyping")]
        public async Task<IActionResult> NotifyTyping([FromBody] int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.All.SendAsync("UserTyping", userId);
            return Ok();
        }




        
    }
}
