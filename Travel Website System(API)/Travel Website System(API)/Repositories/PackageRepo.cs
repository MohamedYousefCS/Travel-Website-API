using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class PackageRepo:IPackageRepo
    {


        ApplicationDBContext db;


        public PackageRepo(ApplicationDBContext db) { this.db = db; }


        public List<string> Search(string searchItem)
        {
            if (string.IsNullOrEmpty(searchItem))
            {
                return new List<string>();
            }

            searchItem = searchItem.ToLower();

            return db.Packages
                .Where(x =>
                    x.Id.ToString().ToLower() == searchItem ||
                    (x.Name != null && x.Name.ToLower().Contains(searchItem)) ||
                    x.Price.ToString().ToLower().Contains(searchItem))
            .Select(x => x.Name)
                .ToList();
        }
       

        public Package GetByName(string name)
        {

            return db.Packages.FirstOrDefault(x => x.Name == name);
        }
    }
}
