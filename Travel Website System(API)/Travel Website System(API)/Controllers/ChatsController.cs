using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public ChatsController(ApplicationDBContext context)
        {
            _context = context;
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





    }
}
