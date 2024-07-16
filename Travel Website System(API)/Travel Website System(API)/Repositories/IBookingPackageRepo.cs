using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public interface IBookingPackageRepo
    {

        public bool GetAllBokking(int id);

        public List<BookingPackage> selectAll();
        public BookingPackage GetById(int id);

        public List<BookingPackage> SelectAllBookingforClient(string clientId);
        public List<BookingPackage> GetAllPaidBookingsForClient(string clientId);
        public void DeleteBooking (int id);

    }
}
