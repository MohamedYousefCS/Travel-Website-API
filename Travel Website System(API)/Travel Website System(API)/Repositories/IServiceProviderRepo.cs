using Travel_Website_System_API.Models;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;

namespace Travel_Website_System_API_.Repositories
{
    public interface IServiceProviderRepo
    {

        ServiceProvider GetByName(string name);

    }
}
