using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

        public MessagesController(IHubContext<ChatHub> hubContext, ApplicationDBContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }




        [HttpPost("SendMessageToClient")]
        public async Task<IActionResult> SendMessageToClient([FromBody] ClientMessageDto clientMessageDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == clientMessageDto.User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToClient", user, clientMessageDto.Content, clientMessageDto.ConnectionId);
            return Ok();
        }




        [HttpPost("SendMessageToAll")]
        public async Task<IActionResult> SendMessageToAll([FromBody] MessageDto messageDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == messageDto.User.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToAll", user, messageDto.Content);
            return Ok();
        }




        [HttpPost("SendMessageToOthers")]
        public async Task<IActionResult> SendMessageToOthers([FromBody] MessageDto messageDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == messageDto.User.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToOthers", user, messageDto.Content);
            return Ok();
        }




        [HttpPost("SendMessageToCaller")]
        public async Task<IActionResult> SendMessageToCaller([FromBody] MessageDto messageDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == messageDto.User.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SendMessageToCaller", user, messageDto.Content);
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

     

     

        [HttpPost("SaveMessageToDatabase")]
        public async Task<IActionResult> SaveMessageToDatabase([FromBody] MessageDto MessageDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == MessageDto.User.ToString());
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserName == MessageDto.User.ToString());
            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.UserName == MessageDto.Receiver.ToString());
            if (user == null || sender == null || receiver == null)
            {
                return BadRequest("User, Sender, or Receiver not found");
            }

            await _hubContext.Clients.Group("ChatHub").SendAsync("SaveMessageToDatabase", user, MessageDto.Content, sender, receiver);
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.User == notification.User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("ReceiveNotification", notification.Content);
            return Ok();
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




        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages([FromQuery] string user1, [FromQuery] string user2)
        {
            var messages = await _context.Messages
                .Where(m => (m.Sender.UserName == user1 && m.Receiver.UserName == user2) ||
                            (m.Sender.UserName == user2 && m.Receiver.UserName == user1))
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto
                {
                    User = m.User,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    ProfilePictureUrl = (string)m.Sender.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}
