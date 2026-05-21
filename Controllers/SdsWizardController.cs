using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChemicalSDS.Data;
using ChemicalSDS.Models;
using ChemicalSDS.Services;

namespace ChemicalSDS.Controllers
{
    public class SdsWizardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SdsWizardService _wizard;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedImageTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];

        public SdsWizardController(AppDbContext context, SdsWizardService wizard, IWebHostEnvironment env)
        {
            _context = context;
            _wizard = wizard;
            _env = env;
        }

        private IActionResult? RequireLogin()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Step(int? id, int step = 1)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            step = Math.Clamp(step, 1, SdsWizardStep.Total);

            Chemical model;
            if (id == null || id == 0)
            {
                if (step > 1)
                {
                    TempData["Error"] = "Please complete Section 1 first.";
                    return RedirectToAction(nameof(Step), new { step = 1 });
                }
                model = new Chemical();
            }
            else
            {
                var existing = await _context.Chemicals
                    .FirstOrDefaultAsync(c => c.Id == id.Value && !c.IsDeleted);
                if (existing == null)
                {
                    TempData["Error"] = "Chemical not found or has been deleted.";
                    return RedirectToAction("Reports", "Home");
                }

                model = existing;
                var maxStep = Math.Min(SdsWizardStep.Total, model.LastCompletedSection + 1);
                if (step > maxStep)
                    step = maxStep;
            }

            ViewBag.Step = step;
            ViewBag.ChemicalId = model.Id > 0 ? model.Id : (int?)null;
            ViewBag.PageTitle = model.Id > 0 && !string.IsNullOrEmpty(model.ProductIdentifier)
                ? $"Edit SDS — {model.ProductIdentifier}"
                : "Add Safety Data Sheet";
            ViewBag.PageIcon = "fa-plus-circle";
            ViewData["Title"] = $"SDS Section {step}";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSection(
            int? id,
            int step,
            Chemical model,
            string direction,
            IFormFile? ghsPictogramImage)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            step = Math.Clamp(step, 1, SdsWizardStep.Total);
            direction = direction?.ToLowerInvariant() ?? "next";

            if (direction == "back")
            {
                if (id.HasValue && id > 0 && step > 1)
                    return RedirectToAction(nameof(Step), new { id, step = step - 1 });
                return RedirectToAction(nameof(Step), new { step = 1 });
            }

            if (!_wizard.ValidateSection(model, step, out var error))
            {
                TempData["Error"] = error;
                if (id.HasValue && id > 0)
                    return RedirectToAction(nameof(Step), new { id, step });
                return RedirectToAction(nameof(Step), new { step });
            }

            Chemical entity;
            if (id == null || id == 0)
            {
                entity = new Chemical { CreatedAt = DateTime.UtcNow, IsDraft = true };
                _wizard.ApplySection(entity, model, step);
                _context.Chemicals.Add(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                var existing = await _context.Chemicals
                    .FirstOrDefaultAsync(c => c.Id == id.Value && !c.IsDeleted);
                if (existing == null)
                {
                    TempData["Error"] = "Chemical not found or has been deleted.";
                    return RedirectToAction("Reports", "Home");
                }

                entity = existing;
                _wizard.ApplySection(entity, model, step);
            }

            if (step == 2 && ghsPictogramImage != null && ghsPictogramImage.Length > 0)
            {
                var path = await SaveGhsImageAsync(entity.Id, ghsPictogramImage);
                if (path != null)
                    entity.GhsPictogramImagePath = path;
            }

            if (step == 14)
                await SaveTransportHazardImagesAsync(entity);

            await _context.SaveChangesAsync();

            if (direction == "finish" || step >= SdsWizardStep.Total)
            {
                TempData["Success"] = $"Safety Data Sheet for \"{entity.ProductIdentifier}\" saved successfully.";
                return RedirectToAction("SDSDetails", "Home", new { id = entity.Id });
            }

            TempData["Success"] = $"Section {step} saved.";
            return RedirectToAction(nameof(Step), new { id = entity.Id, step = step + 1 });
        }

        private async Task<string?> SaveGhsImageAsync(int chemicalId, IFormFile file)
        {
            if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
                return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || ext.Length > 5)
                ext = ".png";

            var dir = Path.Combine(_env.WebRootPath, "uploads", "sds", chemicalId.ToString());
            Directory.CreateDirectory(dir);

            var fileName = $"ghs-label{ext}";
            var fullPath = Path.Combine(dir, fileName);

            foreach (var old in Directory.GetFiles(dir, "ghs-label.*"))
                System.IO.File.Delete(old);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"uploads/sds/{chemicalId}/{fileName}".Replace('\\', '/');
        }

        private async Task SaveTransportHazardImagesAsync(Chemical entity)
        {
            var authorities = new[] { "DOT", "TDG", "Mexico", "IMDG", "IATA" };
            var images = TransportHazardClassCatalog.ParseCustomImages(entity.TransportHazardClassImages);
            var changed = false;

            foreach (var auth in authorities)
            {
                var file = Request.Form.Files[$"transportHazardImage_{auth}"];
                if (file == null || file.Length == 0) continue;

                var path = await SaveTransportHazardImageAsync(entity.Id, auth, file);
                if (path == null) continue;
                images[auth] = path;
                changed = true;
            }

            if (changed)
                entity.TransportHazardClassImages = JsonSerializer.Serialize(images);
        }

        private async Task<string?> SaveTransportHazardImageAsync(int chemicalId, string authority, IFormFile file)
        {
            if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
                return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || ext.Length > 5)
                ext = ".png";

            var safeAuth = authority.Replace(" ", "_", StringComparison.Ordinal);
            var dir = Path.Combine(_env.WebRootPath, "uploads", "sds", chemicalId.ToString(), "transport");
            Directory.CreateDirectory(dir);

            var fileName = $"hazard-{safeAuth}{ext}";
            var fullPath = Path.Combine(dir, fileName);

            foreach (var old in Directory.GetFiles(dir, $"hazard-{safeAuth}.*"))
                System.IO.File.Delete(old);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"uploads/sds/{chemicalId}/transport/{fileName}".Replace('\\', '/');
        }
    }
}
