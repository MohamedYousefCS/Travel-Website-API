using Travel_Website_System_API.Models;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.UnitWork
{
    // register all repos objects here 
    
    public class UnitOFWork
    {
        ApplicationDBContext db;
        // these are fields not properties
        IGenericRepo<Payment> paymentRepo;
        IGenericRepo<BookingPackage> bookingPackageRepo;
        IGenericRepo<BookingService> bookingServiceRepo;
        public UnitOFWork(ApplicationDBContext db) { 
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

    }
}
