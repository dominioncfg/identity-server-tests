using System.ComponentModel.DataAnnotations;

namespace TestIdentityServer.ViewModels
{
    public class PasswordResetEmailConfirmationSentViewModel
    {
        public string ReturnUrl { get; set; }
    }
}
