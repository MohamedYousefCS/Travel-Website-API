using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();


            LoveService loveservice = new LoveService()
            {
                //Id =loveserviceDTO.Id,
                date = DateTime.Now,
                IsDeleted = loveserviceDTO.IsDeleted,
                clientId=userId,
                serviceId=loveserviceDTO.serviceId
            };
            loveserviceRepo.Add(loveservice);
            loveserviceRepo.Save();
            return Ok(loveserviceDTO);

        }



        [HttpDelete("{id}")]
        public ActionResult DeleteLoveService(int id)
        {

            LoveService loveService = loveserviceRepo.GetById(id);
            if (loveService == null) return NotFound();
            loveService.IsDeleted = true;
            loveserviceRepo.Edit(loveService);
            loveserviceRepo.Save();
            return Ok(loveService);

        }

        //User Services

        [HttpGet("user-Services")]
        public ActionResult GetUserLovePackages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var lovePackages = loveserviceRepo.FindWithInclude(
                lp => lp.clientId == userId && !lp.IsDeleted,
                lp => lp.service
            ).ToList();

            if (!lovePackages.Any()) return NotFound("No love Services found for this user.");

            var lovePackageDTOs = lovePackages.Select(lp => new LovePackageDTO
            {
                Id = lp.Id,
                date = lp.date,
                IsDeleted = lp.IsDeleted,
                clientId = lp.clientId,
                packageId = lp.serviceId,
                PackageName = lp.service.Name,
                PackageDescription = lp.service.Description,
                PackagePrice = lp.service.price
            }).ToList();

            return Ok(lovePackageDTOs);
        }
    }
}
