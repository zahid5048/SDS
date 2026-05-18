using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ChemicalSDS.Data;
using ChemicalSDS.Models;
using ChemicalSDS.Services;

namespace ChemicalSDS.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SdsPdfGenerator _pdfGenerator;
        private readonly ChemicalListService _chemicalList;
        private readonly ChemicalDeletionService _chemicalDeletion;

        public HomeController(
            AppDbContext context,
            SdsPdfGenerator pdfGenerator,
            ChemicalListService chemicalList,
            ChemicalDeletionService chemicalDeletion)
        {
            _context = context;
            _pdfGenerator = pdfGenerator;
            _chemicalList = chemicalList;
            _chemicalDeletion = chemicalDeletion;
        }

        private IActionResult? RequireLogin()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");
            return null;
        }

        public async Task<IActionResult> Dashboard(
            string? search,
            int page = 1,
            int pageSize = 10,
            string sortBy = "product",
            string sortDir = "asc")
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "Dashboard";
            ViewBag.PageIcon = "fa-tachometer-alt";

            var model = await _chemicalList.GetListAsync("Dashboard", search, page, pageSize, sortBy, sortDir);
            return View(model);
        }

        public async Task<IActionResult> Reports(
            string? search,
            int page = 1,
            int pageSize = 10,
            string sortBy = "product",
            string sortDir = "asc")
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "All Chemicals";
            ViewBag.PageIcon = "fa-flask";

            var model = await _chemicalList.GetListAsync("Reports", search, page, pageSize, sortBy, sortDir);
            return View(model);
        }

        public async Task<IActionResult> Deleted(
            string? search,
            int page = 1,
            int pageSize = 10,
            string sortBy = "deleted",
            string sortDir = "desc")
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            ViewBag.PageTitle = "Deleted Chemicals";
            ViewBag.PageIcon = "fa-trash-restore";

            var model = await _chemicalList.GetListAsync("Deleted", search, page, pageSize, sortBy, sortDir);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id, string returnAction = "Reports")
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var username = HttpContext.Session.GetString("Username");
            var chemical = await _chemicalDeletion.SoftDeleteAsync(id, username);
            if (chemical == null)
            {
                TempData["Error"] = "Chemical not found or already deleted.";
                return RedirectToList(returnAction);
            }

            TempData["Success"] = $"\"{chemical.ProductIdentifier ?? chemical.ChemicalName}\" moved to deleted.";
            return RedirectToList(returnAction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var chemical = await _chemicalDeletion.RestoreAsync(id);
            if (chemical == null)
            {
                TempData["Error"] = "Could not restore — record not found.";
                return RedirectToAction(nameof(Deleted));
            }

            TempData["Success"] = $"\"{chemical.ProductIdentifier ?? chemical.ChemicalName}\" restored successfully.";
            return RedirectToAction(nameof(Reports));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var chemical = await _chemicalDeletion.PermanentDeleteAsync(id);
            if (chemical == null)
            {
                TempData["Error"] = "Could not delete — record not found or not in deleted list.";
                return RedirectToAction(nameof(Deleted));
            }

            TempData["Success"] = $"\"{chemical.ProductIdentifier ?? chemical.ChemicalName}\" permanently removed.";
            return RedirectToAction(nameof(Deleted));
        }

        private IActionResult RedirectToList(string returnAction) =>
            returnAction switch
            {
                "Dashboard" => RedirectToAction(nameof(Dashboard)),
                "Deleted" => RedirectToAction(nameof(Deleted)),
                _ => RedirectToAction(nameof(Reports))
            };

        // SDS Details - Full 16 Sections
        public async Task<IActionResult> SDSDetails(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var chemical = await _context.Chemicals
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (chemical == null)
                return NotFound();

            ViewBag.PageTitle = $"{chemical.ProductIdentifier} - SDS";
            ViewBag.PageIcon = "fa-file-alt";
            return View(chemical);
        }

        public async Task<IActionResult> DownloadSdsPdf(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var chemical = await _context.Chemicals
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (chemical == null)
                return NotFound();

            var bytes = _pdfGenerator.Generate(chemical);
            var name = $"{chemical.ProductIdentifier ?? chemical.ChemicalName ?? "SDS"}_SafetyDataSheet.pdf";
            name = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));

            return File(bytes, "application/pdf", name);
        }

        public async Task<IActionResult> Emergency(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var chemical = await _context.Chemicals
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (chemical == null)
                return NotFound();

            ViewBag.PageTitle = $"Emergency - {chemical.ChemicalName}";
            ViewBag.PageIcon = "fa-exclamation-circle";
            return View(chemical);
        }

        public IActionResult IndustrialContact()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            return View();
        }

        public IActionResult SectorsSpecialist()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            return View();
        }

        public async Task<IActionResult> HazardCategories()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;

            var chemicals = await _context.Chemicals
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            return View(chemicals);
        }

        public IActionResult RiskAssessment()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            return View();
        }

        public IActionResult Training()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            return View();
        }

        public IActionResult AccidentInvestigation()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            return View();
        }
    }
}