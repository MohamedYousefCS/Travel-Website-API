using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Models;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageServiceController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public PackageServiceController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST api/packages/addservices
        [HttpPost("addservices")]
        public async Task<ActionResult> AddServicesToPackage(int packageId, List<int> serviceIds)
        {
            var package = await _context.Packages
                .Include(p => p.PackageServices)
                .FirstOrDefaultAsync(p => p.Id == packageId);

            if (package == null)
            {
                return NotFound();
            }

            var addedServices = new List<int>();
            var duplicateServices = new List<int>();

            foreach (var serviceId in serviceIds)
            {
                var service = await _context.Services.FindAsync(serviceId);

                if (service == null)
                {
                    return BadRequest($"Service with ID {serviceId} not found.");
                }

                // Check if the service is already associated with the package
                if (package.PackageServices.Any(ps => ps.ServiceId == serviceId))
                {
                    // Service already exists in the package
                    duplicateServices.Add(serviceId); // Track duplicate service IDs
                }
                else
                {
                    // Add the service to the package
                    package.PackageServices.Add(new PackageService
                    {
                        ServiceId = serviceId,
                        AddedOn = DateTime.UtcNow
                    });

                    addedServices.Add(serviceId); // Track added service IDs
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle specific database update errors
                return StatusCode(500, new { message = "Error saving changes to the database.", error = ex.Message });
            }

            var responseMessage = "Services added to package successfully";

            if (duplicateServices.Any())
            {
                responseMessage += $", but services with IDs [{string.Join(", ", duplicateServices)}] were already present.";
            }

            return Ok(new { message = responseMessage, addedServices, duplicateServices });
        }

        // GET api/packages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageDTO>>> GetPackages()
        {
            var packages = await _context.Packages
                .Include(p => p.PackageServices)
                    .ThenInclude(ps => ps.Service)
                .ToListAsync();

            var packageDTOs = packages.Select(p => new PackageDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.Image,
                QuantityAvailable = p.QuantityAvailable,
                Price = p.Price,
                isDeleted = p.isDeleted,
                startDate = p.startDate,
                Duration = p.Duration,
                adminId = p.adminId,
                ServiceNames = p.PackageServices.Select(ps => ps.Service.Name).ToList()
            }).ToList();

            return Ok(packageDTOs);
        }
   



}
}
