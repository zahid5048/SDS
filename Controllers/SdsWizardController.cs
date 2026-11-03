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
            var productLabel = Sec01SubsectionCodec.ShortProductLabel(model.ProductIdentifier);
            ViewBag.PageTitle = model.Id > 0 && !string.IsNullOrEmpty(productLabel)
                ? $"Edit SDS — {productLabel}"
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
            string direction)
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

            if (step == 2)
                await ApplySec02CustomPictogramUploadsAsync(entity);

            await _context.SaveChangesAsync();

            if (direction == "finish" || step >= SdsWizardStep.Total)
            {
                TempData["Success"] = $"Safety Data Sheet for \"{entity.ProductIdentifier}\" saved successfully.";
                return RedirectToAction("SDSDetails", "Home", new { id = entity.Id });
            }

            TempData["Success"] = $"Section {step} saved.";
            return RedirectToAction(nameof(Step), new { id = entity.Id, step = step + 1 });
        }

        private async Task ApplySec02CustomPictogramUploadsAsync(Chemical entity)
        {
            var blocks = Sec02BlockCodec.Parse(entity.Sec02Blocks);
            if (blocks.Count == 0)
                return;

            var changed = false;
            foreach (var file in Request.Form.Files.Where(f =>
                         f.Name.StartsWith("sec02CustomPic_", StringComparison.Ordinal) && f.Length > 0))
            {
                var blockId = file.Name["sec02CustomPic_".Length..];
                if (string.IsNullOrWhiteSpace(blockId))
                    continue;

                var path = await SaveSec02CustomPictogramAsync(entity.Id, blockId, file);
                if (path == null)
                    continue;

                var block = blocks.FirstOrDefault(b =>
                    b.Type == "pictograms" && string.Equals(b.BlockId, blockId, StringComparison.Ordinal));
                if (block == null)
                    continue;

                block.CustomImagePath = path;
                changed = true;
            }

            if (changed)
                entity.Sec02Blocks = Sec02BlockCodec.Serialize(blocks);
        }

        private async Task<string?> SaveSec02CustomPictogramAsync(int chemicalId, string blockId, IFormFile file)
        {
            if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
                return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || ext.Length > 5)
                ext = ".png";

            var safeId = string.Concat(blockId.Where(ch => char.IsLetterOrDigit(ch) || ch == '-'));
            if (string.IsNullOrEmpty(safeId))
                safeId = Guid.NewGuid().ToString("N");

            var dir = Path.Combine(_env.WebRootPath, "uploads", "sds", chemicalId.ToString());
            Directory.CreateDirectory(dir);

            var fileName = $"sec02-pic-{safeId}{ext}";
            var fullPath = Path.Combine(dir, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"uploads/sds/{chemicalId}/{fileName}".Replace('\\', '/');
        }
    }
}
