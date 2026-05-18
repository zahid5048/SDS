using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ChemicalSDS.Data;
using ChemicalSDS.Models;
using ChemicalSDS.Services;

namespace ChemicalSDS.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserAuthService _auth;

        public UsersController(AppDbContext context, UserAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        private IActionResult? RequireAdmin()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["Error"] = "Only administrators can manage users.";
                return RedirectToAction("Dashboard", "Home");
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "Users";
            ViewBag.PageIcon = "fa-users";

            var users = await _context.Users
                .OrderByDescending(u => u.Role == "Admin")
                .ThenBy(u => u.FullName)
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "Add User";
            ViewBag.PageIcon = "fa-user-plus";
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "Add User";
            ViewBag.PageIcon = "fa-user-plus";

            if (!ModelState.IsValid)
                return View(model);

            if (await _auth.UsernameExistsAsync(model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "This username is already taken.");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
                return View(model);
            }

            _context.Users.Add(new User
            {
                Username = model.Username.Trim(),
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                PasswordHash = UserAuthService.HashPassword(model.Password),
                Role = model.Role is "Admin" or "Manager" or "User" ? model.Role : "User",
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User \"{model.Username}\" created successfully. They can now sign in.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.PageTitle = "Edit User";
            ViewBag.PageIcon = "fa-user-edit";

            return View(new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role ?? "User",
                IsActive = user.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "Edit User";
            ViewBag.PageIcon = "fa-user-edit";

            var user = await _context.Users.FindAsync(model.Id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.ConfirmNewPassword))
                    ModelState.AddModelError(nameof(model.ConfirmNewPassword), "Please confirm the new password.");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (await _auth.UsernameExistsAsync(model.Username, model.Id))
            {
                ModelState.AddModelError(nameof(model.Username), "This username is already taken.");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != model.Id))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
                return View(model);
            }

            var currentUsername = HttpContext.Session.GetString("Username");
            if (user.Username == currentUsername && !model.IsActive)
            {
                ModelState.AddModelError(nameof(model.IsActive), "You cannot deactivate your own account.");
                return View(model);
            }

            user.Username = model.Username.Trim();
            user.FullName = model.FullName.Trim();
            user.Email = model.Email.Trim();
            user.Role = model.Role is "Admin" or "Manager" or "User" ? model.Role : "User";
            user.IsActive = model.IsActive;

            if (!string.IsNullOrEmpty(model.NewPassword))
                user.PasswordHash = UserAuthService.HashPassword(model.NewPassword);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"User \"{user.Username}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var currentUsername = HttpContext.Session.GetString("Username");
            if (user.Username == currentUsername)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            if (user.Username == "admin")
            {
                TempData["Error"] = "The default admin account cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User \"{user.Username}\" deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
