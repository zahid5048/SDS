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
                    target.Sec01Subsections = source.Sec01Subsections;
                    Sec01SubsectionCodec.SyncLegacyFields(target, source.Sec01Subsections);
                    if (string.IsNullOrWhiteSpace(target.ProductIdentifier) &&
                        !string.IsNullOrWhiteSpace(source.ProductIdentifier))
                    {
                        var label = Sec01SubsectionCodec.ShortProductLabel(source.ProductIdentifier);
                        target.ProductIdentifier = string.IsNullOrWhiteSpace(label) ? null : label;
                    }
                    if (!string.IsNullOrWhiteSpace(target.ProductIdentifier))
                    {
                        target.ChemicalName = string.IsNullOrWhiteSpace(source.ChemicalName)
                            ? target.ProductIdentifier.Trim()
                            : source.ChemicalName;
                    }
                    break;
                case 2:
                    target.Sec02Blocks = source.Sec02Blocks;
                    Sec02BlockCodec.SyncLegacyFields(target, source.Sec02Blocks);
                    if (!string.IsNullOrWhiteSpace(source.GhsPictogramCodes))
                        target.GhsPictogramCodes = source.GhsPictogramCodes;
                    target.DateOfRevision = source.DateOfRevision ?? target.DateOfRevision;
                    target.DateOfPreviousIssue = source.DateOfPreviousIssue ?? target.DateOfPreviousIssue;
                    if (!string.IsNullOrEmpty(source.GhsPictogramImagePath))
                        target.GhsPictogramImagePath = source.GhsPictogramImagePath;
                    break;
                case 3:
                    target.Sec03Blocks = source.Sec03Blocks;
                    Sec03BlockCodec.SyncLegacyFields(target, source.Sec03Blocks);
                    target.ChemicalName = source.ChemicalName ?? target.ChemicalName;
                    target.ProductCode = source.ProductCode ?? target.ProductCode;
                    break;
                case 4:
                    target.Sec04Subsections = source.Sec04Subsections;
                    Sec04SubsectionCodec.SyncLegacyFields(target, source.Sec04Subsections);
                    break;
                case 5:
                    target.Sec05Subsections = source.Sec05Subsections;
                    Sec05SubsectionCodec.SyncLegacyFields(target, source.Sec05Subsections);
                    break;
                case 6:
                    target.Sec06Subsections = source.Sec06Subsections;
                    Sec06SubsectionCodec.SyncLegacyFields(target, source.Sec06Subsections);
                    break;
                case 7:
                    target.Sec07Subsections = source.Sec07Subsections;
                    Sec07SubsectionCodec.SyncLegacyFields(target, source.Sec07Subsections);
                    break;
                case 8:
                    target.Sec08Subsections = source.Sec08Subsections;
                    Sec08SubsectionCodec.SyncLegacyFields(target, source.Sec08Subsections);
                    break;
                case 9:
                    target.Sec09Subsections = source.Sec09Subsections;
                    Sec09SubsectionCodec.SyncLegacyFields(target, source.Sec09Subsections);
                    break;
                case 10:
                    target.Sec10Subsections = source.Sec10Subsections;
                    Sec10SubsectionCodec.SyncLegacyFields(target, source.Sec10Subsections);
                    break;
                case 11:
                    target.Sec11Subsections = source.Sec11Subsections;
                    Sec11SubsectionCodec.SyncLegacyFields(target, source.Sec11Subsections);
                    break;
                case 12:
                    target.Sec12Subsections = source.Sec12Subsections;
                    Sec12SubsectionCodec.SyncLegacyFields(target, source.Sec12Subsections);
                    break;
                case 13:
                    target.Sec13Subsections = source.Sec13Subsections;
                    Sec13SubsectionCodec.SyncLegacyFields(target, source.Sec13Subsections);
                    break;
                case 14:
                    target.Sec14Subsections = source.Sec14Subsections;
                    Sec14SubsectionCodec.SyncLegacyFields(target, source.Sec14Subsections);
                    if (!string.IsNullOrEmpty(source.TransportHazardClassImages))
                        target.TransportHazardClassImages = source.TransportHazardClassImages;
                    break;
                case 15:
                    target.Sec15Subsections = source.Sec15Subsections;
                    Sec15SubsectionCodec.SyncLegacyFields(target, source.Sec15Subsections);
                    break;
                case 16:
                    target.Sec16Subsections = source.Sec16Subsections;
                    Sec16SubsectionCodec.SyncLegacyFields(target, source.Sec16Subsections);
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
                Sec01SubsectionCodec.SyncLegacyFields(model, model.Sec01Subsections);
                if (string.IsNullOrWhiteSpace(model.ProductIdentifier))
                {
                    error = "Section 1.1 Product identifier (description) is required.";
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
