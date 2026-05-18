using ChemicalSDS.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ChemicalSDS.Services
{
    public class SdsPdfGenerator
    {
        static SdsPdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] Generate(Chemical c)
        {
            var title = c.ProductIdentifier ?? c.ChemicalName ?? "Chemical";

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Background("#0d5c2e").Padding(12).Row(row =>
                        {
                            row.RelativeItem().Column(inner =>
                            {
                                inner.Item().Text("SAFETY DATA SHEET").FontSize(16).Bold().FontColor(Colors.White);
                                inner.Item().Text("C.I.W.C&E — DGJW Govt of Punjab").FontSize(9).FontColor("#c8e6c9");
                            });
                            row.ConstantItem(120).AlignRight().Column(inner =>
                            {
                                inner.Item().Text(title).FontSize(12).Bold().FontColor(Colors.White);
                                inner.Item().Text($"SDS #{c.SDSNumber}").FontSize(8).FontColor("#c8e6c9");
                            });
                        });
                    });

                    page.Content().PaddingTop(12).Column(col =>
                    {
                        Section(col, "1", "Identification",
                            Row("GHS Product Identifier", c.ProductIdentifier),
                            Row("Chemical Name", c.ChemicalName),
                            Row("Other Means of ID", c.OtherMeansOfId),
                            Row("Product Type", c.ProductType),
                            Row("Product Use", c.ProductUse),
                            Row("Synonym", c.Synonym),
                            Row("SDS #", c.SDSNumber),
                            Row("Supplier", c.SupplierDetails),
                            Row("24-Hour Phone", c.EmergencyPhone));

                        Section(col, "2", "Hazards Identification",
                            Row("OSHA/HCS Status", c.OSHASstatus),
                            Row("Classification", c.Classification),
                            Row("Signal Word", c.SignalWord),
                            Row("Hazard Statements", c.HazardStatements),
                            Row("Precautionary — General", c.PrecautionaryGeneral),
                            Row("Precautionary — Prevention", c.PrecautionaryPrevention),
                            Row("Precautionary — Response", c.PrecautionaryResponse),
                            Row("Precautionary — Storage", c.PrecautionaryStorage),
                            Row("Precautionary — Disposal", c.PrecautionaryDisposal));

                        Section(col, "3", "Composition / Ingredients",
                            Row("Substance/Mixture", c.SubstanceMixture),
                            Row("CAS Number", c.CASNumber),
                            Row("Ingredient", c.IngredientName),
                            Row("Percentage", c.Percentage));

                        Section(col, "4", "First Aid Measures",
                            Row("Eye Contact", c.EyeContactFirstAid),
                            Row("Inhalation", c.InhalationFirstAid),
                            Row("Skin Contact", c.SkinContactFirstAid),
                            Row("Ingestion", c.IngestionFirstAid));

                        Section(col, "5", "Fire-Fighting Measures",
                            Row("Suitable Extinguishing Media", c.SuitableExtinguishingMedia),
                            Row("Unsuitable Media", c.UnsuitableExtinguishingMedia),
                            Row("Specific Hazards", c.SpecificHazards),
                            Row("Flash Point", c.FlashPoint),
                            Row("Auto-Ignition Temp", c.AutoIgnitionTemp),
                            Row("Explosive Limits", $"{c.LowerExplosiveLimit} - {c.UpperExplosiveLimit}"));

                        Section(col, "6", "Accidental Release",
                            Row("Small Spill", c.SmallSpillMethods),
                            Row("Large Spill", c.LargeSpillMethods));

                        Section(col, "7", "Handling & Storage",
                            Row("Handling Precautions", c.HandlingPrecautions),
                            Row("Safe Storage", c.SafeStorageConditions),
                            Row("Incompatible Materials", c.IncompatibleMaterials));

                        Section(col, "8", "Exposure Controls / PPE",
                            Row("Exposure Limits", c.OccupationalExposureLimits),
                            Row("Engineering Controls", c.ExposureControls),
                            Row("Eye Protection", c.EyeProtection),
                            Row("Hand Protection", c.HandProtection),
                            Row("Body Protection", c.BodyProtection),
                            Row("Respiratory Protection", c.RespiratoryProtection));

                        Section(col, "9", "Physical & Chemical Properties",
                            Row("Physical State", c.PhysicalState),
                            Row("Color", c.Color),
                            Row("Odor", c.Odor),
                            Row("Melting Point", c.MeltingPoint),
                            Row("Boiling Point", c.BoilingPoint),
                            Row("Flash Point", c.FlashPoint),
                            Row("Vapor Pressure", c.VaporPressure),
                            Row("Relative Density", c.RelativeDensity),
                            Row("Solubility", c.Solubility));

                        Section(col, "10", "Stability & Reactivity",
                            Row("Chemical Stability", c.ChemicalStability),
                            Row("Conditions to Avoid", c.ConditionsToAvoid),
                            Row("Hazardous Decomposition", c.HazardousDecomposition));

                        Section(col, "11", "Toxicological Information",
                            Row("Acute Toxicity", c.AcuteToxicity),
                            Row("Irritation/Corrosion", c.IrritationCorrosion),
                            Row("Carcinogenicity", c.Carcinogenicity));

                        Section(col, "12", "Ecological Information",
                            Row("Ecotoxicity", c.Ecotoxicity),
                            Row("Bioaccumulative Potential", c.BioaccumulativePotential));

                        Section(col, "13", "Disposal Considerations",
                            Row("Disposal Methods", c.DisposalMethods));

                        Section(col, "14", "Transport Information",
                            Row("UN Number", c.UNNumber),
                            Row("Shipping Name", c.UNProperShippingName),
                            Row("Hazard Class", c.TransportHazardClass),
                            Row("Packing Group", c.PackingGroup));

                        Section(col, "15", "Regulatory Information",
                            Row("U.S. Federal", c.USFederalRegulations),
                            Row("State Regulations", c.StateRegulations),
                            Row("Inventory List", c.InventoryList));

                        Section(col, "16", "Other Information",
                            Row("HMIS Ratings", c.HMISRatings),
                            Row("NFPA Ratings", c.NFPARatings),
                            Row("Version", c.Version),
                            Row("Date of Revision", c.DateOfRevision),
                            Row("Notice to Reader", c.NoticeToReader));
                    });

                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("Generated by Chemical SDS System — Punjab | ").FontSize(8).FontColor(Colors.Grey.Medium);
                        t.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        private static void Section(ColumnDescriptor col, string num, string title, params (string Label, string? Value)[] rows)
        {
            col.Item().PaddingTop(8).Column(sec =>
            {
                sec.Item().Background("#1b8a4a").Padding(6).Text($"Section {num}. {title}")
                    .FontSize(10).Bold().FontColor(Colors.White);
                sec.Item().Border(1).BorderColor("#e0e0e0").Padding(8).Column(body =>
                {
                    foreach (var (label, value) in rows)
                    {
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        body.Item().PaddingBottom(4).Row(r =>
                        {
                            r.ConstantItem(140).Text(label).Bold().FontSize(8);
                            r.RelativeItem().Text(value).FontSize(8);
                        });
                    }
                });
            });
        }

        private static (string Label, string? Value) Row(string label, string? value) => (label, value);
    }
}
