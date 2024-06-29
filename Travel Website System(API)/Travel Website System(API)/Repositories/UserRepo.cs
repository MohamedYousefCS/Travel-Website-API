using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class UserRepo
    {

        private readonly ApplicationDBContext _db;

        public UserRepo(ApplicationDBContext db) 
        {
            _db = db;
        }

        public new List<ApplicationUser> GetAll()
        {
            // Exclude soft-deleted users
            return _db.Users.Where(u => !u.IsDeleted).ToList();
        }

        public new ApplicationUser GetById(string id)
        {
            var user = _db.Users.Find(id);
            // Return null if the user is soft-deleted
            return user?.IsDeleted == true ? null : user;
        }


        public List<ApplicationUser> GetClientsByAdminId(string adminId)
        {
            var clients = _db.BookingPackages
                .Where(bp => bp.package.adminId == adminId && !bp.IsDeleted)
                .Select(bp => bp.client.ApplicationUser)
                .Distinct()
                .ToList();

            return clients.Cast<ApplicationUser>().ToList();
        }


        public List<ApplicationUser> GetAllAdmins()
        {
            return _db.Admins
                      .Include(admin => admin.ApplicationUser)
                      .Select(admin => admin.ApplicationUser)
                      .ToList();
        }


        public List<ApplicationUser> GetAllClients()
        {
            return _db.Clients
                      .Include(c => c.ApplicationUser)
                      .Select(c => c.ApplicationUser)
                      .ToList();
        }


        public List<ApplicationUser> GetAllcustomerServices()
        {
            return _db.CustomerServices
                      .Include(cus => cus.ApplicationUser)
                      .Select(cus => cus.ApplicationUser)
                      .ToList();
        }



        public new void SoftDelete(string id)
        {
            var user = GetById(id);
            if (user != null)
            {
                user.IsDeleted = true;
                Update(user);
            }
        }

        public void Update(ApplicationUser user)
        {
            _db.Entry(user).State = EntityState.Modified;
        }

        public new void Save()
        {
            _db.SaveChanges();
        }

    }
}
