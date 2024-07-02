using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public interface IServiceRepo
    {

        List<string> Search(string searchItem);

        Service GetByName(string name);

    }
}
