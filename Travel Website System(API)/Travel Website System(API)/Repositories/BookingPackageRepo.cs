using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class BookingPackageRepo:IBookingPackageRepo
    {
        ApplicationDBContext db;
        public BookingPackageRepo(ApplicationDBContext db)
        {
            this.db = db;
        }
        public List<BookingPackage> selectAll()
        {
            return db.BookingPackages.Include(b=>b.Payment).Include(b=>b.client).Include(b=>b.package).ToList();
        }

        public BookingPackage GetById(int id)
        {
          return  db.BookingPackages.Include(b => b.Payment).Include(b => b.client).ThenInclude(c=>c.ApplicationUser).Include(b => b.package).SingleOrDefault(b =>b.Id == id);
        }
        // this to get relative data and booking with no payment 
        public List<BookingPackage> SelectAllBookingforClient(string clientId)
        {
            return db.BookingPackages.Include(b => b.client).ThenInclude(c => c.ApplicationUser).Include(b => b.package).
                Where(b => b.clientId.Equals(clientId) && b.Payment == null)
                .ToList();
        }


    }
}
