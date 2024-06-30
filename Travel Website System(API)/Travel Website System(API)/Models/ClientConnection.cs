namespace Travel_Website_System_API_.Models
{

    public class ClientConnection
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
