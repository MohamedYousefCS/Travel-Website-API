using Microsoft.AspNetCore.Identity;

namespace Travel_Website_System_API_
{
    public class DataSeed
    {

        private readonly RoleManager<IdentityRole> roleManager;

        public DataSeed(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            // Check if the role exists
            if (!await roleManager.RoleExistsAsync("client"))
            {
                // If the role doesn't exist, create it
                var role = new IdentityRole("client");
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("admin"))
            {
                // If the role doesn't exist, create it
                var role = new IdentityRole("admin");
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("customerService"))
            {
                // If the role doesn't exist, create it
                var role = new IdentityRole("customerService");
                await roleManager.CreateAsync(role);
            }
        
            if (!await roleManager.RoleExistsAsync("superAdmin"))
            {
                // If the role doesn't exist, create it
                var role = new IdentityRole("superAdmin");
                await roleManager.CreateAsync(role);
            }
            // Add more roles if needed
        }
    }
}
