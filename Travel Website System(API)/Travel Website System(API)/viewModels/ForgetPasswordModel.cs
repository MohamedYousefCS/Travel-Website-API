using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.viewModels
{
    public class ForgetPasswordModel
    {
        [Required(ErrorMessage = "you should enter Email")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        public bool IsEmailSent { get; set; }
    }
}
