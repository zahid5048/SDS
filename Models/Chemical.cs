using System.ComponentModel.DataAnnotations;

namespace ChemicalSDS.Models
{
    public class Chemical
    {
        [Key]
        public int Id { get; set; }

        public string? ProductIdentifier { get; set; }

        public string? Sec01Subsections { get; set; }
        public string? Sec02Blocks { get; set; }
        public string? Sec03Blocks { get; set; }
        public string? Sec04Subsections { get; set; }
        public string? Sec05Subsections { get; set; }
        public string? Sec06Subsections { get; set; }
        public string? Sec07Subsections { get; set; }
        public string? Sec08Subsections { get; set; }
        public string? Sec09Subsections { get; set; }
        public string? Sec10Subsections { get; set; }
        public string? Sec11Subsections { get; set; }
        public string? Sec12Subsections { get; set; }
        public string? Sec13Subsections { get; set; }
        public string? Sec14Subsections { get; set; }
        public string? Sec15Subsections { get; set; }
        public string? Sec16Subsections { get; set; }

        public int LastCompletedSection { get; set; }
        public bool IsDraft { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
