using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserRepo _userRepo;
        private readonly ApplicationDBContext _context;



        public UserController(UserManager<ApplicationUser> userManager, UserRepo userRepo, ApplicationDBContext context)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _context = context;
        }

        [HttpGet("All")]
        [Authorize(Roles = "superAdmin, admin")]
        public ActionResult<IEnumerable<ApplicationUser>> GetUsers()
        {
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            if (roles.Contains("superAdmin"))
            {
                var allUsers = _userRepo.GetAll().Where(c => c.IsDeleted == false);
                return Ok(allUsers);
            }
            else if(roles.Contains("admin"))
            {
                var adminId = User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (adminId != null)
                {
                    var clients = _userRepo.GetClientsByAdminId(adminId).Where(c => c.IsDeleted == false && c.IsVerified == true);
                    return Ok(clients);
                }
            }
            return NotFound();

        }


        [HttpGet("admins")]
        [Authorize(Roles = "superAdmin")]
        public ActionResult<IEnumerable<Admin>> GetAdmins()
        {
            var admins = _userRepo.GetAllAdmins().Where(c=>c.IsDeleted==false);
            if (admins == null || !admins.Any())
            {
                return NotFound(new { message = "No admins found" });
            }
            return Ok(admins);
        }


        [HttpGet("clients")]
        [Authorize(Roles = "superAdmin, admin")]
        public ActionResult<IEnumerable<Client>> GetClients()
        {
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            if (roles.Contains("superAdmin"))
            {
                var allClients = _userRepo.GetAllClients().Where(c=>c.IsDeleted==false);
                return Ok(allClients);
            }
            else if (roles.Contains("admin"))
            {
                var adminId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (adminId != null)
                {
                    var clients = _userRepo.GetClientsByAdminId(adminId).Where(c => c.IsDeleted == false);
                    if (clients == null || !clients.Any())
                    {
                        return NotFound(new { message = "No clients found for this admin" });
                    }
                    return Ok(clients);
                }
            }

            return Unauthorized(new { message = "Unauthorized access" });
        }




        [HttpGet("customerService")]
        [Authorize(Roles = "superAdmin")]
        public ActionResult<IEnumerable<Admin>> GetCustomerServices()
        {
            var cus = _userRepo.GetAllcustomerServices().Where(c => c.IsDeleted == false);
            if (cus == null || !cus.Any())
            {
                return NotFound(new { message = "No customer services found" });
            }
            return Ok(cus);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return NotFound();
            }

            user.Fname = updatedUser.Fname;
            user.Lname = updatedUser.Lname;
            user.Gender = updatedUser.Gender;
            user.SSN = updatedUser.SSN;
            user.Role = updatedUser.Role;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        } 

        [HttpPut("completeInfo")]
        public async Task<IActionResult> CompleteInfo(UpdateUserDto updateUserDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(updateUserDto.Email);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                user.PassportNumber = updateUserDto.PassportNumber;
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.ResidanceCountry = updateUserDto.ResidanceCountry;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User updated successfully" });
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return BadRequest(ModelState);
        }




        [HttpDelete("{id}")]
public async Task<IActionResult> SoftDeleteUser(string id)
{
    var user = await _userManager.FindByIdAsync(id);
    if (user == null || user.IsDeleted)
    {
        return NotFound();
    }

    // Check if the user is a client and has active booking packages
    
        var hasActiveBookings = await _context.BookingPackages
            .AnyAsync(bp => bp.clientId == user.Id);

        if (hasActiveBookings)
        {
            return BadRequest(new { message = "User cannot be deleted because they have active booking packages." });
        }
    

    user.IsDeleted = true;
    var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded)
    {
        return BadRequest(result.Errors);
    }
    return Ok(new { message = "User deleted successfully" });
}


    }
}
