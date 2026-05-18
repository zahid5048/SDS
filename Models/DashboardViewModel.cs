namespace ChemicalSDS.Models
{
    public class DashboardViewModel
    {
        public string ListAction { get; set; } = "Dashboard";

        /// <summary>True when showing the deleted-chemicals (recycle bin) list.</summary>
        public bool IsDeletedView => ListAction == "Deleted";

        public List<Chemical> Chemicals { get; set; } = [];

        public string? Search { get; set; }
        public string SortBy { get; set; } = "product";
        public string SortDir { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int FilteredCount { get; set; }
        public int TotalChemicals { get; set; }
        public int CompletedCount { get; set; }
        public int DraftCount { get; set; }
        public int DangerCount { get; set; }
        public int WarningCount { get; set; }

        public DashboardChartData Charts { get; set; } = new();

        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling(FilteredCount / (double)PageSize)
            : 1;

        public int ShowingFrom => FilteredCount == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int ShowingTo => Math.Min(Page * PageSize, FilteredCount);

        public bool IsSorted(string column) =>
            string.Equals(SortBy, column, StringComparison.OrdinalIgnoreCase);

        public string NextSortDir(string column) =>
            IsSorted(column) && SortDir.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        public string SortIcon(string column)
        {
            if (!IsSorted(column)) return "fa-sort";
            return SortDir.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "fa-sort-up" : "fa-sort-down";
        }
    }
}
