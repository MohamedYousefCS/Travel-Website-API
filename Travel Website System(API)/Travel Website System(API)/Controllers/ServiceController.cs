using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {


        ApplicationDBContext db;

        public ServiceController(ApplicationDBContext db)
        {
            this.db = db;
        }

        [HttpGet]
        //[Produces("application/json")]
        public ActionResult GetAllServices()
        {
            List<Service> services = db.Services.ToList();

            List<ServiceDTO> servicesDTO = new List<ServiceDTO>();

            foreach (var service in services) {

                servicesDTO.Add(new ServiceDTO {

                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    Image = service.Image,
                    QuantityAvailable = service.QuantityAvailable,
                    StartDate = service.StartDate,
                    price = service.price,
                    isDeleted = service.isDeleted,
                    BookingServices = service.BookingServices.ToList(),
                    LoveServices = service.LoveServices.ToList(),
                    category = service.category,
                    serviceProvider = service.serviceProvider,
                    packages = service.packages

                });
            
            
            }
            return Ok(servicesDTO);
        }


        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            Service service = db.Services.Find(id);
            if (service == null) return NotFound();
            else
            {
                ServiceDTO serviceDTO=new ServiceDTO() { 
                
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Image= service.Image,
                QuantityAvailable = service.QuantityAvailable,
                StartDate = service.StartDate,
                price=service.price,
                isDeleted = service.isDeleted,
                BookingServices = service.BookingServices.ToList(),
                LoveServices = service.LoveServices.ToList(),
                category=service.category,
                serviceProvider=service.serviceProvider,
                packages=service.packages
                
                };
                return Ok(serviceDTO);

            }
           
        }

        [HttpPost]
        public ActionResult AddService(Service service)
        {
            if (service == null) return BadRequest();
            if(!ModelState.IsValid) return BadRequest();
            db.Services.Add(service);
            db.SaveChanges();
            return Ok(service);
        }

        [HttpPut("{id}")]

        public ActionResult PutService(Service service,int id)
        {
            if(service == null) return BadRequest();
            if(service.Id !=id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            db.Entry(service).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            return NoContent();

        }

        [HttpDelete]
        public ActionResult DeleteService(int id) {
        
         Service service = db.Services.Find(id);
            if (service == null) return NotFound();
            db.Services.Remove(service);
            db.SaveChanges();
            return Ok(service);
        
        }


    }
}
