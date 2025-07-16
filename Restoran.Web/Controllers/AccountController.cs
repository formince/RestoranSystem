using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.User;
using System.Security.Claims;

namespace Restoran.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            // This action will return the login view
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid) return View(userLoginDto);

            var userBll = new BLLAuth();
            var result = await userBll.LoginAsync(userLoginDto);

            if (result == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                return View(userLoginDto);
            }

            // Giriş başarılı ise kullanıcı bilgilerini oturuma kaydet
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result!.Id.ToString()),
                new Claim(ClaimTypes.Name, result.Username),
                new Claim(ClaimTypes.Role, result.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            // This action will return the registration view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userRegisterDto);
            }
            var userBll = new BLLAuth();
            var result = await userBll.RegisterAsync(userRegisterDto);
            if (result.Success == false)
            {
                ModelState.AddModelError("", "Kayıt işlemi başarısız");
                return View(userRegisterDto);
            }
            // Kayıt başarılı ise giriş yap
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            // Oturumu kapat
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");

        }
    }
}
