namespace Travel_Website_System_API_.DTO.PaymentClasses
{
    public class PaymentRequest// will be used in fromt end 
    {
        public decimal Amount { get; set; }
        public string ?Currency { get; set; }
        public int? BookingPackageId { get; set; }  // Single booking service ID for individual payment
        public int ? BookingServiceId { get; set; }  // Single booking service ID for individual payment
       // public List<int>? BookingPackageIds { get; set; } // List of booking package IDs for multiple payments
        //public List<int>? BookingServiceIds { get; set; } // List of booking service IDs for multiple payments

    }
    public class PaymentForManyBookin
    {
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public List<int>? BookingPackageIds { get; set; } // List of booking package IDs for multiple payments
        public List<int>? BookingServiceIds { get; set; } // List of booking service IDs for multiple payments

    }
}
