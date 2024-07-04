using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[Authorize(Roles = "superAdmin, admin")]

    public class ServiceController : ControllerBase
    {


        GenericRepository<Service> serviceRepo;
        private readonly IServiceRepo _serviceRepo;


        ApplicationDBContext db;
        public ServiceController(GenericRepository<Service> serviceRepo, ApplicationDBContext db,IServiceRepo repo)
        {
            this.serviceRepo = serviceRepo;
            this._serviceRepo = repo;
            this.db = db;
        }

        [HttpGet]
        //[Produces("application/json")]
        public ActionResult GetAllServices(int pageNumber = 1, int pageSize = 10)
        {
            List<Service> services = serviceRepo.GetAllWithPagination(pageNumber, pageSize);
            int totalServices = serviceRepo.GetTotalCount();

            List<ServiceDTO> servicesDTO = new List<ServiceDTO>();

            foreach (var service in services)
            {

                servicesDTO.Add(new ServiceDTO
                {

                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    Image = service.Image,
                    QuantityAvailable = service.QuantityAvailable,
                    StartDate = service.StartDate,
                    price = service.price,
                    isDeleted = service.isDeleted,
                    categoryId = service.categoryId,
                    serviceProviderId = service.serviceProviderId,
                    BookingTimeAllowed = service.BookingTimeAllowed

                });


            }

            var response = new PaginatedResponse<ServiceDTO>
            {
                TotalCount = totalServices,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = servicesDTO
            };

            return Ok(response);



        }






        [HttpGet("Search/{searchItem}")]
        public ActionResult<List<string>> Search(string searchItem)
        {
            var serviceNames = _serviceRepo.Search(searchItem);
            return Ok(serviceNames);
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            Service service = serviceRepo.GetById(id);
            if (service == null) return NotFound();
            else
            {
                ServiceDTO serviceDTO = new ServiceDTO() {

                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    Image = service.Image,
                    QuantityAvailable = service.QuantityAvailable,
                    StartDate = service.StartDate,
                    price = service.price,
                    isDeleted = service.isDeleted,
                    categoryId = service.categoryId,
                    serviceProviderId = service.serviceProviderId,
                    BookingTimeAllowed= service.BookingTimeAllowed
                };
                return Ok(serviceDTO);

            }

        }



        [HttpGet("{name:alpha}")]
        public ActionResult GetByName(string name)
        {
            Service service = _serviceRepo.GetByName(name);
            if (service == null) return NotFound();
            else
            {
                ServiceDTO serviceDTO = new ServiceDTO()
                {

                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    Image = service.Image,
                    QuantityAvailable = service.QuantityAvailable,
                    StartDate = service.StartDate,
                    price = service.price,
                    isDeleted = service.isDeleted,
                    categoryId = service.categoryId,
                    serviceProviderId = service.serviceProviderId,
                    BookingTimeAllowed = service.BookingTimeAllowed
                };
                return Ok(serviceDTO);

            }

        }

        [HttpPost]
        public ActionResult AddService(ServiceDTO serviceDTO)
        {
            if (serviceDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            Service service = new Service() {
                //Id= serviceDTO.Id,
                Name = serviceDTO.Name,
                Description = serviceDTO.Description,
                Image = serviceDTO.Image,
                QuantityAvailable = serviceDTO.QuantityAvailable,
                StartDate = serviceDTO.StartDate,
                price = serviceDTO.price,
                isDeleted = serviceDTO.isDeleted,
                categoryId = serviceDTO.categoryId,
                serviceProviderId = serviceDTO.serviceProviderId,
                BookingTimeAllowed= serviceDTO.BookingTimeAllowed
            };
            serviceRepo.Add(service);
            serviceRepo.Save();
            return Ok(serviceDTO);

        }

        [HttpPut("{id}")]

        public ActionResult EditService(ServiceDTO serviceDTO, int id)
        {
            if (serviceDTO == null) return BadRequest();
            if (serviceDTO.Id != id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            Service service = new Service()
            {
                Id = serviceDTO.Id,
                Name = serviceDTO.Name,
                Description = serviceDTO.Description,
                Image = serviceDTO.Image,
                QuantityAvailable = serviceDTO.QuantityAvailable,
                StartDate = serviceDTO.StartDate,
                price = serviceDTO.price,
                isDeleted = serviceDTO.isDeleted,
                categoryId = serviceDTO.categoryId,
                serviceProviderId = serviceDTO.serviceProviderId,
                BookingTimeAllowed = serviceDTO.BookingTimeAllowed
            };
            serviceRepo.Edit(service);
            serviceRepo.Save();
            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            var service = serviceRepo.GetById(id);
            if (service == null) return NotFound();

            // Check if there are any bookings associated with the service
            var hasBookings = _serviceRepo.GetAllBookings(id);
            if (hasBookings)
            {
                // Return a message indicating the service cannot be deleted due to bookings
                return BadRequest("The service cannot be deleted because it is reserved.");
            }

            // Perform a soft delete by marking the service as deleted
            service.isDeleted = true;
            serviceRepo.Edit(service);

            serviceRepo.Save();
            return Ok(service);
        }



    }
}
