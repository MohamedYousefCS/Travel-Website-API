using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.Repositories;
using Travel_Website_System_API_.DTO;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;
using Microsoft.Extensions.DependencyInjection;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            List<ServiceProvider> serviceProviders = ServProviderRepo.GetAll();

            List<ServiceProviderDTO> serviceProvidDTOs = new List<ServiceProviderDTO>();
            
            foreach (ServiceProvider serviceProvider in serviceProviders)
            {
                serviceProvidDTOs.Add(new ServiceProviderDTO()
                {
                        Id = serviceProvider.Id,
                        Name = serviceProvider.Name,
                        Description = serviceProvider.Description,
                        Logo = serviceProvider.Logo,
                        isDeleted=serviceProvider.isDeleted,
                        Services=serviceProvider.Services.Select(n=>n.Name).ToList()
                });
            }
            return Ok(serviceProvidDTOs);
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



        //[HttpGet("/api/SPS/{name}")]

        //[HttpGet("{name:alpha}")]

        //public ActionResult GetByName(string name) {

        //    ServiceProvider SP = db.ServiceProviders.FirstOrDefault(x=>x.Name == name);
        //    if (SP == null) return NotFound();
        //    else return Ok(SP);

        //}



        //Add ServiceProvider

        [HttpPost]

        public ActionResult AddServiceProvider(ServiceProvider serviceProvider)
        {
            if (serviceProvider == null) return BadRequest("ServiceProvider is Null");
            if (!ModelState.IsValid) return BadRequest("Please Enter Vaild Data");
            db.ServiceProviders.Add(serviceProvider);
            db.SaveChanges();
            // return Created("ServiceProvider is Added",serviceProvider);
            return CreatedAtAction("GetById", new {id=serviceProvider.Id }, serviceProvider);


        }

        //Update ServiceProvider

        [HttpPut("{id}")]
        public ActionResult EditServiceProvider(ServiceProvider SP,int id) {
            if (SP == null) return BadRequest();
            if (SP.Id != id) return BadRequest();
            db.Entry(SP).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            //db.ServiceProviders.Update(SP); the same   or db.update(SP); update in two Models
            db.SaveChanges();
            return NoContent();
        
        }

        //Remove ServiceProvider

        [HttpDelete]
        public ActionResult DeleteServiceProvider(int id)
        {
            ServiceProvider SP = db.ServiceProviders.Find(id);
            if (SP == null) return NotFound();
            db.ServiceProviders.Remove(SP);
            db.SaveChanges();
            return Ok(SP);

        }




    }
}
