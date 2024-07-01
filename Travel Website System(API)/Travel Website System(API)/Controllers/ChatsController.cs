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

        // GET: api/Chats
        [HttpGet("GetChats")]
        public  ActionResult GetChats()
        {
            List<Chat> chats= _context.Chats.Where(c=>c.IsDeleted==false)
                .Include(c => c.Messages)
                .ToList();

            List<ChatDTO> chatDTOs = new List<ChatDTO>();

            foreach (var chat in chats)
            {
                chatDTOs.Add(new ChatDTO()
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    CreatedDate = DateTime.Now,
                    Description = chat.Description,
                    Logo = chat.Logo,
                    IsDeleted = chat.IsDeleted,
                    customerServiceId=chat.customerServiceId,
                    clientId=chat.clientId
                });
            }
            return Ok(chatDTOs);
        }

        // GET: api/Chats/5
        [HttpGet("GetChat/{id}")]
        public async Task<ActionResult<Chat>> GetChat(int id)
        {
            var chat = await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            return chat;
        }

        // POST: api/Chats
        [HttpPost("PostChat")]
        public async Task<ActionResult<Chat>> PostChat(Chat chat)
        {
            chat.CreatedDate = DateTime.UtcNow;
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChat), new { id = chat.Id }, chat);
        }

        // PUT: api/Chats/5
        [HttpPut("PutChat/{id}")]
        public async Task<IActionResult> PutChat(int id, Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest();
            }

            _context.Entry(chat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Chats/5
        [HttpDelete("DeleteChat/{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }

    }
}
