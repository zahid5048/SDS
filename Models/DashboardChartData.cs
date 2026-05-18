namespace ChemicalSDS.Models
{
    public class DashboardChartData
    {
        public string[] StatusLabels { get; set; } = ["Complete", "In Progress"];
        public int[] StatusValues { get; set; } = [0, 0];

        public string[] SignalLabels { get; set; } = ["Danger", "Warning", "Other"];
        public int[] SignalValues { get; set; } = [0, 0, 0];

        public string[] MonthlyLabels { get; set; } = [];
        public int[] MonthlyValues { get; set; } = [];

        public string[] ProgressLabels { get; set; } = ["Not started", "Sections 1–8", "Sections 9–15", "Complete"];
        public int[] ProgressValues { get; set; } = [0, 0, 0, 0];
    }
}
