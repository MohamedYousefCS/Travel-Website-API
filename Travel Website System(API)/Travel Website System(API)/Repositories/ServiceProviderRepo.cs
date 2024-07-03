
using Travel_Website_System_API.Models;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;

namespace Travel_Website_System_API_.Repositories
{
    public class ServiceProviderRepo : IServiceProviderRepo
    {
        ApplicationDBContext db;
        public ServiceProviderRepo(ApplicationDBContext db) { 
        this.db = db;
         
        }
        public ServiceProvider GetByName(string name)
        {
            return db.ServiceProviders.FirstOrDefault(x => x.Name == name);
        }
    }
}
