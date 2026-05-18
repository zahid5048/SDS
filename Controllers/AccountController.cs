using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ChemicalSDS.Models;
using ChemicalSDS.Services;

namespace ChemicalSDS.Controllers
{
    public class AccountController : Controller
    {
        private const string CaptchaAKey = "LoginCaptchaA";
        private const string CaptchaBKey = "LoginCaptchaB";

        private readonly UserAuthService _auth;

        public AccountController(UserAuthService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
                return RedirectToAction("Dashboard", "Home");

            return View(NewCaptchaModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (HttpContext.Session.GetString("Username") != null)
                return RedirectToAction("Dashboard", "Home");

            if (!ModelState.IsValid)
            {
                RefreshCaptcha(model);
                return View(model);
            }

            if (!ValidateCaptcha(model.CaptchaAnswer))
            {
                ModelState.AddModelError(nameof(model.CaptchaAnswer), "Incorrect answer. Please try again.");
                RefreshCaptcha(model);
                ViewBag.Error = "Security check failed — add the two numbers correctly.";
                return View(model);
            }

            var user = await _auth.AuthenticateAsync(model.Username.Trim(), model.Password);
            if (user != null)
            {
                ClearCaptcha();
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role ?? "User");
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("FullName", user.FullName);
                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            RefreshCaptcha(model);
            model.Password = string.Empty;
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private LoginViewModel NewCaptchaModel()
        {
            var a = Random.Shared.Next(2, 12);
            var b = Random.Shared.Next(2, 12);
            HttpContext.Session.SetInt32(CaptchaAKey, a);
            HttpContext.Session.SetInt32(CaptchaBKey, b);
            return new LoginViewModel { CaptchaNum1 = a, CaptchaNum2 = b };
        }

        private void RefreshCaptcha(LoginViewModel model)
        {
            var fresh = NewCaptchaModel();
            model.CaptchaNum1 = fresh.CaptchaNum1;
            model.CaptchaNum2 = fresh.CaptchaNum2;
            model.CaptchaAnswer = null;
        }

        private bool ValidateCaptcha(int? answer)
        {
            var a = HttpContext.Session.GetInt32(CaptchaAKey);
            var b = HttpContext.Session.GetInt32(CaptchaBKey);
            if (a == null || b == null || answer == null)
                return false;

            var valid = answer.Value == a.Value + b.Value;
            ClearCaptcha();
            return valid;
        }

        private void ClearCaptcha()
        {
            HttpContext.Session.Remove(CaptchaAKey);
            HttpContext.Session.Remove(CaptchaBKey);
        }
    }
}
