using System.ComponentModel.DataAnnotations;

namespace ChemicalSDS.Models
{
    public class Chemical
    {
        [Key]
        public int Id { get; set; }

        // Section 1: Identification
        public string? ProductIdentifier { get; set; }
        public string? ChemicalName { get; set; }
        public string? OtherMeansOfId { get; set; }
        public string? ProductType { get; set; }
        public string? ProductUse { get; set; }
        public string? Synonym { get; set; }
        public string? SDSNumber { get; set; }
        public string? SupplierDetails { get; set; }
        public string? EmergencyPhone { get; set; }

        // Wizard progress
        public int LastCompletedSection { get; set; }
        public bool IsDraft { get; set; } = true;

        // Section 2: Hazards
        public string? HazardsNotOtherwiseClassified { get; set; }
        public string? DateOfPreviousIssue { get; set; }
        public string? OSHASstatus { get; set; }
        public string? Classification { get; set; }
        public string? SignalWord { get; set; }
        public string? HazardStatements { get; set; }
        public string? PrecautionaryGeneral { get; set; }
        public string? PrecautionaryPrevention { get; set; }
        public string? PrecautionaryResponse { get; set; }
        public string? PrecautionaryStorage { get; set; }
        public string? PrecautionaryDisposal { get; set; }

        // Section 3: Composition
        public string? SubstanceMixture { get; set; }
        public string? ProductCode { get; set; }
        public string? CASNumber { get; set; }
        public string? IngredientName { get; set; }
        public string? Percentage { get; set; }

        // Section 4: First Aid
        public string? EyeContactFirstAid { get; set; }
        public string? InhalationFirstAid { get; set; }
        public string? SkinContactFirstAid { get; set; }
        public string? IngestionFirstAid { get; set; }

        // Section 5: Fire Fighting
        public string? SuitableExtinguishingMedia { get; set; }
        public string? UnsuitableExtinguishingMedia { get; set; }
        public string? SpecificHazards { get; set; }
        public string? FlashPoint { get; set; }
        public string? AutoIgnitionTemp { get; set; }
        public string? LowerExplosiveLimit { get; set; }
        public string? UpperExplosiveLimit { get; set; }

        // Section 6: Spill
        public string? SmallSpillMethods { get; set; }
        public string? LargeSpillMethods { get; set; }

        // Section 7: Handling & Storage
        public string? HandlingPrecautions { get; set; }
        public string? OccupationalHygiene { get; set; }
        public string? SafeStorageConditions { get; set; }
        public string? IncompatibleMaterials { get; set; }

        // Section 8: Exposure controls / PPE
        public string? ExposureControls { get; set; }
        public string? OccupationalExposureLimits { get; set; }
        public string? EyeProtection { get; set; }
        public string? HandProtection { get; set; }
        public string? BodyProtection { get; set; }
        public string? RespiratoryProtection { get; set; }

        // Section 9: Physical Properties
        public string? PhysicalState { get; set; }
        public string? Color { get; set; }
        public string? Odor { get; set; }
        public string? MeltingPoint { get; set; }
        public string? BoilingPoint { get; set; }
        public string? VaporPressure { get; set; }
        public string? VaporDensity { get; set; }
        public string? RelativeDensity { get; set; }
        public string? Solubility { get; set; }
        public string? OdorThreshold { get; set; }
        public string? pH { get; set; }
        public string? EvaporationRate { get; set; }
        public string? Flammability { get; set; }
        public string? MolecularWeight { get; set; }

        // Section 10: Stability
        public string? Reactivity { get; set; }
        public string? PossibilityOfHazardousReactions { get; set; }
        public string? ChemicalStability { get; set; }
        public string? ConditionsToAvoid { get; set; }
        public string? HazardousDecomposition { get; set; }

        // Section 11: Toxicological
        public string? AcuteToxicity { get; set; }
        public string? IrritationCorrosion { get; set; }
        public string? Carcinogenicity { get; set; }
        public string? Mutagenicity { get; set; }
        public string? Teratogenicity { get; set; }
        public string? RoutesOfExposure { get; set; }
        public string? ChronicHealthEffects { get; set; }

        // Section 12: Ecological
        public string? PersistenceDegradability { get; set; }
        public string? MobilityInSoil { get; set; }
        public string? OtherEcologicalEffects { get; set; }
        public string? Ecotoxicity { get; set; }
        public string? BioaccumulativePotential { get; set; }

        // Section 13: Disposal
        public string? DisposalMethods { get; set; }

        // Section 14: Transport
        public string? UNNumber { get; set; }
        public string? UNProperShippingName { get; set; }
        public string? TransportHazardClass { get; set; }
        public string? PackingGroup { get; set; }
        public string? TransportSpecialPrecautions { get; set; }
        public string? EnvironmentalHazardsTransport { get; set; }

        // Section 15: Regulatory
        public string? SARA302304 { get; set; }
        public string? InternationalRegulations { get; set; }
        public string? USFederalRegulations { get; set; }
        public string? StateRegulations { get; set; }
        public string? InventoryList { get; set; }

        // Section 16: Other
        public string? HMISRatings { get; set; }
        public string? NFPARatings { get; set; }
        public string? Version { get; set; }
        public string? DateOfRevision { get; set; }
        public string? NoticeToReader { get; set; }
        public string? DateOfPrinting { get; set; }
        public string? Abbreviations { get; set; }

        // Inventory
        public decimal Quantity { get; set; }
        public string? Unit { get; set; } = "Liters";
        public string? StorageLocation { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}