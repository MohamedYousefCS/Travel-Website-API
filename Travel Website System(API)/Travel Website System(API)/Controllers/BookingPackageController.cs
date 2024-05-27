using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.UnitWork;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Travel_Website_System_API_.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookingPackageController : ControllerBase
    {
        UnitOFWork unitOFWork;
        public BookingPackageController(UnitOFWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }
        // i will try by adding with DTO
        [HttpPost]
        [Consumes("application/json")]
        // public ActionResult Add([FromBody]BookingPackage bookingPackage )
        public ActionResult Add(BookingPackage bookingPackage ) {// get client id , package id , 
            // if i added all bookingPackage not the bookingPackageDTO
            // in api we must add navigation properties not as mvc if the there is an foreign key it load all navproperty object data 
            // by default so in add new object i only give value for the forign key only in mvc
            var client = unitOFWork.db.Clients.Include(c=>c.user).SingleOrDefault(c=>c.clientId==bookingPackage.clientId);
           var package = unitOFWork.db.Packages.Include(c=>c.BookingPackages).SingleOrDefault(p=>p.packageId== bookingPackage.packageId);
            bookingPackage.Date = DateTime.Now;
           bookingPackage.allowingTime = DateTime.Now.AddDays(20);
            //  count the number of bookings for the specified package.
            var bookingCountForPackage = unitOFWork.db.BookingPackages.Count(bp => bp.packageId == bookingPackage.packageId);
            bookingPackage.quantity = bookingCountForPackage + 1;
            // this will calculate the all booking package
            //  bookingPackage.quantity = unitOFWork.BookingPackageRepo.GetAll().Count()+1;
            // in api must add explicity relative data objects to allow nulls
            bookingPackage.client=client;
            bookingPackage.package = package;
            unitOFWork.BookingPackageRepo.Add(bookingPackage);
            // after adding new booking for the specific package the availablequantity in package table will decreased by 1
            package.QuantityAvailable--;
            unitOFWork.db.Packages.Update(package);
            unitOFWork.save();
            return CreatedAtAction(nameof(GetBookingPackageById), new { id = bookingPackage.packageId }, bookingPackage);
        }

        [HttpGet("{id}")]
        public IActionResult GetBookingPackageById(int id)
        {
            // here i get relative data with eager Loading
            var bookingPackage = unitOFWork.CustombookingPackageRepo.GetById(id);// to get relative data 
            if(bookingPackage == null) { return NotFound(); }
            var clientName = bookingPackage.client?.user?.Fname ?? "Unknown";
            var bookingPackageDTO = new BookingPackageDTO
            {
                BookingPackageId = bookingPackage.BookingPackageId,
                Date = bookingPackage.Date,
                quantity = bookingPackage.quantity,
                clientId = bookingPackage.clientId,
                packageId = bookingPackage.packageId,
                allowingTime = bookingPackage.allowingTime,
                clientName = clientName,// i get it for testing only i dont need it now
                Bookingprice = bookingPackage.package?.TotalPrice
            };
            return Ok(bookingPackageDTO);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBooking(int id)
        {
            var bookingPackage = unitOFWork.BookingPackageRepo.GetById(id);
            if (bookingPackage == null)
            {
                return NotFound();
            }
            //bookingPackage.allowingTime < DateTime.Now: if the date of today is > date of allowingtime
            if (bookingPackage.allowingTime != null && bookingPackage.allowingTime < DateTime.Now)
            {
                // get the current package in booking row
                var package = unitOFWork.db.Packages.SingleOrDefault(s => s.packageId == bookingPackage.packageId);
                package.QuantityAvailable++;
                unitOFWork.db.Packages.Update(package);// PackageRepo.update(service)
                bookingPackage.quantity--;
                unitOFWork.BookingPackageRepo.Delete(bookingPackage.BookingPackageId);
                unitOFWork.save();
                return Ok("removed");
            }
            else
            {
                // Allow booking removal only if allowing time has expired
                return BadRequest("Booking package cannot be removed as allowing time has not expired yet.");
            }
        }


    }
}
