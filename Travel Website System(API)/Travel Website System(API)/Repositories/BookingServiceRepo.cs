using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;

using Travel_Website_System_API_.UnitWork;

namespace Travel_Website_System_API_.Repositories
{
    public class BookingServiceRepo:IBookingServiceRepo
    {
        private ApplicationDBContext db;

        public BookingServiceRepo(ApplicationDBContext db)
        {
            this.db = db;
        }

        public BookingService GetById(int id)
        {
            return db.BookingServices.Include(s =>s.Payment).Include(s =>s.client).ThenInclude(s=>s.ApplicationUser)
               .Include(s =>s.service).FirstOrDefault(s => s.Id == id);
        }
        public List<BookingService> GetAll()
        {
            return db.BookingServices.Include(s => s.Payment).Include(s => s.client).ThenInclude(s => s.ApplicationUser)
               .ToList();
        }

        // this to get relative data and allbooking for a client with  those has paid 
        public List<BookingService> GetAllPaidBookingsForClient(string clientId)
        {
            return db.BookingServices.Include(b => b.client).ThenInclude(c => c.ApplicationUser).Include(s=>s.service).
                Include(b => b.Payment)
                            .Where(b => b.clientId.Equals(clientId) && b.Payment != null)
                            .ToList();
        }


    }
}
