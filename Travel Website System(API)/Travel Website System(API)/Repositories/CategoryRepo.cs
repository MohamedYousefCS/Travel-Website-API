using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class CategoryRepo : ICategoryRepo
    {
        ApplicationDBContext db;
        public CategoryRepo(ApplicationDBContext db) {
        this.db = db;
        }
        public Category GetByName(string name)
        {
            return db.Categories.FirstOrDefault(c => c.Name == name);
        }
    }
}
