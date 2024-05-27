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
                BookingServiceId= bookingService.BookingServiceId,
                ClientId = bookingService.clientId,
                ServiceId = bookingService.serviceId,
                Quantity = bookingService.Quantity,
                allowingTime=bookingService.allowingTime,
                Date = bookingService.Date,
            };
            return Ok(bookingServiceDTO); 
        }
    }
}
