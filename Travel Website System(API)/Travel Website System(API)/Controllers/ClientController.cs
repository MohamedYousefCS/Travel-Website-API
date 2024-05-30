using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly IGenericRepo<Client> clientGenericRepo;

        public ClientController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDBContext context, IGenericRepo<Client> clientGenericRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            this.clientGenericRepo = clientGenericRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(ClientRegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();


                user.Fname = registerDto.Name;
                user.Email = registerDto.Email;
                user.PasswordHash = registerDto.Password;
                user.UserName = registerDto.Email;

               string role= user.Role = "client";
                user.UserName = "";
               
               

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role.ToLower());

                    Client client = new Client();

                    
                    client.ApplicationUser = user;

                    clientGenericRepo.Add(client);
                    clientGenericRepo.Save();


                    return Ok(new { Message = "Client registered successfully" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            /*if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Login successful" });
                }

                if (result.IsLockedOut)
                {
                    return BadRequest(new { Message = "User account locked" });
                }
                else
                {
                    return BadRequest(new { Message = "Invalid login attempt" });
                }
            }*/


            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    bool isUserFound = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                    if (isUserFound)
                    {
                        //make the cookies and save them at the client browser
                        await _signInManager.SignInAsync(user, loginDto.RememberMe);

                        //navigate the user according to his role
                        IList<string> userRoles = await _userManager.GetRolesAsync(user);
                        string userRole = userRoles.First();
                        //navigate the user according to his role
                        switch (userRole.ToLower())
                        {
                            case "superadmin":
                                return Ok(new { Message = "Login successful" });
                            case "admin":
                                return Ok(new { Message = "Login successful" });
                            case "client":
                                return Ok(new { Message = "Login successful" });
                            case "customerService":
                                return Ok(new { Message = "Login successful" });
                            default:
                                return Ok(new { Message = "Login successful" });
                        }
                    }
                }
                ModelState.AddModelError("", "The email or password is incorrect");
            }

            return BadRequest(ModelState);
        }
    }
}
