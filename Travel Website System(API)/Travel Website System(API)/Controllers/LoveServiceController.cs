using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoveServiceController : ControllerBase
    {

        GenericRepository<LoveService> loveserviceRepo;


        public LoveServiceController(GenericRepository<LoveService> loveserviceRepo)
        {
            this.loveserviceRepo = loveserviceRepo;
        }

        [HttpPost]
        public ActionResult AddLoveService(LoveServiceDTO loveserviceDTO)
        {
            if (loveserviceDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            LoveService loveservice = new LoveService()
            {
                Id =loveserviceDTO.Id,
                date = DateTime.Now,
                IsDeleted = loveserviceDTO.IsDeleted,
                clientId=loveserviceDTO.clientId,
                serviceId=loveserviceDTO.serviceId
            };
            loveserviceRepo.Add(loveservice);
            loveserviceRepo.Save();
            return Ok(loveserviceDTO);

        }



        [HttpDelete]
        public ActionResult DeleteLoveService(int id)
        {

            LoveService loveService = loveserviceRepo.GetById(id);
            if (loveService == null) return NotFound();
            loveserviceRepo.Remove(loveService);
            loveserviceRepo.Save();
            return Ok(loveService);

        }
    }
}
