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
        

        public ServiceController(GenericRepository<Service> serviceRepo)
        {
            this.serviceRepo = serviceRepo;
        }

        [HttpGet]
        //[Produces("application/json")]
        public ActionResult GetAllServices(int pageNumber = 1, int pageSize = 10)
        {
            List<Service> services = serviceRepo.GetAllWithPaginiation(pageNumber, pageSize);
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


        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            Service service = serviceRepo.GetById(id);
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
                categoryId=service.categoryId,
                serviceProviderId=service.serviceProviderId,
                };
                return Ok(serviceDTO);

            }
           
        }

        [HttpPost]
        public ActionResult AddService(ServiceDTO serviceDTO)
        {
            if (serviceDTO == null) return BadRequest();
            if(!ModelState.IsValid) return BadRequest();
            Service service=new Service() { 
            Id= serviceDTO.Id,
            Name = serviceDTO.Name,
            Description = serviceDTO.Description,
            Image= serviceDTO.Image,
            QuantityAvailable = serviceDTO.QuantityAvailable,
            StartDate = serviceDTO.StartDate,
            price=serviceDTO.price,
            isDeleted = serviceDTO.isDeleted,
            categoryId=serviceDTO.categoryId,
            serviceProviderId= serviceDTO.serviceProviderId
            };
            serviceRepo.Add(service);
            serviceRepo.Save();
            return Ok(serviceDTO);

        }

        [HttpPut("{id}")]

        public ActionResult EditService(ServiceDTO serviceDTO,int id)
        {
            if(serviceDTO == null) return BadRequest();
            if(serviceDTO.Id !=id) return BadRequest();
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
                serviceProviderId = serviceDTO.serviceProviderId
            };
            serviceRepo.Edit(service);
            serviceRepo.Save();
            return NoContent();

        }

        [HttpDelete]
        public ActionResult DeleteService(int id) {
        
         Service service = serviceRepo.GetById(id);
            if (service == null) return NotFound();
            serviceRepo.Remove(service); 
            serviceRepo.Save();
            return Ok(service);
        
        }


    }
}
