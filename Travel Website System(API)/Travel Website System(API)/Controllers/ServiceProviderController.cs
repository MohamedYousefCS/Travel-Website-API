using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.Repositories;
using Travel_Website_System_API_.DTO;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

   // [Authorize(Roles = "superAdmin, admin")]

    public class ServiceProviderController : ControllerBase
    {
        GenericRepository<ServiceProvider> ServProviderRepo;
        IServiceProviderRepo repo;


        public ServiceProviderController( GenericRepository<ServiceProvider> servProviderRepo,IServiceProviderRepo repo)
        {
            this.ServProviderRepo = servProviderRepo;
            this.repo = repo;
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

        [HttpGet("{name:alpha}")]

        public ActionResult GetByName(string name)
        {

            ServiceProvider SP = repo.GetByName(name);
            if (SP == null) return NotFound();
            else return Ok(SP);

        }



        //Add ServiceProvider

        [HttpPost]
        [Authorize(Roles = "superAdmin")]


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
        [Authorize(Roles = "superAdmin")]

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "superAdmin")]

        public ActionResult DeleteServiceProvider(int id)
        {
            ServiceProvider SP = ServProviderRepo.GetById(id);

            if (SP == null) return NotFound();

            if (SP.Services.Any())
            {
                return BadRequest("ServiceProvider cannot be deleted because it is referenced by existing Services.");
            }

            try
            {
                ServProviderRepo.Remove(SP);
                ServProviderRepo.Save();
            }
            catch (DbUpdateException ex)
            {
               
                return BadRequest("Can't deleting the ServiceProvider. Please ensure that it is not referenced by any existing records.");
            }

            return Ok(SP);
        }




    }
}
