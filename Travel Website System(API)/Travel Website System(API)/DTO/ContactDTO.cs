using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.DTO
{
    public class ContactDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The field Name must be a string with a minimum length of 3 and a maximum length of 20.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "The field Message must be a string with a minimum length of 10 and a maximum length of 1000.")]
        public string Message { get; set; }

    }
}
