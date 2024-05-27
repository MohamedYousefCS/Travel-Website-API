using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;
namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        ApplicationDBContext db;

        public ServiceProviderController(ApplicationDBContext db)
        {
            this.db = db;
        }



        //Get All ServiceProviders
        [HttpGet]
        public List<ServiceProvider> GetServiceProviders()
        {
            return db.ServiceProviders.ToList();
        }


        //Get ServiceProvider By Id
        [HttpGet("{id:int}")]

        public ActionResult GetById(int id)
        {
            ServiceProvider SP= db.ServiceProviders.FirstOrDefault(x => x.Id == id);
            if (SP == null)return NotFound();
            else return Ok(SP);
        }


        //Get ServiceProvider By Name

        //[HttpGet("/api/SPS/{name}")]

        [HttpGet("{name:alpha}")]

        public ActionResult GetByName(string name) {

            ServiceProvider SP = db.ServiceProviders.FirstOrDefault(x=>x.Name == name);
            if (SP == null) return NotFound();
            else return Ok(SP);

        }



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
