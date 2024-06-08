using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;
using Travel_Website_System_API_.viewModels;

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
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDBContext context, IGenericRepo<Client> clientGenericRepo, IGenericRepo<Admin> adminGenericRepo, IGenericRepo<CustomerService> cusSerGenericRepo,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            this.clientGenericRepo = clientGenericRepo;
            this.adminGenericRepo = adminGenericRepo;
            this.cusSerGenericRepo = cusSerGenericRepo;
            this.configuration = configuration;
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

            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    bool isUserFound = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                    if (isUserFound)
                    {
                        //get role for user
                        IList<string> userRoles = await _userManager.GetRolesAsync(user);
                        string userRole = userRoles.First();
                        //make the cookies and save them at the client browser
                        //  await _signInManager.SignInAsync(user, loginDto.RememberMe);
                        var claims = new List<Claim>();
                        claims.Add(new Claim( ClaimTypes.Name, user.Fname));
                        claims.Add(new Claim( ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        claims.Add(new Claim( ClaimTypes.Role, userRole));

                       // string securityKey = "my key is mohamed helmy mohamed welcom every one working on this project";
                        SecurityKey sKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
                        SigningCredentials signingCredentials = new SigningCredentials(sKey, SecurityAlgorithms.HmacSha256); 

                        //generate token
                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: configuration["JWT:ValidIssuer"],
                            audience: configuration["JWT:ValidAudience"],
                            claims: claims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: signingCredentials
                            );

                        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

                        //navigate the user according to his role
                        return Ok(new
                        {
                            token = stringToken,
                            Role = userRole
                        });
                    }
                }
                ModelState.AddModelError("", "The email or password is incorrect");
            }

            return BadRequest();
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }


        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Ok(new { message = "email dosnt exist" });
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = model.Email }, Request.Scheme);


                model.IsEmailSent = true;
                return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
            }
            return BadRequest(ModelState);
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    
                    return BadRequest(new { message = "Invalid request" });
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Password has been reset successfully" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }





        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    return Ok(new { message = "Password changed successfully" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }

    



    }
}
