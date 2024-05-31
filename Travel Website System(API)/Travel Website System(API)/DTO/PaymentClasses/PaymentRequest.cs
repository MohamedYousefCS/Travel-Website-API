namespace Travel_Website_System_API_.DTO.PaymentClasses
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int BookingPackageId { get; set; }  // Add this property

    }
}
