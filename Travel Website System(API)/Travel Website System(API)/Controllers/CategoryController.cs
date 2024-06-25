using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[Authorize(Roles = "superAdmin, admin")]

    public class CategoryController : ControllerBase
    {

        ApplicationDBContext db;

        public CategoryController(ApplicationDBContext db)
        {
            this.db = db;
        }

        [HttpGet]

        public ActionResult GetAllCategory() {
        
            List<Category> categories = db.Categories.ToList();

            List<CategoryDTO> categoryDTOs = new List<CategoryDTO>();

            foreach (var category in categories)
            {
                categoryDTOs.Add(new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsDeleted = category.IsDeleted,
                    Services = category.Services,
                });
                
            }

            return Ok(categoryDTOs);
        }

        [HttpGet("{id:int}")]

        public ActionResult GetCategoryById(int id) {

            Category category = db.Categories.Find(id);
            if(category == null) return NotFound();
            else return Ok(category);
        }

        [HttpGet("{name:alpha}")]
        public ActionResult GetCategoryByName(string name)
        {
            Category category = db.Categories.FirstOrDefault(c => c.Name == name);
            if(category == null) return NotFound();
            return Ok(category);
        }


        [HttpPost]

        public ActionResult AddCategory(Category category) {
            if(category == null) return BadRequest();
            if(!ModelState.IsValid) return BadRequest();
            db.Categories.Add(category);
            db.SaveChanges();
            return CreatedAtAction("GetCategoryById", new {id=category.Id},category);
        
        }

        [HttpPut("{id}")]

        public ActionResult EditCategory(Category category,int id) { 
            if (category == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            if(category.Id != id) return BadRequest();
            db.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            return NoContent();
            
        }

        [HttpDelete]

        public ActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if(category == null) return NotFound();
            db.Categories.Remove(category);
            db.SaveChanges();
            return Ok(category);
        }


    }
}
