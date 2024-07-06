using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public interface IBookingServiceRepo
    {
        public BookingService GetById(int id);
        public List<BookingService> GetAll();
        public List<BookingService> GetAllPaidBookingsForClient(string clientId);
    }
}
