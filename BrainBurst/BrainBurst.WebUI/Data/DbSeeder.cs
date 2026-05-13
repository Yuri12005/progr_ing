using BrainBurst.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BrainBurst.WebUI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // Дістаємо менеджери для роботи з ролями та юзерами
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. СТВОРЮЄМО РОЛІ
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
                }
            }

            // 2. СТВОРЮЄМО ДЕФОЛТНОГО АДМІНА (Якщо його ще немає)
            string adminEmail = "admin@brainburst.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Головний Адміністратор",
                    EmailConfirmed = true, // Одразу підтверджений
                    Points = 9999,
                    Rank = "Legend",
                    CreatedAt = DateTime.UtcNow
                };

                var createPowerUser = await userManager.CreateAsync(newAdmin, "AdminPassword123!");
                if (createPowerUser.Succeeded)
                {
                    // Додаємо його в роль Admin
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}