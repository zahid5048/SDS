using ChemicalSDS.Models;

namespace ChemicalSDS.Services
{
    public class SdsWizardService
    {
        public void ApplySection(Chemical target, Chemical source, int step)
        {
            switch (step)
            {
                case 1:
                    target.ProductIdentifier = source.ProductIdentifier;
                    target.ChemicalName = source.ChemicalName;
                    target.OtherMeansOfId = source.OtherMeansOfId;
                    target.ProductType = source.ProductType;
                    target.ProductUse = source.ProductUse;
                    target.Synonym = source.Synonym;
                    target.SDSNumber = source.SDSNumber;
                    target.SupplierDetails = source.SupplierDetails;
                    target.EmergencyPhone = source.EmergencyPhone;
                    break;
                case 2:
                    target.OSHASstatus = source.OSHASstatus;
                    target.Classification = source.Classification;
                    target.SignalWord = source.SignalWord;
                    target.HazardStatements = source.HazardStatements;
                    target.PrecautionaryGeneral = source.PrecautionaryGeneral;
                    target.PrecautionaryPrevention = source.PrecautionaryPrevention;
                    target.PrecautionaryResponse = source.PrecautionaryResponse;
                    target.PrecautionaryStorage = source.PrecautionaryStorage;
                    target.PrecautionaryDisposal = source.PrecautionaryDisposal;
                    target.HazardsNotOtherwiseClassified = source.HazardsNotOtherwiseClassified;
                    target.DateOfRevision = source.DateOfRevision;
                    target.DateOfPreviousIssue = source.DateOfPreviousIssue;
                    target.GhsPictogramCodes = source.GhsPictogramCodes;
                    if (!string.IsNullOrEmpty(source.GhsPictogramImagePath))
                        target.GhsPictogramImagePath = source.GhsPictogramImagePath;
                    break;
                case 3:
                    target.SubstanceMixture = source.SubstanceMixture;
                    target.ChemicalName = source.ChemicalName ?? target.ChemicalName;
                    target.OtherMeansOfId = source.OtherMeansOfId ?? target.OtherMeansOfId;
                    target.ProductCode = source.ProductCode;
                    target.CASNumber = source.CASNumber;
                    target.IngredientName = source.IngredientName;
                    target.Percentage = source.Percentage;
                    break;
                case 4:
                    target.EyeContactFirstAid = source.EyeContactFirstAid;
                    target.InhalationFirstAid = source.InhalationFirstAid;
                    target.SkinContactFirstAid = source.SkinContactFirstAid;
                    target.IngestionFirstAid = source.IngestionFirstAid;
                    target.MedicalAttentionIndication = source.MedicalAttentionIndication;
                    target.NotesToPhysician = source.NotesToPhysician;
                    target.SpecificTreatments = source.SpecificTreatments;
                    target.ProtectionOfFirstAiders = source.ProtectionOfFirstAiders;
                    break;
                case 5:
                    target.SuitableExtinguishingMedia = source.SuitableExtinguishingMedia;
                    target.UnsuitableExtinguishingMedia = source.UnsuitableExtinguishingMedia;
                    target.SpecificHazards = source.SpecificHazards;
                    target.HazardousDecomposition = source.HazardousDecomposition ?? target.HazardousDecomposition;
                    target.FlashPoint = source.FlashPoint;
                    target.AutoIgnitionTemp = source.AutoIgnitionTemp;
                    target.LowerExplosiveLimit = source.LowerExplosiveLimit;
                    target.UpperExplosiveLimit = source.UpperExplosiveLimit;
                    target.FirefighterProtectiveActions = source.FirefighterProtectiveActions;
                    target.FirefighterProtectiveEquipment = source.FirefighterProtectiveEquipment;
                    break;
                case 6:
                    target.SpillNonEmergencyPersonnel = source.SpillNonEmergencyPersonnel;
                    target.SpillEmergencyResponders = source.SpillEmergencyResponders;
                    target.EnvironmentalPrecautionsSpill = source.EnvironmentalPrecautionsSpill;
                    target.SmallSpillMethods = source.SmallSpillMethods;
                    target.LargeSpillMethods = source.LargeSpillMethods;
                    break;
                case 7:
                    target.OccupationalHygiene = source.OccupationalHygiene;
                    target.HandlingPrecautions = source.HandlingPrecautions;
                    target.SafeStorageConditions = source.SafeStorageConditions;
                    break;
                case 8:
                    target.OccupationalExposureLimits = source.OccupationalExposureLimits;
                    target.ExposureControls = source.ExposureControls;
                    target.EnvironmentalExposureControls = source.EnvironmentalExposureControls;
                    target.HygieneMeasures = source.HygieneMeasures;
                    target.EyeProtection = source.EyeProtection;
                    target.HandProtection = source.HandProtection;
                    target.BodyProtection = source.BodyProtection;
                    target.OtherSkinProtection = source.OtherSkinProtection;
                    target.RespiratoryProtection = source.RespiratoryProtection;
                    break;
                case 9:
                    target.PhysicalState = source.PhysicalState;
                    target.Color = source.Color;
                    target.Odor = source.Odor;
                    target.OdorThreshold = source.OdorThreshold;
                    target.pH = source.pH;
                    target.MeltingPoint = source.MeltingPoint;
                    target.BoilingPoint = source.BoilingPoint;
                    target.CriticalTemperature = source.CriticalTemperature;
                    target.FlashPoint = source.FlashPoint ?? target.FlashPoint;
                    target.EvaporationRate = source.EvaporationRate;
                    target.Flammability = source.Flammability;
                    target.LowerExplosiveLimit = source.LowerExplosiveLimit ?? target.LowerExplosiveLimit;
                    target.UpperExplosiveLimit = source.UpperExplosiveLimit ?? target.UpperExplosiveLimit;
                    target.VaporPressure = source.VaporPressure;
                    target.VaporDensity = source.VaporDensity;
                    target.SpecificVolume = source.SpecificVolume;
                    target.GasDensity = source.GasDensity;
                    target.RelativeDensity = source.RelativeDensity;
                    target.Solubility = source.Solubility;
                    target.SolubilityInWater = source.SolubilityInWater;
                    target.PartitionCoefficient = source.PartitionCoefficient;
                    target.AutoIgnitionTemp = source.AutoIgnitionTemp ?? target.AutoIgnitionTemp;
                    target.DecompositionTemperature = source.DecompositionTemperature;
                    target.Viscosity = source.Viscosity;
                    target.FlowTimeIso2431 = source.FlowTimeIso2431;
                    target.MolecularWeight = source.MolecularWeight;
                    break;
                case 10:
                    target.ChemicalStability = source.ChemicalStability;
                    target.ConditionsToAvoid = source.ConditionsToAvoid;
                    target.HazardousDecomposition = source.HazardousDecomposition;
                    target.IncompatibleMaterials = source.IncompatibleMaterials ?? target.IncompatibleMaterials;
                    target.PossibilityOfHazardousReactions = source.PossibilityOfHazardousReactions;
                    target.Reactivity = source.Reactivity;
                    break;
                case 11:
                    target.AcuteToxicity = source.AcuteToxicity;
                    target.IrritationCorrosion = source.IrritationCorrosion;
                    target.Sensitization = source.Sensitization;
                    target.Mutagenicity = source.Mutagenicity;
                    target.Carcinogenicity = source.Carcinogenicity;
                    target.CarcinogenicityClassification = source.CarcinogenicityClassification;
                    target.ReproductiveToxicity = source.ReproductiveToxicity;
                    target.Teratogenicity = source.Teratogenicity;
                    target.TargetOrganToxicitySingle = source.TargetOrganToxicitySingle;
                    target.TargetOrganToxicityRepeated = source.TargetOrganToxicityRepeated;
                    target.AspirationHazard = source.AspirationHazard;
                    target.RoutesOfExposure = source.RoutesOfExposure;
                    target.AcuteHealthEffectsEye = source.AcuteHealthEffectsEye;
                    target.AcuteHealthEffectsInhalation = source.AcuteHealthEffectsInhalation;
                    target.AcuteHealthEffectsSkin = source.AcuteHealthEffectsSkin;
                    target.AcuteHealthEffectsIngestion = source.AcuteHealthEffectsIngestion;
                    target.ToxicologicalSymptomsEye = source.ToxicologicalSymptomsEye;
                    target.ToxicologicalSymptomsInhalation = source.ToxicologicalSymptomsInhalation;
                    target.ToxicologicalSymptomsSkin = source.ToxicologicalSymptomsSkin;
                    target.ToxicologicalSymptomsIngestion = source.ToxicologicalSymptomsIngestion;
                    target.ShortTermImmediateEffects = source.ShortTermImmediateEffects;
                    target.ShortTermDelayedEffects = source.ShortTermDelayedEffects;
                    target.LongTermImmediateEffects = source.LongTermImmediateEffects;
                    target.LongTermDelayedEffects = source.LongTermDelayedEffects;
                    target.ChronicHealthEffects = source.ChronicHealthEffects;
                    target.ChronicEffectsGeneral = source.ChronicEffectsGeneral;
                    target.ChronicCarcinogenicity = source.ChronicCarcinogenicity;
                    target.ChronicMutagenicity = source.ChronicMutagenicity;
                    target.ChronicTeratogenicity = source.ChronicTeratogenicity;
                    target.DevelopmentalEffects = source.DevelopmentalEffects;
                    target.FertilityEffects = source.FertilityEffects;
                    target.AcuteToxicityEstimates = source.AcuteToxicityEstimates;
                    break;
                case 12:
                    target.Ecotoxicity = source.Ecotoxicity;
                    target.BioaccumulativePotential = source.BioaccumulativePotential;
                    target.PersistenceDegradability = source.PersistenceDegradability;
                    target.MobilityInSoil = source.MobilityInSoil;
                    target.OtherEcologicalEffects = source.OtherEcologicalEffects;
                    break;
                case 13:
                    target.DisposalMethods = source.DisposalMethods;
                    break;
                case 14:
                    target.TransportRegulations = source.TransportRegulations;
                    if (!string.IsNullOrEmpty(source.TransportHazardClassImages))
                        target.TransportHazardClassImages = source.TransportHazardClassImages;
                    target.UNNumber = source.UNNumber;
                    target.UNProperShippingName = source.UNProperShippingName;
                    target.TransportHazardClass = source.TransportHazardClass;
                    target.PackingGroup = source.PackingGroup;
                    target.EnvironmentalHazardsTransport = source.EnvironmentalHazardsTransport;
                    target.TransportDotClassification = source.TransportDotClassification;
                    target.TransportTdgClassification = source.TransportTdgClassification;
                    target.TransportIataClassification = source.TransportIataClassification;
                    target.TransportSpecialPrecautions = source.TransportSpecialPrecautions;
                    target.TransportBulkMarpol = source.TransportBulkMarpol;
                    break;
                case 15:
                    target.RegTscaCdrExempt = source.RegTscaCdrExempt;
                    target.RegCleanAirActHaps = source.RegCleanAirActHaps;
                    target.RegCleanAirActClassI = source.RegCleanAirActClassI;
                    target.RegCleanAirActClassII = source.RegCleanAirActClassII;
                    target.RegDeaListI = source.RegDeaListI;
                    target.RegDeaListII = source.RegDeaListII;
                    target.RegSara302Composition = source.RegSara302Composition;
                    target.RegSara304RQ = source.RegSara304RQ;
                    target.RegSara311312Classification = source.RegSara311312Classification;
                    target.RegStateMassachusetts = source.RegStateMassachusetts;
                    target.RegStateNewYork = source.RegStateNewYork;
                    target.RegStateNewJersey = source.RegStateNewJersey;
                    target.RegStatePennsylvania = source.RegStatePennsylvania;
                    target.RegChemWeaponConvention = source.RegChemWeaponConvention;
                    target.RegMontrealProtocol = source.RegMontrealProtocol;
                    target.RegStockholmConvention = source.RegStockholmConvention;
                    target.RegRotterdamConvention = source.RegRotterdamConvention;
                    target.RegUneceAarhus = source.RegUneceAarhus;
                    target.RegInventoryAustralia = source.RegInventoryAustralia;
                    target.RegInventoryCanada = source.RegInventoryCanada;
                    target.RegInventoryChina = source.RegInventoryChina;
                    target.RegInventoryEurope = source.RegInventoryEurope;
                    target.RegInventoryJapanEncs = source.RegInventoryJapanEncs;
                    target.RegInventoryJapanIshl = source.RegInventoryJapanIshl;
                    target.RegInventoryMalaysia = source.RegInventoryMalaysia;
                    target.RegInventoryNewZealand = source.RegInventoryNewZealand;
                    target.RegInventoryPhilippines = source.RegInventoryPhilippines;
                    target.RegInventoryKorea = source.RegInventoryKorea;
                    target.RegInventoryTaiwan = source.RegInventoryTaiwan;
                    target.RegInventoryThailand = source.RegInventoryThailand;
                    target.RegInventoryTurkey = source.RegInventoryTurkey;
                    target.RegInventoryUnitedStates = source.RegInventoryUnitedStates;
                    target.RegInventoryVietnam = source.RegInventoryVietnam;
                    target.SARA302304 = source.RegSara302Composition ?? source.SARA302304;
                    target.USFederalRegulations = source.USFederalRegulations;
                    target.StateRegulations = source.StateRegulations;
                    target.InternationalRegulations = source.InternationalRegulations;
                    target.InventoryList = source.InventoryList;
                    break;
                case 16:
                    target.HmisHealth = source.HmisHealth;
                    target.HmisFlammability = source.HmisFlammability;
                    target.HmisPhysicalHazards = source.HmisPhysicalHazards;
                    target.HmisCautionNote = source.HmisCautionNote;
                    target.NfpaHealth = source.NfpaHealth;
                    target.NfpaFlammability = source.NfpaFlammability;
                    target.NfpaReactivity = source.NfpaReactivity;
                    target.NfpaSpecial = source.NfpaSpecial;
                    target.NfpaCopyrightNote = source.NfpaCopyrightNote;
                    target.ClassificationProcedure = source.ClassificationProcedure ?? target.ClassificationProcedure;
                    target.ClassificationJustification = source.ClassificationJustification;
                    target.References = source.References;
                    target.HMISRatings = BuildHmisString(source);
                    target.NFPARatings = BuildNfpaString(source);
                    target.Version = source.Version;
                    target.DateOfRevision = source.DateOfRevision ?? target.DateOfRevision;
                    target.DateOfPreviousIssue = source.DateOfPreviousIssue ?? target.DateOfPreviousIssue;
                    target.DateOfPrinting = source.DateOfPrinting;
                    target.NoticeToReader = source.NoticeToReader;
                    target.Abbreviations = source.Abbreviations;
                    target.Quantity = source.Quantity;
                    target.Unit = source.Unit;
                    target.StorageLocation = source.StorageLocation;
                    break;
            }

            if (step > target.LastCompletedSection)
                target.LastCompletedSection = step;

            if (step >= SdsWizardStep.Total)
            {
                target.IsDraft = false;
                target.LastCompletedSection = SdsWizardStep.Total;
            }
        }

        public bool ValidateSection(Chemical model, int step, out string? error)
        {
            error = null;
            if (step == 1)
            {
                if (string.IsNullOrWhiteSpace(model.ProductIdentifier))
                {
                    error = "GHS product identifier is required.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(model.ChemicalName))
                {
                    error = "Chemical name is required.";
                    return false;
                }
            }
            return true;
        }

        private static string? BuildHmisString(Chemical c)
        {
            if (string.IsNullOrWhiteSpace(c.HmisHealth) && string.IsNullOrWhiteSpace(c.HmisFlammability) && string.IsNullOrWhiteSpace(c.HmisPhysicalHazards))
                return c.HMISRatings;
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.HmisHealth)) parts.Add($"Health: {c.HmisHealth.Trim()}");
            if (!string.IsNullOrWhiteSpace(c.HmisFlammability)) parts.Add($"Flammability: {c.HmisFlammability.Trim()}");
            if (!string.IsNullOrWhiteSpace(c.HmisPhysicalHazards)) parts.Add($"Physical hazards: {c.HmisPhysicalHazards.Trim()}");
            return parts.Count > 0 ? string.Join(", ", parts) : c.HMISRatings;
        }

        private static string? BuildNfpaString(Chemical c)
        {
            if (string.IsNullOrWhiteSpace(c.NfpaHealth) && string.IsNullOrWhiteSpace(c.NfpaFlammability) && string.IsNullOrWhiteSpace(c.NfpaReactivity))
                return c.NFPARatings;
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.NfpaHealth)) parts.Add($"Health: {c.NfpaHealth.Trim()}");
            if (!string.IsNullOrWhiteSpace(c.NfpaFlammability)) parts.Add($"Flammability: {c.NfpaFlammability.Trim()}");
            if (!string.IsNullOrWhiteSpace(c.NfpaReactivity)) parts.Add($"Instability/Reactivity: {c.NfpaReactivity.Trim()}");
            return parts.Count > 0 ? string.Join(", ", parts) : c.NFPARatings;
        }
    }
}
