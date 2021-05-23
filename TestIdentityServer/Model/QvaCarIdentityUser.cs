using Microsoft.AspNetCore.Identity;
using System;

namespace TestIdentityServer.Model
{
    public class QvaCarIdentityUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
