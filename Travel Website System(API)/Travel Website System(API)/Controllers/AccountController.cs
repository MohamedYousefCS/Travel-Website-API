using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly IGenericRepo<Client> _clientGenericRepo;
        private readonly IGenericRepo<Admin> _adminGenericRepo;
        private readonly IGenericRepo<CustomerService> _cusSerGenericRepo;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDBContext context,
            IGenericRepo<Client> clientGenericRepo,
            IGenericRepo<Admin> adminGenericRepo,
            IGenericRepo<CustomerService> cusSerGenericRepo,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _clientGenericRepo = clientGenericRepo;
            _adminGenericRepo = adminGenericRepo;
            _cusSerGenericRepo = cusSerGenericRepo;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        // Register endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register(ClientRegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    Fname = registerDto.Name,
                    Role = registerDto.Role
                };

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

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return BadRequest(ModelState);
        }

        private void CreateUserAccordingToHisType(ApplicationUser user, string role)
        {
            switch (role.ToLower())
            {
                case "admin":
                    var admin = new Admin { ApplicationUser = user };
                    _adminGenericRepo.Add(admin);
                    _adminGenericRepo.Save();
                    break;
                case "customerservice":
                    var customerService = new CustomerService { ApplicationUser = user };
                    _cusSerGenericRepo.Add(customerService);
                    _cusSerGenericRepo.Save();
                    break;
                case "client":
                    var client = new Client { ApplicationUser = user };
                    _clientGenericRepo.Add(client);
                    _clientGenericRepo.Save();
                    break;
            }
        }

        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    var isUserFound = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                    if (isUserFound)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var userRole = userRoles.FirstOrDefault();

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Fname),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Role, userRole)
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["JWT:ValidIssuer"],
                            audience: _configuration["JWT:ValidAudience"],
                            claims: claims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: creds);

                        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

                        return Ok(new
                        {
                            token = stringToken,
                            Role = userRole
                        });
                    }
                }
                ModelState.AddModelError("", "The email or password is incorrect");
            }
            return BadRequest(ModelState);
        }

        // External login endpoints
        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return BadRequest(new { Message = $"Error from external provider: {remoteError}" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest(new { Message = "Error loading external login information." });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            if (result.IsLockedOut)
            {
                return Forbid();
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return Ok(new { Message = "External login successful. Please complete your registration.", Email = email });
            }
        }

        [HttpPost("external-login-confirmation")]
        public async Task<IActionResult> ExternalLoginConfirmation([FromBody] ExternalLoginConfirmationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return BadRequest(new { Message = "Error loading external login information during confirmation." });
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        var token = GenerateJwtToken(user);
                        return Ok(new { Token = token });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return BadRequest(ModelState);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Logout endpoint
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        // Forget Password endpoint
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Ok(new { message = "Email not found" });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{model.url}?email={model.Email}&token={token}";

                await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");

                return Ok(new { message = "You received a password reset link on your email." });
            }
            return BadRequest("Cannot send email");
        }

        // Reset Password endpoint
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
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

        // Change Password endpoint
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
