using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public ActionResult Add(BookingPackageDTO bookingPackageDTO ) {// get client id , package id , 
            if(ModelState.IsValid)
            {
                // to check the client has booked this package before or not
                var model = unitOFWork.BookingPackageRepo.GetAll()
                    .FirstOrDefault(p=>p.clientId==bookingPackageDTO.clientId&& p.packageId==bookingPackageDTO.packageId);
                if(model!=null)//if exists
                {
                    return BadRequest("this booking for the same client and this package is found before");
                }
                var package = unitOFWork.db.Packages.SingleOrDefault(p => p.Id == bookingPackageDTO.packageId);
                if(package.QuantityAvailable == 0)
                {
                    package.isDeleted = true;// package will be not in website and you cant booking it
                    return BadRequest("this package is not availabe now ");
                }
                bookingPackageDTO.Date = DateTime.Now;
                bookingPackageDTO.allowingTime = DateTime.Now.AddDays(20);
                //  count the number of bookings for the specified package
                var bookingCountForPackage = unitOFWork.db.BookingPackages.Count(bp => bp.packageId == bookingPackageDTO.packageId);
                bookingPackageDTO.quantity = bookingCountForPackage + 1;
                var bookingPackage = new BookingPackage
                {
                    //Id = bookingPackageDTO.BookingPackageId,// is i dentity
                    clientId = bookingPackageDTO.clientId,
                    quantity = bookingPackageDTO.quantity,
                    Data = bookingPackageDTO.Date,
                    allowingTime = bookingPackageDTO.allowingTime,
                    packageId = bookingPackageDTO.packageId,
                };
                unitOFWork.BookingPackageRepo.Add(bookingPackage);
                // after adding new booking for the specific package the availablequantity in package table will decreased by 1
                package.QuantityAvailable--;
                unitOFWork.db.Packages.Update(package);
                unitOFWork.save();
                // will return object in response where i can get it id in ui to pass it in payment
                return CreatedAtAction(nameof(GetBookingPackageById), new { id = bookingPackage.packageId }, bookingPackage);
                // i returned bookingPackage not dto as when i returned dto the booking id was =0 and not changed 
            }
            else
            {
                ModelState.AddModelError(string.Empty, "invalid data provided");
                return BadRequest(ModelState);
            }
      
        }

        [HttpGet("{id}")]
        public IActionResult GetBookingPackageById(int id)
        {
            // here i get relative data with eager Loading
            var bookingPackage = unitOFWork.CustombookingPackageRepo.GetById(id);// to get relative data 
            if(bookingPackage == null) { return NotFound(); }
           // var clientName = bookingPackage.client?.user?.Fname ?? "Unknown";
            var bookingPackageDTO = new BookingPackageDTO
            {
                BookingPackageId = bookingPackage.Id,
                Date = bookingPackage.Data,
                quantity = bookingPackage.quantity,
                clientId = bookingPackage.clientId,
                packageId = bookingPackage.packageId,
                allowingTime = bookingPackage.allowingTime,
               // clientName = clientName,// i get it for testing only i dont need it now
                price = bookingPackage.package?.Price ?? 0,
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
                var package = unitOFWork.db.Packages.SingleOrDefault(s => s.Id == bookingPackage.packageId);
                //package.QuantityAvailable++;
                unitOFWork.db.Packages.Update(package);// PackageRepo.update(service)
                bookingPackage.quantity--;
                unitOFWork.BookingPackageRepo.Delete(bookingPackage.Id);
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
