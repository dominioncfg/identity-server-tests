using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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

            string email = "pepitoperez@gmail.com";
            string password = "SuperPass@1";

            string firstName = "Jose";
            string lastName = "Perez";
            int age = 26;
            string address =  "Calle 13";

            if (await userManager.FindByNameAsync(email) == null)
            {
                QvaCarIdentityUser user = new QvaCarIdentityUser
                {
                    Id = new Guid(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age,
                    ProvinceId = Province.Cienfuegos.Id,  
                    Address = address,
                };
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var adminRole = QvaCarIdentityRole.AdminRole;
                    await userManager.AddToRoleAsync(user, adminRole.Name);
                }

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
                claims.Add(new Claim(JwtClaimTypes.Address, user.Address));
                claims.Add(new Claim(QvaCarClaims.Province, Province.Cienfuegos.Name));

                await userManager.AddClaimsAsync(user, claims);
            }
        }
    }
}
