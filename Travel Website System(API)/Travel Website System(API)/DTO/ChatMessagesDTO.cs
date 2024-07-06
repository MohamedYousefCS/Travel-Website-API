namespace Travel_Website_System_API_.DTO
{
    public class ChatMessagesDTO
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public List<MessageDto> Messages { get; set; }
    }
}
