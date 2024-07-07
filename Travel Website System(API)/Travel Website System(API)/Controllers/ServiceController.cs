using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Identity.Client;
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
        public ServiceController(GenericRepository<Service> serviceRepo, ApplicationDBContext db, IServiceRepo repo)
        {
            this.serviceRepo = serviceRepo;
            this._serviceRepo = repo;
            this.db = db;
        }

        [HttpGet]
        //[Produces("application/json")]
        //[Authorize(Roles = "superAdmin, admin")]

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
                    EndDate = service.EndDate,
                    Duration = (service.EndDate.Value - service.StartDate.Value).Days + 1, // +1 to include both start and end date
                    price = service.price,
                    isDeleted = service.isDeleted,
                    categoryId = service.categoryId,
                    serviceProviderId = service.serviceProviderId,
                    BookingTimeAllowed = service.BookingTimeAllowed

                }); ;
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
        // amira
        [HttpGet("HotelServices")]
       // [Authorize(Roles = "superAdmin, admin")]

        //[Produces("application/json")]
        public ActionResult GetAllHotelServices(int pageNumber = 1, int pageSize = 10)
        {
            List<Service> services = serviceRepo.GetAllHotelsWithPagination(pageNumber, pageSize);
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
                    EndDate = service.EndDate,
                    price = service.price,
                    isDeleted = service.isDeleted,
                    categoryId = service.categoryId,
                    Duration = (service.EndDate.Value - service.StartDate.Value).Days + 1, // +1 to include both start and end date
                    serviceProviderId = service.serviceProviderId,
                    BookingTimeAllowed = service.BookingTimeAllowed,
                    HoltelLocation = service.HoltelLocation, // Hotels Services
                    //NumberOFAvailableRooms = service.NumberOFAvailableRooms, // Hotels Services
                    NumberOFPersons = service.NumberOFPersons // Hotels Services

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
       // [Authorize(Roles = "superAdmin, admin")]

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
            DateTime? packageStartDate = null;
            DateTime? packageEndDate = null;

            // Retrieve all packagServices  rows associated with this service
            var packageServices = db.PackageService
                                    .Where(ps => ps.ServiceId == service.Id)
                                    .ToList();
            if (packageServices != null && packageServices.Any())
            {
                // Assuming you want to get the earliest start date and latest end date
                packageStartDate = packageServices.Min(ps => ps.Package.startDate);
                packageEndDate = packageServices.Max(ps => ps.Package.EndDate);
            }
            ServiceDTO serviceDTO = new ServiceDTO()
            {

                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Image = service.Image,
                QuantityAvailable = service.QuantityAvailable,
                StartDate = service.StartDate,
                EndDate = service.EndDate,
                price = service.price,
                isDeleted = service.isDeleted,
                categoryId = service.categoryId,
                Duration = (service.EndDate.Value - service.StartDate.Value).Days + 1, // +1 to include both start and end date
                serviceProviderId = service.serviceProviderId,
                BookingTimeAllowed = service.BookingTimeAllowed,
                GoingFlightDestination = service.GoingFlightDestination, // Flight Services
                GoingFlightSource = service.GoingFlightSource, // Flight Services
                ComingBackFlightSource = service.ComingBackFlightSource, // Flight Services
                ComingBackFlightDesination = service.ComingBackFlightDesination, // Flight Services
                GoingFlightTime = packageStartDate ?? default, // Use default if null
                ComingFlightTime = packageEndDate ?? default, // Use default if null
                HoltelLocation = service.HoltelLocation, // Hotels Services
                NumberOFAvailableRooms = service.NumberOFAvailableRooms, // Hotels Services
                NumberOFPersons = service.NumberOFPersons // Hotels Services
            };
            return Ok(serviceDTO);
        }


        [HttpGet("{name:alpha}")]
       // [Authorize(Roles = "superAdmin, admin")]

        public IActionResult GetByName(string name)
        {
            Service service = _serviceRepo.GetByName(name);

            if (service == null) return NotFound();

            // Initialize default values for dates
            DateTime? packageStartDate = null;
            DateTime? packageEndDate = null;

            // Retrieve all packagServices  rows associated with this service
            var packageServices = db.PackageService
                                    .Where(ps => ps.ServiceId == service.Id)
                                    .ToList();

            if (packageServices != null && packageServices.Any())
            {
                // Assuming you want to get the earliest start date and latest end date
                packageStartDate = packageServices.Min(ps => ps.Package.startDate);
                packageEndDate = packageServices.Max(ps => ps.Package.EndDate);
            }

            ServiceDTO serviceDTO = new ServiceDTO
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
                EndDate = service.EndDate,
                Duration = (service.EndDate.Value - service.StartDate.Value).Days + 1, // +1 to include both start and end date
                serviceProviderId = service.serviceProviderId,
                BookingTimeAllowed = service.BookingTimeAllowed,
                GoingFlightDestination = service.GoingFlightDestination, // Flight Services
                GoingFlightSource = service.GoingFlightSource, // Flight Services
                ComingBackFlightSource = service.ComingBackFlightSource, // Flight Services
                ComingBackFlightDesination = service.ComingBackFlightDesination, // Flight Services
                GoingFlightTime = packageStartDate ?? default, // Use default if null
                ComingFlightTime = packageEndDate ?? default, // Use default if null
                HoltelLocation = service.HoltelLocation, // Hotels Services
                NumberOFAvailableRooms = service.NumberOFAvailableRooms, // Hotels Services
                NumberOFPersons = service.NumberOFPersons // Hotels Services
            };

            return Ok(serviceDTO);
        }


        [HttpPost]
        [Authorize(Roles = "superAdmin, admin")]

        public ActionResult AddService(ServiceDTO serviceDTO)
        {
            if (serviceDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            Service service = new Service()
            {
                //Id= serviceDTO.Id,
                Name = serviceDTO.Name,
                Description = serviceDTO.Description,
                Image = serviceDTO.Image,
                QuantityAvailable = serviceDTO.QuantityAvailable,
                StartDate = serviceDTO.StartDate,
                price = serviceDTO.price,
                isDeleted = serviceDTO.isDeleted,
                EndDate = serviceDTO.EndDate,
                Duration = (serviceDTO.EndDate.Value - serviceDTO.StartDate.Value).Days + 1, // +1 to include both start and end date
                categoryId = serviceDTO.categoryId,
                serviceProviderId = serviceDTO.serviceProviderId,
                BookingTimeAllowed = serviceDTO.BookingTimeAllowed,
                GoingFlightDestination = serviceDTO.GoingFlightDestination,// Flight Services 
                GoingFlightSource = serviceDTO.GoingFlightSource,//// Flight Services
                ComingBackFlightSource = serviceDTO.ComingBackFlightSource,// Flight Services
                ComingBackFlightDesination = serviceDTO.ComingBackFlightDesination,// Flight Services
                HoltelLocation = serviceDTO.HoltelLocation,// Hotels Services
                NumberOFAvailableRooms = serviceDTO.NumberOFAvailableRooms, // Hotels Services
                NumberOFPersons = serviceDTO.NumberOFPersons,// Hotels Services
            };
            serviceRepo.Add(service);
            serviceRepo.Save();
            return Ok(serviceDTO);

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "superAdmin, admin")]


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
                EndDate = serviceDTO.EndDate,
                Duration = serviceDTO.Duration,
                isDeleted = serviceDTO.isDeleted,
                categoryId = serviceDTO.categoryId,
                serviceProviderId = serviceDTO.serviceProviderId,
                BookingTimeAllowed = serviceDTO.BookingTimeAllowed,
                GoingFlightDestination = serviceDTO.GoingFlightDestination,// Flight Services 
                GoingFlightSource = serviceDTO.GoingFlightSource,//// Flight Services
                ComingBackFlightSource = serviceDTO.ComingBackFlightSource,// Flight Services
                ComingBackFlightDesination = serviceDTO.ComingBackFlightDesination,// Flight Services
                HoltelLocation = serviceDTO.HoltelLocation,// Hotels Services
                NumberOFAvailableRooms = serviceDTO.NumberOFAvailableRooms, // Hotels Services
                NumberOFPersons = serviceDTO.NumberOFPersons,// Hotels Services
            };
            serviceRepo.Edit(service);
            serviceRepo.Save();
            return NoContent();

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "superAdmin, admin")]

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
