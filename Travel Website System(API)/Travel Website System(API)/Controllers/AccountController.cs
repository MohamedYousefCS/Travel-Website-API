using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.IO;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly IGenericRepo<Client> clientGenericRepo;
        private readonly IGenericRepo<Admin> adminGenericRepo;
        private readonly IGenericRepo<CustomerService> cusSerGenericRepo;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDBContext context, IGenericRepo<Client> clientGenericRepo, IGenericRepo<Admin> adminGenericRepo, IGenericRepo<CustomerService> cusSerGenericRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            this.clientGenericRepo = clientGenericRepo;
            this.adminGenericRepo = adminGenericRepo;
            this.cusSerGenericRepo = cusSerGenericRepo;
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

                user.Role = registerDto.Role;
               



                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerDto.Role.ToLower());

                    if (registerDto.Role != "superAdmin")
                    {
                        CreateUserAccordingToHisType(user, registerDto.Role);
                    }
                    return Ok(new { Message = "User registered successfully" });
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }
            return BadRequest(ModelState);

            /*Client client = new Client();


            client.ApplicationUser = user;

            clientGenericRepo.Add(client);
            clientGenericRepo.Save();*/


           /* return Ok(new { Message = "Client registered successfully" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(ModelState);*/
        }



 private void CreateUserAccordingToHisType(ApplicationUser user, string role)
 {
    switch (role.ToLower())
    {
        case "admin":
            Admin admin = new Admin();
                    
            admin.ApplicationUser = user;
            adminGenericRepo.Add(admin);
            adminGenericRepo.Save();
            break;
        case "customerService":
           CustomerService customerService = new CustomerService();
            customerService.ApplicationUser = user;
            cusSerGenericRepo.Add(customerService);
            cusSerGenericRepo.Save();
            break;
        case "client":
            Client client = new Client();
            client.ApplicationUser = user;
            clientGenericRepo.Add(client);
            clientGenericRepo.Save();
            break;
    }
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
                            case "superAdmin":
                                return Ok(new { Message = "superAdmin Login successful" });
                            case "admin":
                                return Ok(new { Message = "admin Login successful" });
                            case "client":
                                return Ok(new { Message = "client Login successful" });
                            case "customerService":
                                return Ok(new { Message =" CustomerService Login successful" });
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
