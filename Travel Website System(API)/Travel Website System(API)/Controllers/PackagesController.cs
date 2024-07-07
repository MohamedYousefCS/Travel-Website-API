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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        GenericRepository<Package> packageRepo;
        private readonly IPackageRepo _packageRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        IBookingPackageRepo bookingPackageRepo;


        public PackagesController(GenericRepository<Package> packageRepo,IPackageRepo repo , IWebHostEnvironment webHostEnvironment,IBookingPackageRepo bookingPackageRepo)
        {
            this.packageRepo = packageRepo;
            this._packageRepo = repo;
            _webHostEnvironment = webHostEnvironment;
            this.bookingPackageRepo = bookingPackageRepo;

        }

        // GET: api/Packages
        [Authorize(Roles = "superAdmin, admin")]

        [HttpGet]
        public ActionResult GetPackages(int pageNumber = 1, int pageSize = 10)
        {
            List<Package> packages = packageRepo.GetAllWithPagination(pageNumber, pageSize);
            int totalPackages = packageRepo.GetTotalCount();

            List<PackageDTO> packageDTOs = new List<PackageDTO>();

            foreach (Package package in packages)
            {
                var serviceNames = package.PackageServices.Select(ps => ps.Service.Name).ToList();

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
                    EndDate = package.EndDate,
                    adminId = package.adminId,
                    BookingTimeAllowed = package.BookingTimeAllowed,
                  ServiceNames = serviceNames ,// Include service names
                    FirstLocation = package.FirstLocation,
                    SecondLocation = package.SecondLocation,
                    FirstLocationDuration = package.FirstLocationDuration,
                    SecondLocationDuration = package.SecondLocationDuration,
                });
            }

            var response = new PaginatedResponse<PackageDTO>
            {
                TotalCount = totalPackages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = packageDTOs
            };

            return Ok(response);
        }

        [HttpGet("Search/{searchItem}")]
        public ActionResult<List<string>> Search(string searchItem)
        {
            var serviceNames = _packageRepo.Search(searchItem);
            return Ok(serviceNames);
        }

        [Authorize(Roles = "superAdmin, admin")]

        [HttpGet("{name:alpha}")]
        public ActionResult GetPackageByName(string name)
        {
            var package = _packageRepo.GetByName(name);

            if (package == null) return NotFound();
            else
            {
                // Ensure that related services are loaded
                var serviceNames = package.PackageServices.Select(ps => ps.Service.Name).ToList();

                PackageDTO packageDTO = new PackageDTO()
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
                    adminId = package.adminId,
                    BookingTimeAllowed = package.BookingTimeAllowed,
                    ServiceNames = serviceNames ,// Include service names
                    FirstLocation = package.FirstLocation,
                    SecondLocation = package.SecondLocation,
                    FirstLocationDuration = package.FirstLocationDuration,
                    SecondLocationDuration = package.SecondLocationDuration,

                };
                return Ok(packageDTO);
            }
        }

        // GET: api/Packages/5
        [Authorize(Roles = "superAdmin, admin")]
        [HttpGet("{id}")]
        public ActionResult GetPackageById(int id)
        {
            var package = packageRepo.GetById(id);

            if (package == null) return NotFound();
            else
            {
                // Ensure that related services are loaded
               var serviceNames = package.PackageServices.Select(ps => ps.Service.Name).ToList();

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
                    EndDate = package.EndDate,
                    adminId = package.adminId,
                    BookingTimeAllowed= package.BookingTimeAllowed,
                   ServiceNames = serviceNames, // Include service names
                    FirstLocation = package.FirstLocation,
                    SecondLocation = package.SecondLocation,
                    FirstLocationDuration = package.FirstLocationDuration,
                    SecondLocationDuration = package.SecondLocationDuration,
                };
                return Ok(packageDTO);
            }
        }


        // POST: api/Packages
        [Authorize(Roles = "superAdmin, admin")]
        [HttpPost]
        public ActionResult AddPackage(PackageDTO packageDTO)
        {
            if (packageDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();

            // Get the current logged-in user's ID
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminId == null)
            {
                return Unauthorized();
            }

            //string uniqueFileName = UploadImage(packageDTO.Image);
            packageDTO.EndDate = packageDTO.startDate?.AddDays(packageDTO.Duration ?? 0);
            packageDTO.SecondLocationDuration = packageDTO.Duration - packageDTO.FirstLocationDuration;

            Package package = new Package() {
                //Id = packageDTO.Id,
                Name = packageDTO.Name,
                Description = packageDTO.Description,
                //Image = uniqueFileName,
                Image= packageDTO.Image,
                QuantityAvailable = packageDTO.QuantityAvailable,
                Price = packageDTO.Price,
                isDeleted = packageDTO.isDeleted,
                startDate = packageDTO.startDate,
                Duration = packageDTO.Duration,
                EndDate = packageDTO.EndDate,
                adminId = adminId,
                BookingTimeAllowed = packageDTO.BookingTimeAllowed,
                FirstLocation = packageDTO.FirstLocation,
                SecondLocation = packageDTO.SecondLocation,
                FirstLocationDuration = packageDTO.FirstLocationDuration,
                SecondLocationDuration= packageDTO.SecondLocationDuration,
            };
            packageRepo.Add(package);
            packageDTO.Id = package.Id;
            packageRepo.Save();
            // return Ok(packageDTO);
            return CreatedAtAction("GetPackageById", new { id = package.Id }, packageDTO);

        }


        //implement image any problem call helmy 
        private string UploadImage(IFormFile file)
        {

            if (file != null && file.Length > 0)
            {

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/packages");


                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique filename for the uploaded file
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                // Combine the uploads folder path with the unique filename
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the specified path
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Return the unique filename
                return uniqueFileName;
            }

            // If file is null or has no content, return null
            return null;
        }



        // PUT: api/Packages/5
        [Authorize(Roles = "superAdmin, admin")]
        [HttpPut("{id}")]
        public ActionResult EditPackage(int id, [FromBody] PackageDTO packageDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get current user's ID
            var package = packageRepo.GetById(id);

            if (package == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is authorized to edit this package
            if (userId != package.adminId)
            {
                return BadRequest("You are not authorized to edit this package");
            }

            // Proceed with package edit logic
            if (packageDTO == null || packageDTO.Id != id || !ModelState.IsValid)
            {
                return BadRequest();
            }

           // string uniqueFileName = UploadImage(packageDTO.Image);
            package.Name = packageDTO.Name;
            package.Description = packageDTO.Description;
            //package.Image = uniqueFileName;
            package.Image=packageDTO.Image;
            package.QuantityAvailable = packageDTO.QuantityAvailable;
            package.Price = packageDTO.Price;
            package.isDeleted = packageDTO.isDeleted;
            package.startDate = packageDTO.startDate;
            package.Duration = packageDTO.Duration;
            package.EndDate = packageDTO.EndDate;
            package.BookingTimeAllowed = packageDTO.BookingTimeAllowed;
            package.FirstLocation = package.FirstLocation;
            package.SecondLocation = package.SecondLocation;
            package.FirstLocationDuration = package.FirstLocationDuration;
            package.SecondLocationDuration = package.SecondLocationDuration;

            packageRepo.Edit(package);
            packageRepo.Save();

            return NoContent();
        }

        // DELETE: api/Packages/5
        [Authorize(Roles = "superAdmin, admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletePackage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get current user's ID
            var package = packageRepo.GetById(id);

            if (package == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is authorized to delete this package
            if (userId != package.adminId)
            {
               return BadRequest("You are not authorized to delete this package");
            }

            // Check if there are any bookings associated with the package
            var hasBookings = bookingPackageRepo.GetAllBokking(id);
            if (hasBookings)
            {
                return BadRequest("The package cannot be deleted because it is reserved.");
            }

            // Perform a soft delete by marking the package as deleted
            package.isDeleted = true;
            packageRepo.Edit(package);
            packageRepo.Save();

            return Ok(package);
        }








    }
}
