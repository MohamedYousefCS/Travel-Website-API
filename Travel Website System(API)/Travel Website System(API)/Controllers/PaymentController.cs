using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.UnitWork;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        UnitOFWork unitOFWork;

        public PaymentController(UnitOFWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }
        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            Payment pay = unitOFWork.PaymentRepo.GetById(id);
            return Ok(pay);
        }

    }
}
            

