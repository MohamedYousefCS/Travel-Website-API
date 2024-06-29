using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.Repositories;
using Travel_Website_System_API_.DTO;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

   // [Authorize(Roles = "superAdmin, admin")]

    public class ServiceProviderController : ControllerBase
    {
        ApplicationDBContext db;
        GenericRepository<ServiceProvider> ServProviderRepo;


        public ServiceProviderController(ApplicationDBContext db, GenericRepository<ServiceProvider> servProviderRepo)
        {
            this.db = db;
            this.ServProviderRepo = servProviderRepo;
        }



        //Get All ServiceProviders
        [HttpGet]
        public ActionResult GetServiceProviders()
        {
            // Fetch all service providers from the repository
            List<ServiceProvider> serviceProviders = ServProviderRepo.GetAll();

            // Create a list to hold the service provider DTOs
            List<ServiceProviderDTO> serviceProviderDTOs = new List<ServiceProviderDTO>();

            // Loop through each service provider and map it to the DTO
            foreach (ServiceProvider serviceProvider in serviceProviders)
            {
                serviceProviderDTOs.Add(new ServiceProviderDTO()
                {
                    Id = serviceProvider.Id,
                    Name = serviceProvider.Name,
                    Description = serviceProvider.Description,
                    Logo = serviceProvider.Logo,
                    isDeleted = serviceProvider.isDeleted,
                    Services = serviceProvider.Services.Select(n => n.Name).ToList()
                });
            }

            // Return the list of DTOs with an OK response
            return Ok(serviceProviderDTOs);
        }

      
        //Get ServiceProvider By Id
        [HttpGet("{id:int}")]

        public ActionResult GetById(int id)
        {
            ServiceProvider serviceProvider = ServProviderRepo.GetById(id);
            if (serviceProvider == null)return NotFound();
            else
            {
                ServiceProviderDTO serviceProviderDTO = new ServiceProviderDTO() {

                    Id = serviceProvider.Id,
                    Name = serviceProvider.Name,
                    Description = serviceProvider.Description,
                    Logo = serviceProvider.Logo,
                    isDeleted = serviceProvider.isDeleted,
                    Services = serviceProvider.Services.Select(n => n.Name).ToList()
                };
                return Ok(serviceProviderDTO);
            }
        }



        [HttpGet("/api/SPS/{name}")]

        [HttpGet("{name:alpha}")]

        public ActionResult GetByName(string name)
        {

            ServiceProvider SP = db.ServiceProviders.FirstOrDefault(x => x.Name == name);
            if (SP == null) return NotFound();
            else return Ok(SP);

        }



        //Add ServiceProvider

        [HttpPost]

        public ActionResult AddServiceProvider(ServiceProviderDTO serviceProviderDTO)
        {
            if (serviceProviderDTO == null) return BadRequest("ServiceProvider is Null");
            if (!ModelState.IsValid) return BadRequest("Please Enter Vaild Data");
            ServiceProvider serviceProvider = new ServiceProvider()
            {
                //Id = serviceProviderDTO.Id,
                Name= serviceProviderDTO.Name,
                Description = serviceProviderDTO.Description,
                Logo = serviceProviderDTO.Logo,
                isDeleted = serviceProviderDTO.isDeleted,
            };
            ServProviderRepo.Add(serviceProvider);
            ServProviderRepo.Save();
            return CreatedAtAction("GetById", new {id=serviceProvider.Id }, serviceProvider);


        }

        //Update ServiceProvider

        [HttpPut("{id}")]
        public ActionResult EditServiceProvider(ServiceProviderDTO serviceProviderDTO, int id) {
            if (serviceProviderDTO == null) return BadRequest();
            if (serviceProviderDTO.Id != id) return BadRequest();
            ServiceProvider serviceProvider = new ServiceProvider()
            {
                Id = serviceProviderDTO.Id,
                Name = serviceProviderDTO.Name,
                Description = serviceProviderDTO.Description,
                Logo = serviceProviderDTO.Logo,
                isDeleted = serviceProviderDTO.isDeleted,
            };
            ServProviderRepo.Edit(serviceProvider);
            ServProviderRepo.Save();
            return NoContent();
        
        }

        //Remove ServiceProvider

        [HttpDelete]
        public ActionResult DeleteServiceProvider(int id)
        {
            ServiceProvider SP = ServProviderRepo.GetById(id);
            if (SP == null) return NotFound();
            ServProviderRepo.Remove(SP);
            ServProviderRepo.Save();
            return Ok(SP);

        }




    }
}
