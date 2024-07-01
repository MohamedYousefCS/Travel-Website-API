
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class ServiceRepo : IServiceRepo
    {
        ApplicationDBContext db;


        public ServiceRepo(ApplicationDBContext db) {  this.db = db; }


        public List<string> Search(string searchItem)
        {
            if (string.IsNullOrEmpty(searchItem))
            {
                return new List<string>();
            }

            searchItem = searchItem.ToLower();

            return db.Services
                .Where(x =>
                    x.Id.ToString().ToLower() == searchItem ||
                    (x.Name != null && x.Name.ToLower().Contains(searchItem)) ||
                    x.price.ToString().ToLower().Contains(searchItem))
            .Select(x => x.Name)
                .ToList();
        }




        public Service GetByName(string name)
        {

            return db.Services.FirstOrDefault(x => x.Name == name);
        }



    }
   

    
}
