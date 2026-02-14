using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Data
{
    
public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            
            string[] roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "AdminPassword123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
           
            var normalEmail = "user@example.com";
            var normalUser = await userManager.FindByEmailAsync(normalEmail);
            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = normalEmail,
                    Email = normalEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(normalUser, "UserPassword123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }

}

