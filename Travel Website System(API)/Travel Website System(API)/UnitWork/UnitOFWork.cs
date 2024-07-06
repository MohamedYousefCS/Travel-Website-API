using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.UnitWork
{
    // register all repos objects here 

    public class UnitOFWork
    {
       public ApplicationDBContext db;
        IGenericRepo<Payment> paymentRepo;
        IGenericRepo<BookingPackage> bookingPackageRepo;
        IGenericRepo<BookingService> bookingServiceRepo;
        IBookingPackageRepo custombookingPackageRepo;
        IBookingServiceRepo custombookingServiceRepo;
        public UnitOFWork(ApplicationDBContext db)
        {
            this.db = db;
        }
        // declaring public properties for repos , and enable get only 
        // these will be called in controllers 

        public IGenericRepo<Payment> PaymentRepo
        {
            get
            {
                if (paymentRepo == null)
                {
                    paymentRepo = new GenericRepo<Payment>(db);
                }
                return paymentRepo;
            }
        }
        //BookingPackageRepo :this  will be used in controllers
        public IGenericRepo<BookingPackage> BookingPackageRepo
        {
            get
            {
                if (bookingPackageRepo == null)
                {
                    bookingPackageRepo = new GenericRepo<BookingPackage>(db);
                }
                return bookingPackageRepo;
            }
        }
        public IGenericRepo<BookingService> BookingServiceRepo
        {
            get
            {
                if (bookingServiceRepo == null)
                {
                    bookingServiceRepo = new GenericRepo<BookingService>(db);
                }
                return bookingServiceRepo;
            }
        }
        // Custom Repos to include relative Data
        public IBookingPackageRepo CustombookingPackageRepo
        {
            get
            {
                if (custombookingPackageRepo == null)// to check there is only one object of the repo
                {
                    custombookingPackageRepo = new BookingPackageRepo(db);
                }
                return custombookingPackageRepo;
            }
        }
        public IBookingServiceRepo CustombookingServiceRepo
        {
            get
            {
                if (custombookingServiceRepo == null)// to check there is only one object of the repo
                {
                    custombookingServiceRepo = new BookingServiceRepo(db);
                }
                return custombookingServiceRepo;
            }
        }
        
        public void  Save()
        {
            db.SaveChanges();
        }
        public async void saveAsynch()
        {
            await db.SaveChangesAsync();
        }
    }
}
