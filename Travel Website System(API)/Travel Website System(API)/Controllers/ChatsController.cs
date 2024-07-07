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
        private readonly ILogger<ChatsController> _logger;


        public ChatsController(ApplicationDBContext context, IHubContext<ChatHub> hub , ILogger<ChatsController> logger)
        {
            _context = context;
            _hub = hub;
            _logger = logger;
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
       // hhhhhhhhh

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

            await _hub.Clients.Clients(userConnections.ToArray<string>()).SendAsync("ReceiveMessageFromCustomer", JsonConvert.SerializeObject(newMessage));
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



        [HttpPost("SendMessageToAll")]
        public async Task<IActionResult> SendMessageToAll(string userId, string message)
        {
            // Find the user in the database
            var user = await _context.ApplicationUsers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

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
            var userConnections = _context.UserConnections.AsNoTracking().Where(x => x.ApplicationUserId == user.Id).Select(x => x.ConnectionId.ToString()).ToList();

            await _hub.Clients.Clients(userConnections).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(newMessage));

            return Ok(true);
        }



        [HttpPost("NotifyUser")]
        public async Task<IActionResult> NotifyUserAsync(string userId, string message)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("NotifyUserAsync: User ID is null or empty.");
                return BadRequest("User ID cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("NotifyUserAsync: Message is null or empty.");
                return BadRequest("Message cannot be null or empty.");
            }

            try
            {
                _logger.LogInformation($"Notifying user '{userId}' with message: {message}");
                await _hub.Clients.User(userId).SendAsync("ReceiveNotification", message);
                _logger.LogInformation("Notification sent successfully.");
                return Ok("Notification sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while notifying the user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while notifying the user.");
            }
        }



        [HttpPost("JoinGroup")]
        public async Task<IActionResult> JoinGroup(UserConnection conn)
        {
            if (conn == null || string.IsNullOrWhiteSpace(conn.ApplicationUser.ToString()))
            {
                _logger.LogWarning("JoinGroup: Connection or ApplicationUser is null or empty.");
                return BadRequest("Invalid connection data.");
            }

            try
            {
                _logger.LogInformation($"User '{conn.ApplicationUser}' is joining the group.");
                await _hub.Clients.All.SendAsync("ReceiveMessage", "CustomerService", $"{conn.ApplicationUser} has joined the group");
                _logger.LogInformation("User joined the group successfully.");
                return Ok("User joined the group successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while joining the group.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while joining the group.");
            }
        }

        // GET: api/Chats/GetAllMessages
        [HttpGet("GetAllMessages")]
        public async Task<ActionResult<IEnumerable<ChatMessagesDTO>>> GetAllMessages()
        {
            var chats = await _context.Chats
                .Include(c => c.Messages)
                .ToListAsync();

            if (!chats.Any())
            {
                return NotFound();
            }

            var chatMessagesDTOs = chats.Select(chat => new ChatMessagesDTO
            {
                ChatId = chat.Id,
                ChatName = chat.Name,
                Messages = chat.Messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Sender = m.SenderId,
                    Receiver = m.ReceiverId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                }).ToList()
            }).ToList();

            return Ok(chatMessagesDTOs);
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
            if (messageId <= 0)
            {
                _logger.LogWarning("MarkMessageAsRead: Invalid messageId provided.");
                return BadRequest("Invalid messageId.");
            }

            try
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message == null)
                {
                    _logger.LogWarning($"MarkMessageAsRead: Message with messageId {messageId} not found.");
                    return NotFound("Message not found.");
                }

                message.IsRead = true;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Message with messageId {messageId} marked as read.");

                await _hub.Clients.User(message.Sender.ToString()).SendAsync("MessageRead", messageId);
                _logger.LogInformation("Message read notification sent successfully.");

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking message as read.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }




        //notify for last seen 

        [HttpPost("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus(int userId, string status)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("UpdateUserStatus: Invalid userId provided.");
                return BadRequest("Invalid userId.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                _logger.LogWarning("UpdateUserStatus: Status cannot be null or empty.");
                return BadRequest("Status cannot be null or empty.");
            }

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"UpdateUserStatus: User with userId {userId} not found.");
                    return NotFound("User not found.");
                }

                user.Status = status;
                user.LastSeen = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User status updated for userId {userId}. New status: {status}");

                await _hub.Clients.All.SendAsync("UserStatusUpdated", userId, status, user.LastSeen);
                _logger.LogInformation("User status notification sent successfully.");

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user status.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }




        // notify of typing messages

        [HttpGet("NotifyTyping/{userId}")]
        public async Task<IActionResult> NotifyTyping(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("NotifyTyping: Invalid userId provided.");
                return BadRequest("Invalid userId.");
            }

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"NotifyTyping: User with userId {userId} not found.");
                    return NotFound("User not found.");
                }

                _logger.LogInformation($"Notifying typing status for userId {userId}.");
                await _hub.Clients.All.SendAsync("UserTyping", userId);
                _logger.LogInformation("Typing status notification sent successfully.");

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while notifying typing status.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
