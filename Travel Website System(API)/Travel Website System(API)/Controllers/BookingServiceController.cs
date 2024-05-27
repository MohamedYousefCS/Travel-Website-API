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
        // in add and get by id i will add and get Dto 
        // i used Lazyloading here
        [HttpPost]
        public IActionResult Add(BookingServiceDTO bookingServiceDTO)//client id , service id 
        {
            if(bookingServiceDTO == null)
            {
                return BadRequest("the bookingService order cant be null ");
            }
            var existBookService = unitOFWork.BookingServiceRepo.GetAll()
                .FirstOrDefault(b=>b.clientId==bookingServiceDTO.ClientId && b.serviceId ==bookingServiceDTO.ClientId);
            // to check uniueness of clientId and serviceId
            if (existBookService != null) { 
                return BadRequest("A booking service with the same ClientId and ServiceId already exists.");
            } 
            bookingServiceDTO.Quantity = unitOFWork.BookingServiceRepo.GetAll().Count(s=>s.serviceId ==bookingServiceDTO.ServiceId)+1;
            bookingServiceDTO.allowingTime = DateTime.Now.AddDays(20);// will be added by admin 
            bookingServiceDTO.Date = DateTime.Now;
            var bookingService = new BookingService(){ 
                BookingServiceId = bookingServiceDTO.BookingServiceId,
                allowingTime = bookingServiceDTO.allowingTime,
                Date = DateTime.Now,
                clientId = bookingServiceDTO.ClientId,
                serviceId = bookingServiceDTO.ServiceId,
                Quantity = bookingServiceDTO.Quantity,
            };
            unitOFWork.BookingServiceRepo.Add(bookingService);
            var service = unitOFWork.db.Services.SingleOrDefault(s => s.serviceId == bookingServiceDTO.ServiceId);
            if (service != null)
            {
                service.QuantityAvailable--;
                // when adding bookingService object the quantity is increased by 1 and the available quantity for the service 
                // is decreased by 1, and the vise of deleting bookingservice 
            }
            unitOFWork.db.Services.Update(service);
            unitOFWork.save();
            // will return objcect in response where i can get it id in ui to pass it in payment
           return CreatedAtAction(nameof(GetByID), new { id = bookingServiceDTO.BookingServiceId }, bookingServiceDTO);
         //  return Created();// not will return data in response
        }
        [HttpGet] public IActionResult GetByID(int id)
        {
            if(id == 0)
            {
                return BadRequest("please enter valid id");
            }
            // here i will get relative data by lazy loading
            var bookingService = unitOFWork.BookingServiceRepo.GetById(id);
            if(bookingService == null)
            {
                return NotFound();
            }
            BookingServiceDTO bookingServiceDTO = new BookingServiceDTO()
            {
                BookingServiceId = bookingService.BookingServiceId,
                ClientId = bookingService.clientId,
                ServiceId = bookingService.serviceId,
                Quantity = bookingService.Quantity,
                allowingTime = bookingService.allowingTime,
                Date = bookingService.Date,
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
                var service = unitOFWork.db.Services.SingleOrDefault(s => s.serviceId == bookingService.serviceId);
                // if i declared servicerepo
                //var service = unitOFWork.serviceRepo.getAll().SingleOrDefault(s => s.serviceId == bookingService.serviceId); 
                service.QuantityAvailable++;
                unitOFWork.db.Services.Update(service);// serviceRepo.update(service)
                bookingService.Quantity--;
                unitOFWork.BookingServiceRepo.Delete(bookingService.BookingServiceId);
                unitOFWork.save();
                return Ok("removed");// or bookingServiceDTO
            }
            else
            {
                // Allow booking removal only if allowing time has expired
                return BadRequest("Booking service cannot be removed as allowing time has not expired yet.");
            }    
        }
    }
}
