using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.UnitWork;

namespace Travel_Website_System_API_.Repositories
{
    // to get all booking relative data, this repersents customRepo in unitOf work
    public class BookingPackageRepo:IBookingPackageRepo
    {
        ApplicationDBContext db;
        public BookingPackageRepo(ApplicationDBContext db)
        {
            this.db = db;
        }
        public bool GetAllBokking(int id)
        {
            return db.BookingPackages.Any(bp => bp.packageId == id);
        }

        public List<BookingPackage> selectAll()
        {
            return db.BookingPackages.Include(b=>b.Payment).Include(b=>b.client).Include(b=>b.package).ToList();
        }

        public BookingPackage GetById(int id)
        {
          return  db.BookingPackages.Include(b => b.Payment).Include(b => b.client).ThenInclude(c=>c.ApplicationUser).Include(b => b.package).SingleOrDefault(b =>b.Id == id);
        }
        // this to get relative data and allbooking for a client with  no payment 
        public List<BookingPackage> SelectAllBookingforClient(string clientId)
        {
            return db.BookingPackages.Include(b => b.client).ThenInclude(c => c.ApplicationUser).Include(b => b.package).
                Where(b => b.clientId.Equals(clientId) && b.Payment == null&& b.IsDeleted ==false)
                .ToList();
        }

        // this to get relative data and allbooking for a client with  those have paid 
        public List<BookingPackage> GetAllPaidBookingsForClient(string clientId)
        {
            return db.BookingPackages.Include(b => b.client).ThenInclude(c => c.ApplicationUser).Include(b => b.package).
                Include(b=>b.Payment)
                            .Where(b => b.clientId.Equals(clientId) && b.Payment!=null)
                            .ToList();
        }
   
        public void DeleteBooking(int id )
        {
            var bookingPackage = db.BookingPackages.SingleOrDefault(b => b.Id == id);
            if (bookingPackage != null)
            {
                var package = db.Packages.SingleOrDefault(s => s.Id == bookingPackage.packageId);
                package.QuantityAvailable++;
                db.Packages.Update(package);
                bookingPackage.quantity--;
                bookingPackage.IsDeleted = true;
                db.Packages.Update(package);
                db.SaveChanges();
            }
        }

    }
}
