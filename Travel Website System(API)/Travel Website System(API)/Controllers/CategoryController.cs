using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO;
using Travel_Website_System_API_.Repositories;

namespace Travel_Website_System_API_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[Authorize(Roles = "superAdmin, admin")]

    public class CategoryController : ControllerBase
    {


        GenericRepository<Category> categortRepo;
        ICategoryRepo repo;


        public CategoryController(GenericRepository<Category> CategoryRepo, ICategoryRepo repo)
        {
            this.categortRepo = CategoryRepo;
            this.repo = repo;
        }

        [HttpGet]

        public ActionResult GetAllCategory()
        {
            List<Category> categories = categortRepo.GetAll();

            List<CategoryDTO> categoryDTOs = new List<CategoryDTO>();

            foreach (var category in categories)
            {
                categoryDTOs.Add(new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsDeleted = category.IsDeleted,
                    ServiceNames = category.Services.Select(s => s.Name).ToList() // Add this line
                });
            }

            return Ok(categoryDTOs);
        }

        [HttpGet("{id:int}")]

        public ActionResult GetCategoryById(int id) {

            Category category = categortRepo.GetById(id);
            if(category == null) return NotFound();
            else
            {
                CategoryDTO categoryDTO = new CategoryDTO()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsDeleted = category.IsDeleted,
                    ServiceNames = category.Services.Select(s => s.Name).ToList() // Add this line

                };
                return Ok(categoryDTO);

            }
        }

        [HttpGet("{name:alpha}")]
        public ActionResult GetCategoryByName(string name)
        {
            Category category =repo.GetByName(name);
            if(category == null) return NotFound();
            return Ok(category);
        }


        [HttpPost]

        public ActionResult AddCategory(CategoryDTO categoryDTO) {
            if(categoryDTO == null) return BadRequest();
            if(!ModelState.IsValid) return BadRequest();
            Category category = new Category() {
            Name= categoryDTO.Name,
            Description= categoryDTO.Description,
            IsDeleted=categoryDTO.IsDeleted,
            };
            categortRepo.Add(category);
            categortRepo.Save();
          
            return CreatedAtAction("GetCategoryById", new {id=category.Id},category);
        
        }

        [HttpPut("{id}")]

        public ActionResult EditCategory(CategoryDTO categoryDTO,int id) { 
            if (categoryDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            if(categoryDTO.Id != id) return BadRequest();
            Category category = new Category()
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                IsDeleted = categoryDTO.IsDeleted,

            };
            categortRepo.Edit(category);
            categortRepo.Save();
            return NoContent();
            
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)
        {
            Category category = categortRepo.GetById(id);

            if (category == null) return NotFound();

            if (category.Services.Any())
            {
                
                return BadRequest("Category cannot be deleted because it is referenced by existing Services.");
            }

            try
            {
                categortRepo.Remove(category);
                categortRepo.Save();
            }
            catch (DbUpdateException ex)
            {
               
                return BadRequest("Can't deleting the Category. Please ensure that it is not referenced by any existing records.");
            }

            return Ok(category);
        }



    }
}
