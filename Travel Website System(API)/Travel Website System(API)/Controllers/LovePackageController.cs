using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LovePackageController : ControllerBase
    {
        GenericRepository<LovePackage> lovePackageRepo;

        public LovePackageController(GenericRepository<LovePackage> lovePackageRepo)
        {
            this.lovePackageRepo = lovePackageRepo;
        }

        [HttpPost]
        [Authorize(Roles = "client")]

        public ActionResult AddLovePackage(LovePackageDTO lovePackageDTO)
        {
            if (lovePackageDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();


            LovePackage lovePackage = new LovePackage()
            {
                //Id = lovePackageDTO.Id,
                date = DateTime.Now,
                IsDeleted = false,
                clientId = userId,
                packageId = lovePackageDTO.packageId
            };
            lovePackageRepo.Add(lovePackage);
            lovePackageRepo.Save();
            return Ok(lovePackageDTO);

        }



        [HttpDelete("{id}")]
        public ActionResult DeleteLovePackage(int id)
        {

            LovePackage lovePackage = lovePackageRepo.GetById(id);
            if (lovePackage == null) return NotFound();
            lovePackage.IsDeleted=true;
            lovePackageRepo.Edit(lovePackage);
            lovePackageRepo.Save();
            return Ok(lovePackage);

        }



        [HttpGet("user-packages")]
        public ActionResult GetUserLovePackages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var lovePackages = lovePackageRepo.FindWithInclude(
                lp => lp.clientId == userId && !lp.IsDeleted,
                lp => lp.package
            ).ToList();

            if (!lovePackages.Any()) return NotFound("No love packages found for this user.");

            var lovePackageDTOs = lovePackages.Select(lp => new LovePackageDTO
            {
                Id = lp.Id,
                date = lp.date,
                IsDeleted = lp.IsDeleted,
                clientId = lp.clientId,
                packageId = lp.packageId,
                PackageName = lp.package.Name, 
                PackageDescription = lp.package.Description,
                PackagePrice = lp.package.Price
            }).ToList();

            return Ok(lovePackageDTOs);
        }






    }
}
