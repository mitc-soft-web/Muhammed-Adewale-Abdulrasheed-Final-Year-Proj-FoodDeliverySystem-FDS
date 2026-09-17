using FoodDeliverySystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliverySystem.SeedData
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");

            if (!admins.Any())
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@fds.com",
                    Email = "admin@fds.com",
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Admin@123");

                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
