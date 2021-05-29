using Microsoft.AspNetCore.Identity;
using System;

namespace TestIdentityServer.Model
{
    public class QvaCarIdentityUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public int ProvinceId { get; set; }
    }
}
