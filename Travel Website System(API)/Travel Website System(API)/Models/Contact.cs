namespace Travel_Website_System_API_.Models
{
    public class Contact
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false; // Default to not deleted


    }
}
