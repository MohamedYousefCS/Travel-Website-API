using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Hubs;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        private IHubContext<ChatHub> _hub;

        public ChatsController(ApplicationDBContext context, IHubContext<ChatHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpGet("GetChats")]
        public ActionResult GetChats()
        {
            List<Chat> chats = _context.Chats.Where(c => c.IsDeleted == false)
                .Include(c => c.Messages)
                .ToList();

            List<ChatDTO> chatDTOs = new List<ChatDTO>();

            foreach (var chat in chats)
            {
                List<string> messageContents = chat.Messages.Select(m => m.Content).ToList();

                chatDTOs.Add(new ChatDTO()
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    CreatedDate = chat.CreatedDate,
                    Description = chat.Description,
                    Logo = chat.Logo,
                    IsDeleted = chat.IsDeleted,
                    customerServiceId = chat.customerServiceId,
                    clientId = chat.clientId,
                    Messages = messageContents
                });
            }

            return Ok(chatDTOs);
        }


        // GET: api/Chats/5
        [HttpGet("GetChat/{id}")]
        public async Task<ActionResult<ChatDTO>> GetChatById(int id)
        {
            var chat = await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            List<string> messageContents = chat.Messages.Select(m => m.Content).ToList();

            ChatDTO chatDTO = new ChatDTO()
            {
                Id = chat.Id,
                Name = chat.Name,
                CreatedDate = chat.CreatedDate,
                Description = chat.Description,
                Logo = chat.Logo,
                IsDeleted = chat.IsDeleted,
                customerServiceId = chat.customerServiceId,
                clientId = chat.clientId,
                Messages = messageContents
            };

            return Ok(chatDTO);
        }


        // GET: api/Chats/GetChatByName/{name}
        [HttpGet("GetChatByName/{name}")]
        public async Task<ActionResult<IEnumerable<ChatDTO>>> GetChatByName(string name)
        {
            var chats = await _context.Chats
                .Include(c => c.Messages)
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

            if (!chats.Any())
            {
                return NotFound();
            }

            List<ChatDTO> chatDTOs = chats.Select(chat => new ChatDTO()
            {
                Id = chat.Id,
                Name = chat.Name,
                CreatedDate = chat.CreatedDate,
                Description = chat.Description,
                Logo = chat.Logo,
                IsDeleted = chat.IsDeleted,
                customerServiceId = chat.customerServiceId,
                clientId = chat.clientId,
                Messages = chat.Messages.Select(m => m.Content).ToList()
            }).ToList();

            return Ok(chatDTOs);

        }


        // POST: api/Chats/AddChat
        [HttpPost("AddChat")]
        public async Task<ActionResult<ChatDTO>> AddChat(ChatDTO chatDTO)
        {
            var chat = new Chat
            {
                Name = chatDTO.Name,
                CreatedDate = chatDTO.CreatedDate,
                Description = chatDTO.Description,
                Logo = chatDTO.Logo,
                IsDeleted = chatDTO.IsDeleted,
                customerServiceId = chatDTO.customerServiceId,
                clientId = chatDTO.clientId,
                Messages = chatDTO.Messages.Select(m => new Message { Content = m }).ToList()
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            chatDTO.Id = chat.Id;

            return CreatedAtAction(nameof(GetChatById), new { id = chat.Id }, chatDTO);
        }


        // DELETE: api/Chats/DeleteChat/{id}
        [HttpDelete("DeleteChat/{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            chat.IsDeleted = true;
            _context.Entry(chat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }




        private bool ChatExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }


        // send messaage from customer service to clients

        [HttpPost("SendMessageToClient")]
        public async Task<IActionResult> SendMessageToClient(string message, string ReceiverId, string SenderId)
        {
            // Store the message in the database
            var newMessage = new Message
            {
                ReceiverId = ReceiverId,
                SenderId = SenderId,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            // Send the message to the specified client
            // await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
            var users = new List<string>() { ReceiverId };
            var userConnections = _context.UserConnections.AsNoTracking().Where(x => users.Contains(x.ApplicationUserId)).Select(x => x.ConnectionId.ToString());

            await _hub.Clients.Clients(userConnections.ToArray<string>()).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(newMessage));
            return Ok(true);
        }


        // send messaage from clients to customer service 

        [HttpPost("SendMessageToCustomerService")]
        public async Task<IActionResult> SendMessageToCustomerService(string message, string ReceiverId, string SenderId)
        {
            var user = await _context.CustomerServices
                .FirstOrDefaultAsync(c => c.Id == ReceiverId);

            if (user != null)
            {
                // Store the message in the database
                var newMessage = new Message
                {
                    ReceiverId = ReceiverId,
                    SenderId = SenderId,
                    Content = message,
                    Timestamp = DateTime.UtcNow
                };
                await _context.Messages.AddAsync(newMessage);
                await _context.SaveChangesAsync();

                // Send the message to the customer service
                // await Clients.User(user.Id).SendAsync("ReceiveMessageFromClient", message);
                var users = new List<string>() { ReceiverId };
                var userConnections = _context.UserConnections.AsNoTracking().Where(x => users.Contains(x.ApplicationUserId)).Select(x => x.ConnectionId.ToString());

                await _hub.Clients.Clients(userConnections.ToArray<string>()).SendAsync("ReceiveMessageFromClient", JsonConvert.SerializeObject(newMessage));

            }
            //else
            //{
            //    // Handle case where customer service is not connected
            //    await _hub.Clients.Caller.SendAsync("NoCustomerServiceAvailable");
            //}
            return Ok(true);

        }


        //deleting messages

        [HttpDelete("DeleteMessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("MessageDeleted", messageId);
            }
            return Ok(true);
        }


        //mark for reading status

        [HttpPut("MarkMessageAsRead/{messageId}")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                await _hub.Clients.User(message.Sender.ToString()).SendAsync("MessageRead", messageId);
            }
            return Ok(true);

        }


        //mark for last seen 

        [HttpPut("UpdateUserStatus/{userId}/{status}")]

        public async Task<IActionResult> UpdateUserStatus(int userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Status = status;
                user.LastSeen = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await _hub.Clients.All.SendAsync("UserStatusUpdated", userId, status);
            }
            return Ok(true);
        }


        // notify of typing messages

        [HttpGet("NotifyTyping/{userId}")]
        public async Task<IActionResult> NotifyTyping(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await _hub. Clients.All.SendAsync("UserTyping", userId);
            }
            return Ok(true);
        }
    }
}
