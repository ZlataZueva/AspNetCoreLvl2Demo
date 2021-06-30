using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ASPNETCoreLvl2Demo.Identity;
using ASPNETCoreLvl2Demo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreLvl2Demo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;

        public AccountController(UserManager<CustomUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user == null)
                {
                    user = new CustomUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.Name,
                        FavoriteMusician = model.FavoriteMusician
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    return result.Succeeded ? View("Success") : View();
                }

                ModelState.AddModelError("", "Registration failed");

                return View();
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    if (user.FavoriteMusician != null)
                    {
                        identity.AddClaim(new Claim("FavoriteMusician", user.FavoriteMusician));
                    }
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

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