using Microsoft.AspNetCore.Identity;

namespace LetsMeet.Models
{
    public class IdentitySeedData
    {
        public static void CreateRole(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            CreateRoleAsync(serviceProvider, configuration).Wait();
        }

        public static async Task CreateRoleAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            serviceProvider = serviceProvider.CreateScope().ServiceProvider;

            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string role = "User";

            if (await roleManager.FindByNameAsync(role) == null)
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
