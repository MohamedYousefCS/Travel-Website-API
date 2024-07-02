namespace Travel_Website_System_API_.DTO.PaymentClasses
{
    public class PayPalSettings
    {
        // Business account credentials for sandbox environment,merchant account not platform account 
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

}
