using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TestIdentityServer.Model;
using TestIdentityServer.Services;
using TestIdentityServer.ViewModels;

namespace TestIdentityServer.Controllers.IdentityServer.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        private readonly UserManager<QvaCarIdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public UserRegistrationController(UserManager<QvaCarIdentityUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult RegisterUser(string returnUrl)
        {
            var vm = new RegisterUserViewModel()
            { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "User Already Exists");
                return View(model);
            }

            var userToCreate = new QvaCarIdentityUser
            {
                Id = Guid.NewGuid(),
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = false,
            };

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(JwtClaimTypes.Address, model.Address));
            claims.Add(new Claim(JwtClaimTypes.GivenName, model.GivenName));
            claims.Add(new Claim(JwtClaimTypes.FamilyName, model.FamilyName));

            var createResult = await _userManager.CreateAsync(userToCreate, model.Password);

            if (!createResult.Succeeded)
            {
                ModelState.AddModelError("", "Fail to create your User");
                return View(model);
            }

            var createdUser = await _userManager.FindByEmailAsync(model.Email);
            var roleResult = await _userManager.AddToRoleAsync(createdUser, QvaCarIdentityRole.FreeUserRole.Name);
            var claimsResult = await _userManager.AddClaimsAsync(createdUser, claims);

            if (!roleResult.Succeeded)
            {
                ModelState.AddModelError("", "Fail to create your User");
                return View(model);
            }

            if (!claimsResult.Succeeded)
            {
                ModelState.AddModelError("", "Fail to create your User");
                return View(model);
            }

            await SendConfirmationEmailAsync(createdUser);
            return RedirectToAction(nameof(RegistrationCodeSent), new { returnUrl = model.ReturnUrl });
        }
        [HttpGet]
        public  IActionResult RegistrationCodeSent(string returnUrl)
        {
            return View(new RegistrationCodeSentViewModel() { ReturnUrl = returnUrl });
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return View("EmailConfirmed");
                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            return NotFound("User does not exist");
        }

        private async Task SendConfirmationEmailAsync(QvaCarIdentityUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.ActionLink(nameof(ConfirmEmail), "UserRegistration", new { userId = user.Id, @token = token });
            string body = $"<a href=\"{confirmationLink}\">Confirm Email</a> or Copy this Url {confirmationLink}.";
            await _emailService.SendEmailAsync("info@mydomain.com", user.Email, "Confirm your email address", body);
        }
    }
}
