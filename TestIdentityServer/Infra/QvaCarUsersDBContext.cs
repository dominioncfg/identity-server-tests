using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TestIdentityServer.Model;

namespace TestIdentityServer.Infra
{
  
    public class QvaCarUsersDBContext :
        IdentityDbContext
        <
            QvaCarIdentityUser,
            QvaCarIdentityRole,
            Guid,
            IdentityUserClaim<Guid>,
            QvaCarIdentityUserRole,
            IdentityUserLogin<Guid>,
            IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>
        >
    {
        public QvaCarUsersDBContext() { }
        public QvaCarUsersDBContext(DbContextOptions<QvaCarUsersDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            string UsersShcema = "Core";

            modelBuilder.Entity<QvaCarIdentityUser>(b =>
            {
                b.ToTable("Users", UsersShcema);
            });

            modelBuilder.Entity<QvaCarIdentityRole>(b =>
            {
                b.ToTable("Roles", UsersShcema);
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.ToTable("Claims", UsersShcema);
            });

            modelBuilder.Entity<QvaCarIdentityUserRole>(b =>
            {
                b.ToTable("User_Roles", UsersShcema);
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.ToTable("UserLogins", UsersShcema);
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.ToTable("UserTokens", UsersShcema);
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.ToTable("Role_Claims", UsersShcema);
            });
        }
    }
}
