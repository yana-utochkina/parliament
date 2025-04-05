using Microsoft.AspNetCore.Identity;
using ParliamentDomain.Model;
using ParliamentInfrastructure.Models;

namespace ParliamentInfrastructure
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<DefaultUser> userManager, RoleManager<IdentityRole> roleManager, ParliamentDbContext context)
        {
            string adminEmail = "admin@gmail.com";
            string password = "Qwerty_1";
            if (await roleManager.FindByNameAsync("admin") is null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("worker") is null)
            {
                await roleManager.CreateAsync(new IdentityRole("worker"));
            }
            if (await roleManager.FindByNameAsync("student") is null)
            {
                await roleManager.CreateAsync(new IdentityRole("student"));
            }
            if (await roleManager.FindByNameAsync("guest") is null)
            {
                await roleManager.CreateAsync(new IdentityRole("guest"));
            }
            if (await userManager.FindByNameAsync(adminEmail) is null)
            {
                DefaultUser admin = new DefaultUser { Email = adminEmail, UserName = adminEmail, EmailConfirmed = true };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    User user = new User
                    {
                        Email = adminEmail,
                        FullName = "Administrator", // За потреби додати більше полів
                        University = "Default University",
                        Faculty = "Admin Faculty"
                    };

                    // Додавання користувача до таблиці Users
                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
