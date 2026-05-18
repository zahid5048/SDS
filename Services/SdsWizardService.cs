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
                    target.RoutesOfExposure = source.RoutesOfExposure;
                    target.ChronicHealthEffects = source.ChronicHealthEffects;
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
                    break;
                case 6:
                    target.SmallSpillMethods = source.SmallSpillMethods;
                    target.LargeSpillMethods = source.LargeSpillMethods;
                    break;
                case 7:
                    target.OccupationalHygiene = source.OccupationalHygiene;
                    target.HandlingPrecautions = source.HandlingPrecautions;
                    target.SafeStorageConditions = source.SafeStorageConditions;
                    target.IncompatibleMaterials = source.IncompatibleMaterials;
                    break;
                case 8:
                    target.OccupationalExposureLimits = source.OccupationalExposureLimits;
                    target.ExposureControls = source.ExposureControls;
                    target.EyeProtection = source.EyeProtection;
                    target.HandProtection = source.HandProtection;
                    target.BodyProtection = source.BodyProtection;
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
                    target.FlashPoint = source.FlashPoint ?? target.FlashPoint;
                    target.EvaporationRate = source.EvaporationRate;
                    target.Flammability = source.Flammability;
                    target.LowerExplosiveLimit = source.LowerExplosiveLimit ?? target.LowerExplosiveLimit;
                    target.UpperExplosiveLimit = source.UpperExplosiveLimit ?? target.UpperExplosiveLimit;
                    target.VaporPressure = source.VaporPressure;
                    target.VaporDensity = source.VaporDensity;
                    target.RelativeDensity = source.RelativeDensity;
                    target.Solubility = source.Solubility;
                    target.AutoIgnitionTemp = source.AutoIgnitionTemp ?? target.AutoIgnitionTemp;
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
                    target.Carcinogenicity = source.Carcinogenicity;
                    target.Mutagenicity = source.Mutagenicity;
                    target.Teratogenicity = source.Teratogenicity;
                    target.RoutesOfExposure = source.RoutesOfExposure ?? target.RoutesOfExposure;
                    target.ChronicHealthEffects = source.ChronicHealthEffects ?? target.ChronicHealthEffects;
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
                    target.UNNumber = source.UNNumber;
                    target.UNProperShippingName = source.UNProperShippingName;
                    target.TransportHazardClass = source.TransportHazardClass;
                    target.PackingGroup = source.PackingGroup;
                    target.EnvironmentalHazardsTransport = source.EnvironmentalHazardsTransport;
                    target.TransportSpecialPrecautions = source.TransportSpecialPrecautions;
                    break;
                case 15:
                    target.USFederalRegulations = source.USFederalRegulations;
                    target.SARA302304 = source.SARA302304;
                    target.StateRegulations = source.StateRegulations;
                    target.InternationalRegulations = source.InternationalRegulations;
                    target.InventoryList = source.InventoryList;
                    break;
                case 16:
                    target.HMISRatings = source.HMISRatings;
                    target.NFPARatings = source.NFPARatings;
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
    }
}
