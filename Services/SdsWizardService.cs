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
                    Sec01SubsectionCodec.SyncProductIdentifier(target, source.Sec01Subsections);
                    if (string.IsNullOrWhiteSpace(target.ProductIdentifier) &&
                        !string.IsNullOrWhiteSpace(source.ProductIdentifier))
                    {
                        var label = Sec01SubsectionCodec.ShortProductLabel(source.ProductIdentifier);
                        target.ProductIdentifier = string.IsNullOrWhiteSpace(label) ? null : label;
                    }
                    break;
                case 2:
                    target.Sec02Blocks = source.Sec02Blocks;
                    break;
                case 3:
                    target.Sec03Blocks = source.Sec03Blocks;
                    break;
                case 4:
                    target.Sec04Subsections = source.Sec04Subsections;
                    break;
                case 5:
                    target.Sec05Subsections = source.Sec05Subsections;
                    break;
                case 6:
                    target.Sec06Subsections = source.Sec06Subsections;
                    break;
                case 7:
                    target.Sec07Subsections = source.Sec07Subsections;
                    break;
                case 8:
                    target.Sec08Subsections = source.Sec08Subsections;
                    break;
                case 9:
                    target.Sec09Subsections = source.Sec09Subsections;
                    break;
                case 10:
                    target.Sec10Subsections = source.Sec10Subsections;
                    break;
                case 11:
                    target.Sec11Subsections = source.Sec11Subsections;
                    break;
                case 12:
                    target.Sec12Subsections = source.Sec12Subsections;
                    break;
                case 13:
                    target.Sec13Subsections = source.Sec13Subsections;
                    break;
                case 14:
                    target.Sec14Subsections = source.Sec14Subsections;
                    break;
                case 15:
                    target.Sec15Subsections = source.Sec15Subsections;
                    break;
                case 16:
                    target.Sec16Subsections = source.Sec16Subsections;
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
                Sec01SubsectionCodec.SyncProductIdentifier(model, model.Sec01Subsections);
                if (string.IsNullOrWhiteSpace(model.ProductIdentifier))
                {
                    error = "Section 1.1 Product identifier (description) is required.";
                    return false;
                }
            }
            return true;
        }
    }
}
