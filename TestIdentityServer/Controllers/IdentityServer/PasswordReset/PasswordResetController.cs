using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestIdentityServer.Model;
using TestIdentityServer.Services;
using TestIdentityServer.ViewModels;

namespace TestIdentityServer.Controllers.IdentityServer.UserRegistration
{
    public class PasswordResetController : Controller
    {
        private const string ControllerName = "PasswordReset";
        private readonly UserManager<QvaCarIdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public PasswordResetController(UserManager<QvaCarIdentityUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult ForgotPassword(string returnUrl)
        {
            var viewModel = new PasswordResetViewModel() { ReturnUrl = returnUrl, };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(PasswordResetViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "There is no account for that email");
                return View(viewModel);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendTokenAsync(user, token);

            return View("ForgotPasswordConfirmation", new PasswordResetEmailConfirmationSentViewModel { ReturnUrl = viewModel.ReturnUrl });
        }

        [HttpGet]
        public IActionResult SetNewPassword(string email, string token)
        {
            var viewModel = new SetNewPasswordViewModel() { Email = email, Token = token };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNewPassword(SetNewPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "There is no account for that email");
                return View(viewModel);
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(viewModel);
            }

            return RedirectToAction(nameof(PasswordResetDone));
        }

        [HttpGet]
        public IActionResult PasswordResetDone()
        {
            return View();
        }

        private async Task SendTokenAsync(QvaCarIdentityUser user, string token)
        {
            var callback = Url.ActionLink(nameof(SetNewPassword), ControllerName, new { token, email = user.Email });
            string body = $"<a href=\"{callback}\" >Click to reset your password</a> or open in the browser {callback}";
            await _emailService.SendEmailAsync("info@mydomain.com", user.Email, "Reset Your Account Password", body);
        }
    }
}
