using System;
using System.Threading.Tasks;
using ASPNETCoreLvl2Demo.Identity;
using ASPNETCoreLvl2Demo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreLvl2Demo.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;

        public AccountController(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new CustomUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Name,
                    FavoriteMusician = model.FavoriteMusician
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return result.Succeeded ? View("Success") : View();
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action(nameof(ResetPassword), "Account",
                        new { token, name = user.UserName }, Request.Scheme);

                    await System.IO.File.WriteAllTextAsync("resetLink.txt", resetLink);
                }
                else
                {
                    ModelState.AddModelError("", "User with specified name not found");
                }
            }
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string name)
        {
            var model = new ResetPasswordModel { Token = token, Name = name };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return result.Succeeded ? View("Success") : View();
                }
                ModelState.AddModelError("", "Invalid request");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Name, model.Password, true, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Claims));
                }

                ModelState.AddModelError("", "Invalid user name or password");
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Policy = "IsVaccinated")]
        public IActionResult Travel()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "HasFavoriteMusician")]
        public IActionResult Music()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}