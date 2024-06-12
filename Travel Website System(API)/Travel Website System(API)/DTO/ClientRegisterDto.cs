using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.DTO
{
    public class ClientRegisterDto
    {
        [Required(ErrorMessage = "first name is required")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "The name must not be more than 25 characters and not less than 2 characters")]
        public string Name { get; set; }
       
       

        [Required(ErrorMessage = "Email field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is incorrect")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter the password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@$%^&*()-_=+\\\|\[\]{};:'"",.<>/?]+).{8,}$", ErrorMessage = "The password must contain at least one uppercase letter, at least one number, and at least one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must enter the password confirmation field")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password mismatch")]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }

    }

    
}
