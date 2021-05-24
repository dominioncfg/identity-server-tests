using System.ComponentModel.DataAnnotations;

namespace TestIdentityServer.ViewModels
{
    public class PasswordResetViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }
}
