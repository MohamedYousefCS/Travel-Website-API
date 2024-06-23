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


        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
        {
            // Save message to database
            var newMessage = new Message
            {
                User = message.User,
                Content = message.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Send message to all clients
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Content);
            return Ok();
        }


        [HttpPost("NotifyUser")]
        public async Task<IActionResult> NotifyUser([FromBody] NotificationDto notification)
        {
            await _hubContext.Clients.User(notification.User).SendAsync("ReceiveNotification", notification.Content);
            return Ok();
        }


        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages()
        {
            // Retrieve messages from the database
            var messages = await _context.Messages
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto
                {
                    User = m.User,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                })
                .ToListAsync();

            return Ok(messages);
        }

    }

   
    

}
