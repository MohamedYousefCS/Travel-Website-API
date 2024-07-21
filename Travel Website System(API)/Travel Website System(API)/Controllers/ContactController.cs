using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Models;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {

        private readonly ApplicationDBContext db;

        public ContactController(ApplicationDBContext db)
        {
            this.db = db;
        }


        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] ContactDTO contactDto)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    Name = contactDto.Name,
                    Email = contactDto.Email,
                    Message = contactDto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
                return Ok(contact);
            }
            return BadRequest(ModelState);
        }
         // superadmin can view message only 
        [Authorize(Roles = "superAdmin")]   // send Token

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await db.Contacts
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            return Ok(contacts);
        }

        // superadmin can delete message only 

        [Authorize(Roles = "superAdmin")]   // send Token

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteContact(int id)
        {
            var contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new { Message = "Contact not found." });
            }

            contact.IsDeleted = true;
            await db.SaveChangesAsync();
            return NoContent();
        }


    }
}
