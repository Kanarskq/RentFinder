using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RentFinder.Web.Mvc.Models;
using RentFinder.Web.Mvc.Services;

namespace RentFinder.Web.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _authService.LoginAsync(model);
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Store token in session
                HttpContext.Session.SetString("JwtToken", response.Token);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _authService.RegisterAsync(model);
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Store token in session
                HttpContext.Session.SetString("JwtToken", response.Token);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Registration failed");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await _authService.GetUserProfileAsync();
            if (profile == null)
            {
                return RedirectToAction("Login");
            }
            return View(profile);
        }
    }
}
