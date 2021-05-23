using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TestIdentityServer.Model;

namespace TestIdentityServer.Infra
{
    public class IdentitySeedData
    {
        public static void CreateIdentitySeedData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                MigrateDatabaseAsync(serviceScope).GetAwaiter().GetResult();
                CreateRoles(serviceScope).GetAwaiter().GetResult();
                CreateAdminAccountAsync(serviceScope).GetAwaiter().GetResult();
            }
           
        }

        private static async Task MigrateDatabaseAsync(IServiceScope serviceScope)
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<QvaCarUsersDBContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static async Task CreateRoles(IServiceScope serviceScope)
        {
            RoleManager<QvaCarIdentityRole> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<QvaCarIdentityRole>>();

            var roles = new[] { QvaCarIdentityRole.FreeUserRole, QvaCarIdentityRole.AdminRole };
            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role.Name) == null)
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task CreateAdminAccountAsync(IServiceScope serviceScope)
        {
            UserManager<QvaCarIdentityUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<QvaCarIdentityUser>>();

            string username = "Alice";
            string email = "alice@gmail.com";
            string password = "Blablacar@1";

            string firstName = "Alice";
            string lastName = "Smith";
            int age = 23;

            if (await userManager.FindByNameAsync(username) == null)
            {
                QvaCarIdentityUser user = new QvaCarIdentityUser
                {
                    Id = new Guid(),
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age,
                };

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var adminRole = QvaCarIdentityRole.AdminRole;
                    await userManager.AddToRoleAsync(user, adminRole.Name);
                }
            }
        }
    }
}
