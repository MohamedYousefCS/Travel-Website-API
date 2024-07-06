using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.UnitWork;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingServiceController : ControllerBase
    {
        UnitOFWork unitOFWork;
        public BookingServiceController(UnitOFWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }
        
        //[Authorize(Roles ="client")]
        [HttpPost]
        public IActionResult Add(BookingServiceDTO bookingServiceDTO)//client id , service id 
        {
            if(bookingServiceDTO == null)
            {
                return BadRequest("the bookingService order cant be null ");
            }
            var existBookService = unitOFWork.BookingServiceRepo.GetAll()
                .FirstOrDefault(b => b.clientId == bookingServiceDTO.ClientId && b.serviceId == bookingServiceDTO.ServiceId);
            // to check uniqeness of clientId and serviceId
            if (existBookService != null) { 
                return BadRequest("A booking service with the same ClientId and ServiceId already exists.");
            }
            var service = unitOFWork.db.Services.SingleOrDefault(s => s.Id == bookingServiceDTO.ServiceId);
            // in this the package is removed if its quantity is =0
            if (service.QuantityAvailable ==0)
            {
                service.isDeleted = true;
                return BadRequest("sorry this service is not valid for booking now");
            }
            // when adding bookingService object the quantity is increased by 1 and the available quantity for the service will be decreased by 1
            bookingServiceDTO.Quantity = unitOFWork.BookingServiceRepo.GetAll()
                .Count(s=>s.serviceId ==bookingServiceDTO.ServiceId)+1;
            bookingServiceDTO.allowingTime = DateTime.Now.AddDays(service.BookingTimeAllowed ?? 0 );// will be added by admin 
            bookingServiceDTO.Date = DateTime.Now;
            var bookingService = new BookingService(){ 
                allowingTime = bookingServiceDTO.allowingTime,
                Date =bookingServiceDTO.Date,
                clientId = bookingServiceDTO.ClientId,
                serviceId = bookingServiceDTO.ServiceId,
                Quantity = bookingServiceDTO.Quantity,
            };
            unitOFWork.BookingServiceRepo.Add(bookingService);
            unitOFWork.Save();
            // when adding bookingService object the quantity is increased by 1 and the available quantity for the service 
            // is decreased by 1, and the vise of deleting bookingservice
            service.QuantityAvailable--;
            unitOFWork.db.Services.Update(service);
            unitOFWork.Save();
            bookingServiceDTO.Id = bookingService.Id;
            var savedBooking = unitOFWork.CustombookingServiceRepo.GetById(bookingService.Id);
            bookingServiceDTO = new BookingServiceDTO
            {
                Id = savedBooking.Id,
                Date = savedBooking.Date,
                Quantity = savedBooking.Quantity,
                ClientId = savedBooking.clientId,
                ServiceId = savedBooking.serviceId,
                allowingTime = savedBooking.allowingTime,
                price = service.price??0,
            };
            // will return object in response where i can get it id in ui to pass it in payment
            return CreatedAtAction(nameof(GetByID), new { id = bookingService.Id }, bookingServiceDTO);
        }

        [HttpGet("{id}")] 
        public IActionResult GetByID(int id)// return Dto
        {
            if(id == 0)
            {
                return BadRequest("please enter valid id");
            }
            // here i will get relative data by lazy loading
            var bookingService = unitOFWork.CustombookingServiceRepo.GetById(id);
            if(bookingService == null)
            {
                return NotFound();
            }
            BookingServiceDTO bookingServiceDTO = new BookingServiceDTO()
            {
                Id = bookingService.Id,
                ClientId = bookingService.clientId,
                ServiceId = bookingService.serviceId,
                Quantity = bookingService.Quantity,
                allowingTime = bookingService.allowingTime,
                Date = bookingService.Date,
                price = bookingService?.service?.price ?? 0
                //ClientName = bookingService.client?.user?.Fname ?? "unknown"
            };
            return Ok(bookingServiceDTO);
        }
        /* this is for Deleting Bookingpackage/service
        //  when booking is deleted and who will delete it?
        // 1- by admin when allowing time of booking is expired
        // 2-  or when package is deleted true , no may be booked before becoming is deleted true (if bookingquantity=0)
        // i will apply on 1, so if time now > time allowing this means it should removed and before this we will update 
        // availablequantity in service/package by increasing it by 1 and decresae booking quantity in bookingservice/package
        // tables by decreasing it by 1 then remove the bookingrow
        */

        /* Deleting package/service row ,package is deleted true if
        //1- bookingquantity in table package/service  is =0 , when the use booking the package it will reduce its booking 
         //quantity -1 in table package/service

          //2- or when bookingpackge/service in booking table the quantity of booking foreach package will increased by one
          //and compared with the quantity in package/service table if are euals make this package/service is deleted = true;
         */

       // [Authorize(Roles = "admin,superAdmin")]
        [HttpDelete ("{id}")]// delete and get dont have body in request
        public IActionResult DeleteBooking(int id)
        {
            var bookingService = unitOFWork.BookingServiceRepo.GetById(id);
            if (bookingService == null)
            {
                return NotFound();
            }
            if(bookingService.allowingTime != null && bookingService.allowingTime < DateTime.Now)
            {
                // get the current service in booking row
                var service = unitOFWork.db.Services.SingleOrDefault(s => s.Id == bookingService.serviceId);
                // if i declared servicerepo
                //var service = unitOFWork.serviceRepo.getAll().SingleOrDefault(s => s.serviceId == bookingService.serviceId); 
                service.QuantityAvailable++;
                unitOFWork.db.Services.Update(service);// serviceRepo.update(service)
                bookingService.Quantity--;
                unitOFWork.BookingServiceRepo.Delete(bookingService.Id);
                unitOFWork.Save();
                return Ok("removed");// or bookingServiceDTO
            }
            else
            {
                // Allow booking removal only if allowing time has expired
                return BadRequest("Booking service cannot be removed as allowing time has not expired yet.");
            }    
        }

        // [Authorize(Roles ="client,admin,superAdmin")]
        [HttpGet("AllPaidBookings/{clientId}")]
        public IActionResult GetAllPaidBookingsForClient(string clientId)
        {

            if (string.IsNullOrEmpty(clientId))
            {
                return NotFound("Client ID cannot be null or empty.");
            }
            var AllPaidBookings = unitOFWork.CustombookingServiceRepo.GetAllPaidBookingsForClient(clientId);
            if (AllPaidBookings == null) { return Ok("there is no paid bookings"); }
            var AllPaidBookingsDTO = new List<BookingServiceDTO>();

            foreach (var item in AllPaidBookings)
            {
                AllPaidBookingsDTO.Add(
                    new BookingServiceDTO
                    {
                        Id = item.Id,
                        ClientId = item.clientId,
                        ServiceId = item.serviceId,
                        Quantity = item.Quantity,
                        allowingTime = item.allowingTime,
                        Date = item.Date,
                        price = item?.service?.price ?? 0
                    });
            }
            return Ok(AllPaidBookingsDTO);
        }
    }
}
