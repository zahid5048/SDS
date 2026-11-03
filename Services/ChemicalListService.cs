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
                "status" or "date" or "deleted" => sortBy,
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
                SortDir = sortDir
            };

            if (listAction == "Dashboard")
                await LoadDashboardStatsAsync(model, activeQuery);
            else
            {
                model.TotalChemicals = await activeQuery.CountAsync();
                model.DraftCount = await activeQuery.CountAsync(c => c.IsDraft);
                model.CompletedCount = model.TotalChemicals - model.DraftCount;
            }

            var query = allQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var term = model.Search;
                query = query.Where(c =>
                    c.ProductIdentifier != null && c.ProductIdentifier.Contains(term));
            }

            model.FilteredCount = await query.CountAsync();

            query = (sortBy, sortDir) switch
            {
                ("status", "desc") => query.OrderByDescending(c => c.IsDraft).ThenByDescending(c => c.LastCompletedSection),
                ("status", _) => query.OrderBy(c => c.IsDraft).ThenBy(c => c.LastCompletedSection),
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
                .Select(c => new Chemical
                {
                    Id = c.Id,
                    ProductIdentifier = c.ProductIdentifier,
                    IsDraft = c.IsDraft,
                    LastCompletedSection = c.LastCompletedSection,
                    DeletedAt = c.DeletedAt,
                    DeletedBy = c.DeletedBy,
                    IsDeleted = c.IsDeleted
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return model;
        }

        private static async Task LoadDashboardStatsAsync(DashboardViewModel model, IQueryable<Chemical> activeQuery)
        {
            // Narrow columns only — full Chemical rows include huge SDS JSON/text and slow counts badly
            var slim = activeQuery.Select(c => new
            {
                c.IsDraft,
                c.LastCompletedSection,
                c.CreatedAt
            });

            var stats = await slim
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Draft = g.Count(x => x.IsDraft),
                    NotStarted = g.Count(x => x.IsDraft && x.LastCompletedSection <= 0),
                    MidProgress = g.Count(x => x.IsDraft && x.LastCompletedSection >= 1 && x.LastCompletedSection <= 8),
                    LateProgress = g.Count(x => x.IsDraft && x.LastCompletedSection >= 9 && x.LastCompletedSection <= 15)
                })
                .FirstOrDefaultAsync();

            model.TotalChemicals = stats?.Total ?? 0;
            model.DraftCount = stats?.Draft ?? 0;
            model.CompletedCount = model.TotalChemicals - model.DraftCount;

            var charts = model.Charts;
            charts.StatusValues = [model.CompletedCount, model.DraftCount];

            charts.ProgressValues =
            [
                stats?.NotStarted ?? 0,
                stats?.MidProgress ?? 0,
                stats?.LateProgress ?? 0,
                model.CompletedCount
            ];

            var startMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
            var createdDates = await slim
                .Where(x => x.CreatedAt >= startMonth)
                .Select(x => x.CreatedAt)
                .ToListAsync();

            var labels = new List<string>();
            var values = new List<int>();
            for (var i = 0; i < 6; i++)
            {
                var monthStart = startMonth.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1);
                labels.Add(monthStart.ToString("MMM yyyy"));
                values.Add(createdDates.Count(d => d >= monthStart && d < monthEnd));
            }

            charts.MonthlyLabels = labels.ToArray();
            charts.MonthlyValues = values.ToArray();
        }
    }
}
