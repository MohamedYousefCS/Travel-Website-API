using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public interface ICategoryRepo
    {

        Category GetByName(string name);

    }
}
