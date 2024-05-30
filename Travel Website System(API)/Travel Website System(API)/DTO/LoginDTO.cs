using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.DTO
{
    public class LoginDTO
    {
       
        
        
        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter the password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
