using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

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
        public ActionResult AddLoveService(LovePackageDTO lovePackageDTO)
        {
            if (lovePackageDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            LovePackage lovePackage = new LovePackage()
            {
                Id = lovePackageDTO.Id,
                date = DateTime.Now,
                IsDeleted = lovePackageDTO.IsDeleted,
                clientId = lovePackageDTO.clientId,
                packageId = lovePackageDTO.packageId
            };
            lovePackageRepo.Add(lovePackage);
            lovePackageRepo.Save();
            return Ok(lovePackageDTO);

        }



        [HttpDelete]
        public ActionResult DeleteLoveService(int id)
        {

            LovePackage lovePackage = lovePackageRepo.GetById(id);
            if (lovePackage == null) return NotFound();
            lovePackageRepo.Remove(lovePackage);
            lovePackageRepo.Save();
            return Ok(lovePackage);

        }

    }
}
