namespace ChemicalSDS.Models
{
    public static class SdsWizardStep
    {
        public const int Total = 16;

        public static readonly IReadOnlyList<(int Number, string Title, string ShortTitle)> All =
        [
            (1, "Identification", "Identification"),
            (2, "Hazards Identification", "Hazards"),
            (3, "Composition / Ingredients", "Composition"),
            (4, "First Aid Measures", "First Aid"),
            (5, "Fire-Fighting Measures", "Fire Fighting"),
            (6, "Accidental Release", "Release"),
            (7, "Handling & Storage", "Handling"),
            (8, "Exposure Controls / PPE", "Exposure"),
            (9, "Physical & Chemical Properties", "Properties"),
            (10, "Stability & Reactivity", "Stability"),
            (11, "Toxicological Information", "Toxicology"),
            (12, "Ecological Information", "Ecology"),
            (13, "Disposal Considerations", "Disposal"),
            (14, "Transport Information", "Transport"),
            (15, "Regulatory Information", "Regulatory"),
            (16, "Other Information", "Other")
        ];

        public static string GetTitle(int step) =>
            All.FirstOrDefault(s => s.Number == step).Title ?? $"Section {step}";
    }
}
