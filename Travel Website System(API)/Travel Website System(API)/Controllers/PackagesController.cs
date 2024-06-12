using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.Repositories;
using Travel_Website_System_API_.DTO;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        GenericRepository<Package> packageRepo;


        public PackagesController( GenericRepository<Package> packageRepo)
        {
            this.packageRepo = packageRepo;
        }

        // GET: api/Packages
        [HttpGet]
        public ActionResult GetPackages()
        {
          List<Package>packages= packageRepo.GetAll();
            List<PackageDTO> packageDTOs = new List<PackageDTO>();
            foreach (Package package in packages)
            {
                packageDTOs.Add(new PackageDTO
                {
                    Id = package.Id,
                    Name = package.Name,
                    Description = package.Description,
                    Image = package.Image,
                    QuantityAvailable = package.QuantityAvailable,
                    Price = package.Price,
                    isDeleted = package.isDeleted,
                    startDate = package.startDate,
                    Duration = package.Duration,
                    adminId = package.adminId
                });

            }
            return Ok(packageDTOs);

        }

        // GET: api/Packages/5
        [HttpGet("{id}")]
        public ActionResult GetPackageById(int id)
        {
            var package = packageRepo.GetById(id);

            if (package == null) return NotFound();
            else
            {
                PackageDTO packageDTO = new PackageDTO() {
                    Id = package.Id,
                    Name = package.Name,
                    Description = package.Description,
                    Image = package.Image,
                    QuantityAvailable = package.QuantityAvailable,
                    Price = package.Price,
                    isDeleted = package.isDeleted,
                    startDate = package.startDate,
                    Duration = package.Duration,
                    adminId = package.adminId
                };
                return Ok(packageDTO);
            }
        }


        // POST: api/Packages
        [HttpPost]
        public ActionResult AddPackage(PackageDTO packageDTO)
        {
            if (packageDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            Package package = new Package() {
                Id = packageDTO.Id,
                Name = packageDTO.Name,
                Description = packageDTO.Description,
                Image = packageDTO.Image,
                QuantityAvailable = packageDTO.QuantityAvailable,
                Price = packageDTO.Price,
                isDeleted = packageDTO.isDeleted,
                startDate = packageDTO.startDate,
                Duration = packageDTO.Duration,
                adminId = packageDTO.adminId
            };
            packageRepo.Add(package);
            packageRepo.Save();
            // return Ok(packageDTO);
            return CreatedAtAction("GetPackageById", new { id = package.Id }, packageDTO);

        }


        // PUT: api/Packages/5
        [HttpPut("{id}")]
        public ActionResult EditPackage(int id, PackageDTO packageDTO)
        {
            if (packageDTO == null) return BadRequest();
            if (packageDTO.Id != id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            Package package = new Package()
            {
                Id = packageDTO.Id,
                Name = packageDTO.Name,
                Description = packageDTO.Description,
                Image = packageDTO.Image,
                QuantityAvailable = packageDTO.QuantityAvailable,
                Price = packageDTO.Price,
                isDeleted = packageDTO.isDeleted,
                startDate = packageDTO.startDate,
                Duration = packageDTO.Duration,
                adminId = packageDTO.adminId
            };
            packageRepo.Edit(package);
            packageRepo.Save();
            return NoContent();
        }

        
        // DELETE: api/Packages/5
        [HttpDelete("{id}")]
        public IActionResult DeletePackage(int id)
        {
            Package package = packageRepo.GetById(id);
            if (package == null) return NotFound();
            packageRepo.Remove(package);
            packageRepo.Save();
            return Ok(package);
        }

    }
}
