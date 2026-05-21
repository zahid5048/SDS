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
        /// <summary>Comma-separated GHS pictogram codes (GHS01–GHS09).</summary>
        public string? GhsPictogramCodes { get; set; }
        /// <summary>Uploaded GHS label / pictogram image (relative wwwroot path).</summary>
        public string? GhsPictogramImagePath { get; set; }

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
        public string? MedicalAttentionIndication { get; set; }
        public string? NotesToPhysician { get; set; }
        public string? SpecificTreatments { get; set; }
        public string? ProtectionOfFirstAiders { get; set; }

        // Section 5: Fire Fighting
        public string? SuitableExtinguishingMedia { get; set; }
        public string? UnsuitableExtinguishingMedia { get; set; }
        public string? SpecificHazards { get; set; }
        public string? FlashPoint { get; set; }
        public string? AutoIgnitionTemp { get; set; }
        public string? LowerExplosiveLimit { get; set; }
        public string? UpperExplosiveLimit { get; set; }
        public string? FirefighterProtectiveActions { get; set; }
        public string? FirefighterProtectiveEquipment { get; set; }

        // Section 6: Accidental release
        public string? SpillNonEmergencyPersonnel { get; set; }
        public string? SpillEmergencyResponders { get; set; }
        public string? EnvironmentalPrecautionsSpill { get; set; }
        public string? SmallSpillMethods { get; set; }
        public string? LargeSpillMethods { get; set; }

        // Section 7: Handling & Storage
        public string? HandlingPrecautions { get; set; }
        public string? OccupationalHygiene { get; set; }
        public string? SafeStorageConditions { get; set; }
        public string? IncompatibleMaterials { get; set; }

        // Section 8: Exposure controls / PPE
        public string? OccupationalExposureLimits { get; set; }
        public string? ExposureControls { get; set; }
        public string? EnvironmentalExposureControls { get; set; }
        public string? HygieneMeasures { get; set; }
        public string? EyeProtection { get; set; }
        public string? HandProtection { get; set; }
        public string? BodyProtection { get; set; }
        public string? OtherSkinProtection { get; set; }
        public string? RespiratoryProtection { get; set; }

        // Section 9: Physical Properties
        public string? PhysicalState { get; set; }
        public string? Color { get; set; }
        public string? Odor { get; set; }
        public string? OdorThreshold { get; set; }
        public string? pH { get; set; }
        public string? MeltingPoint { get; set; }
        public string? BoilingPoint { get; set; }
        public string? CriticalTemperature { get; set; }
        public string? EvaporationRate { get; set; }
        public string? Flammability { get; set; }
        public string? VaporPressure { get; set; }
        public string? VaporDensity { get; set; }
        public string? SpecificVolume { get; set; }
        public string? GasDensity { get; set; }
        public string? RelativeDensity { get; set; }
        public string? Solubility { get; set; }
        public string? SolubilityInWater { get; set; }
        public string? PartitionCoefficient { get; set; }
        public string? DecompositionTemperature { get; set; }
        public string? Viscosity { get; set; }
        public string? FlowTimeIso2431 { get; set; }
        public string? MolecularWeight { get; set; }

        // Section 10: Stability
        public string? Reactivity { get; set; }
        public string? PossibilityOfHazardousReactions { get; set; }
        public string? ChemicalStability { get; set; }
        public string? ConditionsToAvoid { get; set; }
        public string? HazardousDecomposition { get; set; }

        // Section 11: Toxicological
        public string? AcuteToxicity { get; set; }
        /// <summary>Rows: ingredient | result | species | score | exposure | observation</summary>
        public string? IrritationCorrosion { get; set; }
        public string? Sensitization { get; set; }
        public string? Mutagenicity { get; set; }
        public string? Carcinogenicity { get; set; }
        /// <summary>Rows: ingredient | OSHA | IARC | NTP</summary>
        public string? CarcinogenicityClassification { get; set; }
        public string? ReproductiveToxicity { get; set; }
        public string? Teratogenicity { get; set; }
        public string? TargetOrganToxicitySingle { get; set; }
        public string? TargetOrganToxicityRepeated { get; set; }
        public string? AspirationHazard { get; set; }
        public string? RoutesOfExposure { get; set; }
        public string? AcuteHealthEffectsEye { get; set; }
        public string? AcuteHealthEffectsInhalation { get; set; }
        public string? AcuteHealthEffectsSkin { get; set; }
        public string? AcuteHealthEffectsIngestion { get; set; }
        public string? ToxicologicalSymptomsEye { get; set; }
        public string? ToxicologicalSymptomsInhalation { get; set; }
        public string? ToxicologicalSymptomsSkin { get; set; }
        public string? ToxicologicalSymptomsIngestion { get; set; }
        public string? ShortTermImmediateEffects { get; set; }
        public string? ShortTermDelayedEffects { get; set; }
        public string? LongTermImmediateEffects { get; set; }
        public string? LongTermDelayedEffects { get; set; }
        public string? ChronicHealthEffects { get; set; }
        public string? ChronicEffectsGeneral { get; set; }
        public string? ChronicCarcinogenicity { get; set; }
        public string? ChronicMutagenicity { get; set; }
        public string? ChronicTeratogenicity { get; set; }
        public string? DevelopmentalEffects { get; set; }
        public string? FertilityEffects { get; set; }
        public string? AcuteToxicityEstimates { get; set; }

        // Section 12: Ecological
        /// <summary>Rows: ingredient | result | species | exposure</summary>
        public string? Ecotoxicity { get; set; }
        public string? PersistenceDegradability { get; set; }
        /// <summary>Rows: ingredient | LogPow | BCF | potential</summary>
        public string? BioaccumulativePotential { get; set; }
        public string? MobilityInSoil { get; set; }
        public string? OtherEcologicalEffects { get; set; }

        // Section 13: Disposal
        public string? DisposalMethods { get; set; }

        // Section 14: Transport
        /// <summary>Rows: field | DOT | TDG | Mexico | IMDG | IATA</summary>
        public string? TransportRegulations { get; set; }
        /// <summary>JSON: authority → custom pictogram path (e.g. DOT, TDG, Mexico, IMDG, IATA)</summary>
        public string? TransportHazardClassImages { get; set; }
        public string? UNNumber { get; set; }
        public string? UNProperShippingName { get; set; }
        public string? TransportHazardClass { get; set; }
        public string? PackingGroup { get; set; }
        public string? EnvironmentalHazardsTransport { get; set; }
        public string? TransportDotClassification { get; set; }
        public string? TransportTdgClassification { get; set; }
        public string? TransportIataClassification { get; set; }
        public string? TransportSpecialPrecautions { get; set; }
        public string? TransportBulkMarpol { get; set; }

        // Section 15: Regulatory
        public string? RegTscaCdrExempt { get; set; }
        public string? RegCleanAirActHaps { get; set; }
        public string? RegCleanAirActClassI { get; set; }
        public string? RegCleanAirActClassII { get; set; }
        public string? RegDeaListI { get; set; }
        public string? RegDeaListII { get; set; }
        public string? RegSara302Composition { get; set; }
        public string? RegSara304RQ { get; set; }
        public string? RegSara311312Classification { get; set; }
        public string? RegStateMassachusetts { get; set; }
        public string? RegStateNewYork { get; set; }
        public string? RegStateNewJersey { get; set; }
        public string? RegStatePennsylvania { get; set; }
        public string? RegChemWeaponConvention { get; set; }
        public string? RegMontrealProtocol { get; set; }
        public string? RegStockholmConvention { get; set; }
        public string? RegRotterdamConvention { get; set; }
        public string? RegUneceAarhus { get; set; }
        public string? RegInventoryAustralia { get; set; }
        public string? RegInventoryCanada { get; set; }
        public string? RegInventoryChina { get; set; }
        public string? RegInventoryEurope { get; set; }
        public string? RegInventoryJapanEncs { get; set; }
        public string? RegInventoryJapanIshl { get; set; }
        public string? RegInventoryMalaysia { get; set; }
        public string? RegInventoryNewZealand { get; set; }
        public string? RegInventoryPhilippines { get; set; }
        public string? RegInventoryKorea { get; set; }
        public string? RegInventoryTaiwan { get; set; }
        public string? RegInventoryThailand { get; set; }
        public string? RegInventoryTurkey { get; set; }
        public string? RegInventoryUnitedStates { get; set; }
        public string? RegInventoryVietnam { get; set; }
        public string? SARA302304 { get; set; }
        public string? InternationalRegulations { get; set; }
        public string? USFederalRegulations { get; set; }
        public string? StateRegulations { get; set; }
        public string? InventoryList { get; set; }

        // Section 16: Other
        public string? HmisHealth { get; set; }
        public string? HmisFlammability { get; set; }
        public string? HmisPhysicalHazards { get; set; }
        public string? HmisCautionNote { get; set; }
        public string? NfpaHealth { get; set; }
        public string? NfpaFlammability { get; set; }
        public string? NfpaReactivity { get; set; }
        public string? NfpaSpecial { get; set; }
        public string? NfpaCopyrightNote { get; set; }
        public string? ClassificationProcedure { get; set; }
        public string? ClassificationJustification { get; set; }
        public string? References { get; set; }
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

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}