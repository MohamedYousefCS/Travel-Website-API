using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        ApplicationDBContext db;

        public UserController(ApplicationDBContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public List<User> GetAll()
        {
            return db.Users.ToList();
        }
    }
}
