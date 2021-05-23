using Microsoft.AspNetCore.Identity;
using System;
namespace TestIdentityServer.Model
{
    public class QvaCarIdentityRole : IdentityRole<Guid>
    {
        public QvaCarIdentityRole() : base() { }
        public QvaCarIdentityRole(Guid id, string roleName) : base(roleName) {
            Id = id;
        }

        public static readonly QvaCarIdentityRole AdminRole = new QvaCarIdentityRole(new Guid("998a3987-c1ba-4551-b197-8c25d5214e50"), "Admin");
        public static readonly QvaCarIdentityRole FreeUserRole = new QvaCarIdentityRole(new Guid("e7e638f8-949f-4e33-98ee-07d353e7044a"), "FreeUser");
    }
}
