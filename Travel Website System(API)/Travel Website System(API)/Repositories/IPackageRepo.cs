using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public interface IPackageRepo
    {

        List<string> Search(string searchItem);

        Package GetByName(string name);

    }
}
