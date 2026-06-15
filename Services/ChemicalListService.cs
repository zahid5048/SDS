using ChemicalSDS.Data;
using ChemicalSDS.Models;
using Microsoft.EntityFrameworkCore;

namespace ChemicalSDS.Services
{
    public class ChemicalListService
    {
        private readonly AppDbContext _context;

        public ChemicalListService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetListAsync(
            string listAction,
            string? search,
            int page = 1,
            int pageSize = 10,
            string sortBy = "product",
            string sortDir = "asc")
        {
            page = Math.Max(1, page);
            pageSize = pageSize switch
            {
                5 or 10 or 25 or 50 or 100 => pageSize,
                _ => 10
            };

            var deletedOnly = listAction == "Deleted";

            sortBy = sortBy.ToLowerInvariant() switch
            {
                "cas" or "status" or "signal" or "flash" or "storage" or "date" or "deleted" => sortBy,
                _ => "product"
            };

            sortDir = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

            var activeQuery = _context.Chemicals.AsNoTracking().Where(c => !c.IsDeleted);
            var allQuery = deletedOnly
                ? _context.Chemicals.AsNoTracking().Where(c => c.IsDeleted)
                : activeQuery;

            var model = new DashboardViewModel
            {
                ListAction = listAction,
                Search = search?.Trim(),
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDir = sortDir,
                TotalChemicals = await activeQuery.CountAsync(),
                DraftCount = await activeQuery.CountAsync(c => c.IsDraft),
                DangerCount = await activeQuery.CountAsync(c => c.SignalWord == "Danger")
            };
            model.CompletedCount = model.TotalChemicals - model.DraftCount;
            model.WarningCount = await activeQuery.CountAsync(c => c.SignalWord == "Warning");

            if (listAction == "Dashboard")
                await LoadChartStatsAsync(model, activeQuery);

            var query = allQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var term = model.Search;
                query = query.Where(c =>
                    (c.ProductIdentifier != null && c.ProductIdentifier.Contains(term)) ||
                    (c.ChemicalName != null && c.ChemicalName.Contains(term)) ||
                    (c.CASNumber != null && c.CASNumber.Contains(term)) ||
                    (c.SignalWord != null && c.SignalWord.Contains(term)) ||
                    (c.StorageLocation != null && c.StorageLocation.Contains(term)) ||
                    (c.SDSNumber != null && c.SDSNumber.Contains(term)));
            }

            model.FilteredCount = await query.CountAsync();

            query = (sortBy, sortDir) switch
            {
                ("cas", "desc") => query.OrderByDescending(c => c.CASNumber),
                ("cas", _) => query.OrderBy(c => c.CASNumber),
                ("status", "desc") => query.OrderByDescending(c => c.IsDraft).ThenByDescending(c => c.LastCompletedSection),
                ("status", _) => query.OrderBy(c => c.IsDraft).ThenBy(c => c.LastCompletedSection),
                ("signal", "desc") => query.OrderByDescending(c => c.SignalWord),
                ("signal", _) => query.OrderBy(c => c.SignalWord),
                ("flash", "desc") => query.OrderByDescending(c => c.FlashPoint),
                ("flash", _) => query.OrderBy(c => c.FlashPoint),
                ("storage", "desc") => query.OrderByDescending(c => c.StorageLocation),
                ("storage", _) => query.OrderBy(c => c.StorageLocation),
                ("date", "desc") => query.OrderByDescending(c => c.CreatedAt),
                ("date", _) => query.OrderBy(c => c.CreatedAt),
                ("deleted", "desc") => query.OrderByDescending(c => c.DeletedAt),
                ("deleted", _) => query.OrderBy(c => c.DeletedAt),
                ("product", "desc") => query.OrderByDescending(c => c.ProductIdentifier),
                _ => query.OrderBy(c => c.ProductIdentifier)
            };

            var totalPages = model.TotalPages;
            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            model.Page = page;
            model.Chemicals = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return model;
        }

        private static async Task LoadChartStatsAsync(DashboardViewModel model, IQueryable<Chemical> activeQuery)
        {
            var charts = model.Charts;
            charts.StatusValues = [model.CompletedCount, model.DraftCount];

            var otherSignal = Math.Max(0, model.TotalChemicals - model.DangerCount - model.WarningCount);
            charts.SignalValues = [model.DangerCount, model.WarningCount, otherSignal];

            var notStarted = await activeQuery.CountAsync(c => c.IsDraft && c.LastCompletedSection <= 0);
            var midProgress = await activeQuery.CountAsync(c => c.IsDraft && c.LastCompletedSection >= 1 && c.LastCompletedSection <= 8);
            var lateProgress = await activeQuery.CountAsync(c => c.IsDraft && c.LastCompletedSection >= 9 && c.LastCompletedSection <= 15);
            charts.ProgressValues = [notStarted, midProgress, lateProgress, model.CompletedCount];

            var utcNow = DateTime.UtcNow;
            var startMonth = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5);
            var chemicals = await activeQuery
                .Where(c => c.CreatedAt >= startMonth)
                .Select(c => c.CreatedAt)
                .ToListAsync();

            var labels = new List<string>();
            var values = new List<int>();
            for (var i = 0; i < 6; i++)
            {
                var monthStart = startMonth.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1);
                labels.Add(monthStart.ToString("MMM yyyy"));
                values.Add(chemicals.Count(d => d >= monthStart && d < monthEnd));
            }

            charts.MonthlyLabels = labels.ToArray();
            charts.MonthlyValues = values.ToArray();
        }
    }
}
