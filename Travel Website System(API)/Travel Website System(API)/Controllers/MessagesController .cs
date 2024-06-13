using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Hubs;
namespace Travel_Website_System_API_.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Content);
            return Ok();
        }

        [HttpPost("NotifyUser")]
        public async Task<IActionResult> NotifyUser([FromBody] NotificationDto notification)
        {
            await _hubContext.Clients.User(notification.User).SendAsync("ReceiveNotification", notification.Content);
            return Ok();
        }


    }

   
    

}
