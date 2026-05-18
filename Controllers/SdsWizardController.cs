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

        public SdsWizardController(AppDbContext context, SdsWizardService wizard)
        {
            _context = context;
            _wizard = wizard;
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
                model = await _context.Chemicals
                    .FirstOrDefaultAsync(c => c.Id == id.Value && !c.IsDeleted);
                if (model == null)
                {
                    TempData["Error"] = "Chemical not found or has been deleted.";
                    return RedirectToAction("Reports", "Home");
                }

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
        public async Task<IActionResult> SaveSection(int? id, int step, Chemical model, string direction)
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
            }
            else
            {
                entity = await _context.Chemicals
                    .FirstOrDefaultAsync(c => c.Id == id.Value && !c.IsDeleted);
                if (entity == null)
                {
                    TempData["Error"] = "Chemical not found or has been deleted.";
                    return RedirectToAction("Reports", "Home");
                }
                _wizard.ApplySection(entity, model, step);
            }

            await _context.SaveChangesAsync();

            if (direction == "finish" || step >= SdsWizardStep.Total)
            {
                TempData["Success"] = $"Safety Data Sheet for \"{entity.ProductIdentifier}\" saved successfully.";
                return RedirectToAction("SDSDetails", "Home", new { id = entity.Id });
            }

            TempData["Success"] = $"Section {step} saved.";
            return RedirectToAction(nameof(Step), new { id = entity.Id, step = step + 1 });
        }
    }
}
