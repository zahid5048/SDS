using ChemicalSDS.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ChemicalSDS.Services
{
    public class SdsPdfGenerator
    {
        private readonly string? _webRoot;

        static SdsPdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public SdsPdfGenerator(IWebHostEnvironment? env = null)
        {
            _webRoot = env?.WebRootPath;
        }

        public byte[] Generate(Chemical c)
        {
            var title = c.ProductIdentifier ?? c.ChemicalName ?? "Chemical";

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(32);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Header().Element(h => RenderHeader(h, title, c.SDSNumber));
                    page.Footer().Element(RenderFooter);

                    page.Content().PaddingTop(8).Column(col =>
                    {
                        RenderSection1(col, c);
                        RenderSection2(col, c);
                        RenderSection3(col, c);
                        RenderSection4(col, c);
                        RenderSection5(col, c);
                        RenderSection6(col, c);
                        RenderSection7(col, c);
                        RenderSection8(col, c);
                        RenderSection9(col, c);
                        RenderSection10(col, c);
                        RenderSection11(col, c);
                        RenderSection12(col, c);
                        RenderSection13(col, c);
                        RenderSection14(col, c);
                        RenderSection15(col, c);
                        RenderSection16(col, c);
                    });
                });
            }).GeneratePdf();
        }

        private void RenderHeader(IContainer container, string title, string? sdsNumber)
        {
            container.Background("#0d5c2e").Padding(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SAFETY DATA SHEET").FontSize(14).Bold().FontColor(Colors.White);
                    col.Item().Text("C.I.W.C&E — DGJW Govt of Punjab").FontSize(8).FontColor("#c8e6c9");
                });
                row.ConstantItem(130).AlignRight().Column(col =>
                {
                    col.Item().Text(title).FontSize(11).Bold().FontColor(Colors.White);
                    if (!string.IsNullOrWhiteSpace(sdsNumber))
                        col.Item().Text($"SDS #{sdsNumber}").FontSize(8).FontColor("#c8e6c9");
                });
            });
        }

        private void RenderFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(t =>
                {
                    t.Span("Chemical SDS System — Punjab | ").FontSize(7).FontColor(Colors.Grey.Medium);
                    t.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontSize(7).FontColor(Colors.Grey.Medium);
                });
                row.RelativeItem().AlignRight().Text(t =>
                {
                    t.Span("Page ").FontSize(7).FontColor(Colors.Grey.Medium);
                    t.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Medium);
                    t.Span(" of ").FontSize(7).FontColor(Colors.Grey.Medium);
                    t.TotalPages().FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private static void RenderSection1(ColumnDescriptor col, Chemical c) =>
            Section(col, "1", "Identification",
                Row("GHS Product Identifier", c.ProductIdentifier),
                Row("Chemical Name", c.ChemicalName),
                Row("Other Means of Identification", c.OtherMeansOfId),
                Row("Product Type", c.ProductType),
                Row("Product Use", c.ProductUse),
                Row("Synonym", c.Synonym),
                Row("SDS #", c.SDSNumber),
                Row("Supplier Details", c.SupplierDetails),
                Row("24-Hour Emergency Phone", c.EmergencyPhone));

        private void RenderSection2(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "2", "Hazards identification", body =>
            {
                AddRows(body, c,
                    Row("OSHA/HCS status", c.OSHASstatus),
                    Row("Classification", c.Classification),
                    Row("Signal word", c.SignalWord),
                    Row("Hazards not otherwise classified", c.HazardsNotOtherwiseClassified));

                RenderGhsPictograms(body, c);

                AddRows(body, c,
                    Row("Hazard statements", c.HazardStatements),
                    Row("Precautionary — General", c.PrecautionaryGeneral),
                    Row("Precautionary — Prevention", c.PrecautionaryPrevention),
                    Row("Precautionary — Response", c.PrecautionaryResponse),
                    Row("Precautionary — Storage", c.PrecautionaryStorage),
                    Row("Precautionary — Disposal", c.PrecautionaryDisposal));
            });
        }

        private static void RenderSection3(ColumnDescriptor col, Chemical c) =>
            Section(col, "3", "Composition / information on ingredients",
                Row("Substance/Mixture", c.SubstanceMixture),
                Row("Product code", c.ProductCode),
                Row("Ingredient name", c.IngredientName),
                Row("CAS number", c.CASNumber),
                Row("Percentage", c.Percentage));

        private static void RenderSection4(ColumnDescriptor col, Chemical c) =>
            Section(col, "4", "First aid measures",
                Row("Eye contact", c.EyeContactFirstAid),
                Row("Inhalation", c.InhalationFirstAid),
                Row("Skin contact", c.SkinContactFirstAid),
                Row("Ingestion", c.IngestionFirstAid),
                Row("Indication of immediate medical attention and special treatment needed", c.MedicalAttentionIndication),
                Row("Notes to physician", c.NotesToPhysician),
                Row("Specific treatments", c.SpecificTreatments),
                Row("Protection of first-aiders", c.ProtectionOfFirstAiders));

        private static void RenderSection5(ColumnDescriptor col, Chemical c) =>
            Section(col, "5", "Fire-fighting measures",
                Row("Suitable extinguishing media", c.SuitableExtinguishingMedia),
                Row("Unsuitable extinguishing media", c.UnsuitableExtinguishingMedia),
                Row("Specific hazards arising from the chemical", c.SpecificHazards),
                Row("Hazardous thermal decomposition products", c.HazardousDecomposition),
                Row("Special protective actions for fire-fighters", c.FirefighterProtectiveActions),
                Row("Special protective equipment for fire-fighters", c.FirefighterProtectiveEquipment),
                Row("Flash point", c.FlashPoint),
                Row("Auto-ignition temperature", c.AutoIgnitionTemp),
                Row("Lower explosive limit", c.LowerExplosiveLimit),
                Row("Upper explosive limit", c.UpperExplosiveLimit));

        private static void RenderSection6(ColumnDescriptor col, Chemical c) =>
            Section(col, "6", "Accidental release measures",
                Row("For non-emergency personnel", c.SpillNonEmergencyPersonnel),
                Row("For emergency responders", c.SpillEmergencyResponders),
                Row("Environmental precautions", c.EnvironmentalPrecautionsSpill),
                Row("Small spill", c.SmallSpillMethods),
                Row("Large spill", c.LargeSpillMethods));

        private static void RenderSection7(ColumnDescriptor col, Chemical c) =>
            Section(col, "7", "Handling and storage",
                Row("Precautions for safe handling", c.HandlingPrecautions),
                Row("Advice on general occupational hygiene", c.OccupationalHygiene),
                Row("Conditions for safe storage", c.SafeStorageConditions));

        private void RenderSection8(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "8", "Exposure controls / personal protection", body =>
            {
                AddOelTable(body, "Occupational exposure limits", c.OccupationalExposureLimits);

                AddRows(body, c,
                    Row("Appropriate engineering controls", c.ExposureControls),
                    Row("Environmental exposure controls", c.EnvironmentalExposureControls),
                    Row("Hygiene measures", c.HygieneMeasures),
                    Row("Eye / face protection", c.EyeProtection),
                    Row("Hand protection", c.HandProtection),
                    Row("Body protection", c.BodyProtection),
                    Row("Other skin protection", c.OtherSkinProtection),
                    Row("Respiratory protection", c.RespiratoryProtection));
            });
        }

        private static void RenderSection9(ColumnDescriptor col, Chemical c) =>
            Section(col, "9", "Physical and chemical properties",
                Row("Physical state", c.PhysicalState),
                Row("Color", c.Color),
                Row("Odor", c.Odor),
                Row("Odor threshold", c.OdorThreshold),
                Row("pH", c.pH),
                Row("Melting point / freezing point", c.MeltingPoint),
                Row("Boiling point", c.BoilingPoint),
                Row("Critical temperature", c.CriticalTemperature),
                Row("Flash point", c.FlashPoint),
                Row("Evaporation rate", c.EvaporationRate),
                Row("Flammability (solid, gas)", c.Flammability),
                Row("Lower explosive (flammable) limit", c.LowerExplosiveLimit),
                Row("Upper explosive (flammable) limit", c.UpperExplosiveLimit),
                Row("Vapor pressure", c.VaporPressure),
                Row("Vapor density", c.VaporDensity),
                Row("Specific volume (ft³/lb)", c.SpecificVolume),
                Row("Gas density (lb/ft³)", c.GasDensity),
                Row("Relative density", c.RelativeDensity),
                Row("Solubility", c.Solubility),
                Row("Solubility in water", c.SolubilityInWater),
                Row("Partition coefficient: n-octanol/water", c.PartitionCoefficient),
                Row("Auto-ignition temperature", c.AutoIgnitionTemp),
                Row("Decomposition temperature", c.DecompositionTemperature),
                Row("Viscosity", c.Viscosity),
                Row("Flow time (ISO 2431)", c.FlowTimeIso2431),
                Row("Molecular weight", c.MolecularWeight));

        private static void RenderSection10(ColumnDescriptor col, Chemical c) =>
            Section(col, "10", "Stability and reactivity",
                Row("Reactivity", c.Reactivity),
                Row("Possibility of hazardous reactions", c.PossibilityOfHazardousReactions),
                Row("Chemical stability", c.ChemicalStability),
                Row("Conditions to avoid", c.ConditionsToAvoid),
                Row("Incompatible materials", c.IncompatibleMaterials),
                Row("Hazardous decomposition products", c.HazardousDecomposition),
                Row("Hazardous polymerization", c.HazardousPolymerization));

        private void RenderSection11(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "11", "Toxicological information", body =>
            {
                AddRows(body, c, Row("Acute toxicity", c.AcuteToxicity));

                if (!string.IsNullOrWhiteSpace(c.IrritationCorrosion) && c.IrritationCorrosion.Contains('|'))
                    AddPipeTable(body, "Irritation/Corrosion",
                        ["Product/ingredient name", "Result", "Species", "Score", "Exposure", "Observation"], c.IrritationCorrosion);
                else
                    AddRow(body, "Irritation/Corrosion", c.IrritationCorrosion);

                AddRows(body, c,
                    Row("Sensitization", c.Sensitization),
                    Row("Mutagenicity", c.Mutagenicity),
                    Row("Carcinogenicity", c.Carcinogenicity));

                if (!string.IsNullOrWhiteSpace(c.CarcinogenicityClassification) && c.CarcinogenicityClassification.Contains('|'))
                    AddPipeTable(body, "Classification",
                        ["Product/ingredient name", "OSHA", "IARC", "NTP"], c.CarcinogenicityClassification);

                AddRows(body, c,
                    Row("Reproductive toxicity", c.ReproductiveToxicity),
                    Row("Teratogenicity", c.Teratogenicity),
                    Row("Specific target organ toxicity (single exposure)", c.TargetOrganToxicitySingle),
                    Row("Specific target organ toxicity (repeated exposure)", c.TargetOrganToxicityRepeated),
                    Row("Aspiration hazard", c.AspirationHazard),
                    Row("Routes of exposure", c.RoutesOfExposure),
                    Row("Potential acute health effects — Eye", c.AcuteHealthEffectsEye),
                    Row("Potential acute health effects — Inhalation", c.AcuteHealthEffectsInhalation),
                    Row("Potential acute health effects — Skin", c.AcuteHealthEffectsSkin),
                    Row("Potential acute health effects — Ingestion", c.AcuteHealthEffectsIngestion),
                    Row("Symptoms — Eye", c.ToxicologicalSymptomsEye),
                    Row("Symptoms — Inhalation", c.ToxicologicalSymptomsInhalation),
                    Row("Symptoms — Skin", c.ToxicologicalSymptomsSkin),
                    Row("Symptoms — Ingestion", c.ToxicologicalSymptomsIngestion),
                    Row("Short term — Immediate effects", c.ShortTermImmediateEffects),
                    Row("Short term — Delayed effects", c.ShortTermDelayedEffects),
                    Row("Long term — Immediate effects", c.LongTermImmediateEffects),
                    Row("Long term — Delayed effects", c.LongTermDelayedEffects),
                    Row("Potential chronic health effects", c.ChronicHealthEffects),
                    Row("Chronic — General", c.ChronicEffectsGeneral),
                    Row("Chronic — Carcinogenicity", c.ChronicCarcinogenicity),
                    Row("Chronic — Mutagenicity", c.ChronicMutagenicity),
                    Row("Chronic — Teratogenicity", c.ChronicTeratogenicity),
                    Row("Developmental effects", c.DevelopmentalEffects),
                    Row("Fertility effects", c.FertilityEffects),
                    Row("Acute toxicity estimates", c.AcuteToxicityEstimates));
            });
        }

        private void RenderSection12(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "12", "Ecological information", body =>
            {
                if (!string.IsNullOrWhiteSpace(c.Ecotoxicity) && c.Ecotoxicity.Contains('|'))
                    AddPipeTable(body, "Toxicity",
                        ["Product/ingredient name", "Result", "Species", "Exposure"], c.Ecotoxicity);
                else
                    AddRow(body, "Toxicity", c.Ecotoxicity);

                AddRows(body, c,
                    Row("Persistence and degradability", c.PersistenceDegradability));

                if (!string.IsNullOrWhiteSpace(c.BioaccumulativePotential) && c.BioaccumulativePotential.Contains('|'))
                    AddPipeTable(body, "Bioaccumulative potential",
                        ["Product/ingredient name", "LogP\u2080w", "BCF", "Potential"], c.BioaccumulativePotential);
                else
                    AddRow(body, "Bioaccumulative potential", c.BioaccumulativePotential);

                AddRows(body, c,
                    Row("Soil/water partition coefficient (Koc)", c.MobilityInSoil),
                    Row("Other adverse effects", c.OtherEcologicalEffects));
            });
        }

        private static void RenderSection13(ColumnDescriptor col, Chemical c) =>
            Section(col, "13", "Disposal considerations",
                Row("Disposal methods", c.DisposalMethods));

        private void RenderSection14(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "14", "Transport information", body =>
            {
                if (!string.IsNullOrWhiteSpace(c.TransportRegulations) && c.TransportRegulations.Contains('|'))
                    AddTransportRegulationsTable(body, c);
                else
                {
                    AddRows(body, c,
                        Row("UN number", c.UNNumber),
                        Row("UN proper shipping name", c.UNProperShippingName),
                        Row("Transport hazard class(es)", c.TransportHazardClass),
                        Row("Packing group", c.PackingGroup),
                        Row("Environmental hazards", c.EnvironmentalHazardsTransport));
                    RenderTransportHazardClassImages(body, c);
                }

                AddRows(body, c,
                    Row("DOT classification", c.TransportDotClassification),
                    Row("TDG classification", c.TransportTdgClassification),
                    Row("IATA", c.TransportIataClassification),
                    Row("Special precautions for user", c.TransportSpecialPrecautions),
                    Row("Transport in bulk (MARPOL / IBC Code)", c.TransportBulkMarpol));
            });
        }

        private static void RenderSection15(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "15", "Regulatory information", body =>
            {
                body.Item().PaddingBottom(2).Text("U.S. Federal regulations").Bold().FontColor("#1565c0").FontSize(8);
                AddRows(body, c,
                    Row("TSCA 8(a) CDR Exempt/Partial exemption", c.RegTscaCdrExempt),
                    Row("Clean Air Act Section 112 (b) HAPs", c.RegCleanAirActHaps),
                    Row("Clean Air Act Section 602 Class I", c.RegCleanAirActClassI),
                    Row("Clean Air Act Section 602 Class II", c.RegCleanAirActClassII),
                    Row("DEA List I", c.RegDeaListI),
                    Row("DEA List II", c.RegDeaListII));

                body.Item().PaddingTop(4).Text("SARA").Bold().FontColor("#1565c0").FontSize(8);
                AddRows(body, c,
                    Row("SARA 302/304 — Composition", c.RegSara302Composition ?? c.SARA302304),
                    Row("SARA 304 RQ", c.RegSara304RQ),
                    Row("SARA 311/312 — Classification", c.RegSara311312Classification));

                body.Item().PaddingTop(4).Text("State regulations").Bold().FontColor("#1565c0").FontSize(8);
                AddRows(body, c,
                    Row("Massachusetts", c.RegStateMassachusetts),
                    Row("New York", c.RegStateNewYork),
                    Row("New Jersey", c.RegStateNewJersey),
                    Row("Pennsylvania", c.RegStatePennsylvania));

                body.Item().PaddingTop(4).Text("International regulations").Bold().FontColor("#1565c0").FontSize(8);
                AddRows(body, c,
                    Row("Chemical Weapon Convention", c.RegChemWeaponConvention),
                    Row("Montreal Protocol", c.RegMontrealProtocol),
                    Row("Stockholm Convention", c.RegStockholmConvention),
                    Row("Rotterdam Convention (PIC)", c.RegRotterdamConvention),
                    Row("UNECE Aarhus Protocol", c.RegUneceAarhus));

                body.Item().PaddingTop(4).Text("Inventory list").Bold().FontColor("#1565c0").FontSize(8);
                AddRows(body, c,
                    Row("Australia", c.RegInventoryAustralia),
                    Row("Canada", c.RegInventoryCanada),
                    Row("China", c.RegInventoryChina),
                    Row("Europe", c.RegInventoryEurope),
                    Row("Japan (ENCS)", c.RegInventoryJapanEncs),
                    Row("Japan (ISHL)", c.RegInventoryJapanIshl),
                    Row("Malaysia", c.RegInventoryMalaysia),
                    Row("New Zealand", c.RegInventoryNewZealand),
                    Row("Philippines", c.RegInventoryPhilippines),
                    Row("Republic of Korea", c.RegInventoryKorea),
                    Row("Taiwan", c.RegInventoryTaiwan),
                    Row("Thailand", c.RegInventoryThailand),
                    Row("Turkey", c.RegInventoryTurkey),
                    Row("United States", c.RegInventoryUnitedStates),
                    Row("Viet Nam", c.RegInventoryVietnam));

                if (!string.IsNullOrWhiteSpace(c.InternationalRegulations))
                {
                    body.Item().PaddingTop(4).Text("International regulations (summary)").Bold().FontColor("#1565c0").FontSize(8);
                    body.Item().Text(c.InternationalRegulations).FontSize(8);
                }

                if (!string.IsNullOrWhiteSpace(c.InventoryList))
                {
                    body.Item().PaddingTop(4).Text("Inventory list (summary)").Bold().FontColor("#1565c0").FontSize(8);
                    body.Item().Text(c.InventoryList).FontSize(8);
                }
            });
        }

        private static void RenderSection16(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "16", "Other information", body =>
            {
                var hmisH = c.HmisHealth ?? ParseRating(c.HMISRatings, "Health");
                var hmisF = c.HmisFlammability ?? ParseRating(c.HMISRatings, "Flammability");
                var hmisP = c.HmisPhysicalHazards ?? ParseRating(c.HMISRatings, "Physical");
                if (!string.IsNullOrWhiteSpace(hmisH) || !string.IsNullOrWhiteSpace(hmisF) || !string.IsNullOrWhiteSpace(hmisP))
                {
                    body.Item().Text("Hazardous Material Information System (U.S.A.)").Bold().FontSize(8);
                    AddPipeTable(body, "", ["Category", "Rating"],
                        $"Health|{hmisH ?? "—"}\nFlammability|{hmisF ?? "—"}\nPhysical hazards|{hmisP ?? "—"}");
                    if (!string.IsNullOrWhiteSpace(c.HmisCautionNote))
                        body.Item().PaddingBottom(4).Text(c.HmisCautionNote).FontSize(7).Italic();
                }

                var nfpaH = c.NfpaHealth ?? ParseRating(c.NFPARatings, "Health");
                var nfpaF = c.NfpaFlammability ?? ParseRating(c.NFPARatings, "Flammability");
                var nfpaR = c.NfpaReactivity ?? ParseRating(c.NFPARatings, "Instability") ?? ParseRating(c.NFPARatings, "Reactivity");
                if (!string.IsNullOrWhiteSpace(nfpaH) || !string.IsNullOrWhiteSpace(nfpaF))
                {
                    body.Item().PaddingTop(4).Text("NFPA 704 (U.S.A.)").Bold().FontSize(8);
                    body.Item().Text($"Health: {nfpaH}  |  Flammability: {nfpaF}  |  Reactivity: {nfpaR}  |  Special: {c.NfpaSpecial ?? "—"}").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(c.NfpaCopyrightNote))
                        body.Item().Text(c.NfpaCopyrightNote).FontSize(7).Italic();
                }

                if (!string.IsNullOrWhiteSpace(c.ClassificationProcedure))
                    AddPipeTable(body, "Procedure used to derive the classification",
                        ["Classification", "Justification"],
                        $"{c.ClassificationProcedure}|{c.ClassificationJustification}");

                AddRows(body, c,
                    Row("Date of printing", c.DateOfPrinting),
                    Row("Date of issue / revision", c.DateOfRevision),
                    Row("Date of previous issue", c.DateOfPreviousIssue),
                    Row("Version", c.Version));

                if (c.Quantity > 0 || !string.IsNullOrWhiteSpace(c.StorageLocation) || !string.IsNullOrWhiteSpace(c.Unit))
                {
                    body.Item().PaddingTop(4).Text("Lab inventory (internal)").Bold().FontColor("#1565c0").FontSize(8);
                    AddRows(body, c,
                        Row("Quantity", c.Quantity > 0 ? $"{c.Quantity} {c.Unit}".Trim() : c.Unit),
                        Row("Storage location", c.StorageLocation));
                }

                body.Item().PaddingTop(6).AlignCenter().Text("UN = United Nations").FontSize(8);
                AddRow(body, "References", string.IsNullOrWhiteSpace(c.References) ? "Not available." : c.References);

                if (!string.IsNullOrWhiteSpace(c.Abbreviations))
                {
                    body.Item().PaddingTop(4).Text("Key to abbreviations").Bold().FontColor("#1565c0").FontSize(8);
                    body.Item().Text(c.Abbreviations).FontSize(7);
                }

                body.Item().PaddingTop(6).Text("Notice to reader").Bold().FontColor("#1565c0").FontSize(8);
                body.Item().Text(c.NoticeToReader ?? DefaultNotice).FontSize(8).Bold();
            });
        }

        private void RenderGhsPictograms(ColumnDescriptor body, Chemical chem)
        {
            var codes = GhsPictogramCatalog.ParseCodes(chem.GhsPictogramCodes);
            var uploadPath = chem.GhsPictogramImagePath;
            var hasUpload = !string.IsNullOrWhiteSpace(uploadPath);
            if (codes.Count == 0 && !hasUpload)
                return;

            body.Item().PaddingTop(4).Text("GHS hazard pictograms").Bold().FontSize(8);

            body.Item().PaddingTop(2).Row(row =>
            {
                foreach (var code in codes)
                {
                    var item = GhsPictogramCatalog.All.FirstOrDefault(g =>
                        string.Equals(g.Code, code, StringComparison.OrdinalIgnoreCase));
                    if (item == null)
                        continue;

                    row.ConstantItem(68).Padding(2).Column(fig =>
                    {
                        fig.Item().Element(cell => EmbedImage(cell, item.ImagePath, 56));
                        fig.Item().AlignCenter().Text(item.Label).FontSize(6);
                    });
                }

                if (hasUpload)
                {
                    row.ConstantItem(68).Padding(2).Column(fig =>
                    {
                        fig.Item().Element(cell => EmbedImage(cell, uploadPath, 56));
                        fig.Item().AlignCenter().Text("Label image").FontSize(6);
                    });
                }
            });
        }

        private void RenderTransportHazardClassImages(ColumnDescriptor body, Chemical c)
        {
            if (string.IsNullOrWhiteSpace(c.TransportHazardClass))
                return;

            var classNum = c.TransportHazardClass.Trim().Split(' ')[0];
            var custom = TransportHazardClassCatalog.ParseCustomImages(c.TransportHazardClassImages);
            string? rel = null;
            if (custom.Count > 0)
                rel = custom.Values.FirstOrDefault();
            rel ??= TransportHazardClassCatalog.GetImagePath(classNum);

            body.Item().PaddingTop(4).Text("Transport hazard pictogram").Bold().FontSize(8);
            body.Item().Element(c => EmbedImage(c, rel, 56));
        }

        private void AddTransportRegulationsTable(ColumnDescriptor body, Chemical c)
        {
            var data = c.TransportRegulations;
            if (string.IsNullOrWhiteSpace(data) || !data.Contains('|'))
                return;

            var authorities = new[] { "DOT", "TDG", "Mexico", "IMDG", "IATA" };
            var rows = data.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(l => l.Split('|').Select(x => x.Trim()).ToArray())
                .Where(r => r.Length > 0 && !string.IsNullOrWhiteSpace(r[0]))
                .ToList();
            if (rows.Count == 0)
                return;

            var customImages = TransportHazardClassCatalog.ParseCustomImages(c.TransportHazardClassImages);

            body.Item().PaddingTop(4).PaddingBottom(2).Text("Transport regulations").Bold().FontSize(8);

            body.Item().PaddingBottom(4).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(95);
                    foreach (var _ in authorities)
                        cols.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background("#e8f5e9").Border(0.5f).BorderColor("#ccc")
                        .Padding(3).Text("Field").Bold().FontSize(7);
                    foreach (var auth in authorities)
                        header.Cell().Background("#e8f5e9").Border(0.5f).BorderColor("#ccc")
                            .Padding(3).Text(auth).Bold().FontSize(7);
                });

                foreach (var row in rows)
                {
                    var field = row[0];
                    var isHazard = field.Contains("hazard class", StringComparison.OrdinalIgnoreCase);

                    table.Cell().Border(0.5f).BorderColor("#ddd").Padding(3)
                        .Text(field).Bold().FontSize(7);

                    for (var i = 0; i < authorities.Length; i++)
                    {
                        var val = i + 1 < row.Length ? row[i + 1] : "";
                        var auth = authorities[i];

                        table.Cell().Border(0.5f).BorderColor("#ddd").Padding(3).Column(cell =>
                        {
                            if (!string.IsNullOrWhiteSpace(val))
                                cell.Item().Text(val).FontSize(7);

                            if (isHazard && !string.IsNullOrWhiteSpace(val))
                            {
                                var classNum = val.Split(' ')[0];
                                customImages.TryGetValue(auth, out var customPath);
                                var rel = !string.IsNullOrWhiteSpace(customPath)
                                    ? customPath
                                    : TransportHazardClassCatalog.GetImagePath(classNum);
                                cell.Item().Element(c => EmbedImage(c, rel, 40));
                            }
                        });
                    }
                }
            });
        }

        private void EmbedImage(IContainer container, string? relativePath, float maxHeight)
        {
            var bytes = SdsPdfImageHelper.LoadImageBytes(_webRoot, relativePath, (int)maxHeight * 2);
            if (bytes == null || bytes.Length == 0)
                return;

            container.PaddingTop(2).MaxHeight(maxHeight).Image(bytes).FitArea();
        }

        private static void Section(ColumnDescriptor col, string num, string title, params (string Label, string? Value)[] rows)
        {
            SectionStart(col, num, title, body => AddRows(body, null, rows));
        }

        private static void SectionStart(ColumnDescriptor col, string num, string title, Action<ColumnDescriptor> buildBody)
        {
            col.Item().PaddingTop(6).Column(sec =>
            {
                sec.Item().Background("#1b8a4a").Padding(5).Text($"Section {num}. {title}")
                    .FontSize(9).Bold().FontColor(Colors.White);
                sec.Item().Border(1).BorderColor("#d0d0d0").Padding(6).Column(buildBody);
            });
        }

        private static void AddRows(ColumnDescriptor body, Chemical? _, params (string Label, string? Value)[] rows)
        {
            foreach (var (label, value) in rows)
                AddRow(body, label, value);
        }

        private static void AddRow(ColumnDescriptor body, string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            body.Item().PaddingBottom(3).Row(r =>
            {
                r.ConstantItem(155).Text(label).Bold().FontSize(8).FontColor("#1565c0");
                r.RelativeItem().Text(value.Replace("\r\n", "\n")).FontSize(8);
            });
        }

        private static void AddOelTable(ColumnDescriptor body, string title, string? data)
        {
            var rows = OelLimitsFormatter.Parse(data);
            if (rows.Count == 0)
            {
                if (!string.IsNullOrWhiteSpace(data))
                    AddRow(body, title, data);
                return;
            }

            if (!string.IsNullOrWhiteSpace(title))
                body.Item().PaddingTop(4).PaddingBottom(2).Text(title).Bold().FontSize(8);

            body.Item().PaddingBottom(4).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#e8f5e9").Border(0.5f).BorderColor("#ccc")
                        .Padding(3).Text("Ingredient name").Bold().FontSize(7);
                    header.Cell().Background("#e8f5e9").Border(0.5f).BorderColor("#ccc")
                        .Padding(3).Text("Exposure limits").Bold().FontSize(7);
                });

                foreach (var row in rows)
                {
                    table.Cell().Border(0.5f).BorderColor("#ddd").Padding(3)
                        .Text(row.Ingredient).FontSize(7);
                    table.Cell().Border(0.5f).BorderColor("#ddd").Padding(3)
                        .Text(row.Limits).FontSize(7);
                }
            });
        }

        private static void AddPipeTable(ColumnDescriptor body, string title, string[] headers, string? data)
        {
            if (string.IsNullOrWhiteSpace(data) || !data.Contains('|')) return;

            var rows = data.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(l => l.Split('|').Select(c => c.Trim()).ToArray())
                .Where(r => r.Any(c => !string.IsNullOrEmpty(c)))
                .ToList();
            if (rows.Count == 0) return;

            if (!string.IsNullOrWhiteSpace(title))
                body.Item().PaddingTop(4).PaddingBottom(2).Text(title).Bold().FontSize(8);

            body.Item().PaddingBottom(4).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    foreach (var _ in headers)
                        cols.RelativeColumn();
                });

                table.Header(header =>
                {
                    foreach (var h in headers)
                        header.Cell().Background("#e8f5e9").Border(0.5f).BorderColor("#ccc")
                            .Padding(3).Text(h).Bold().FontSize(7);
                });

                foreach (var row in rows)
                {
                    for (var i = 0; i < headers.Length; i++)
                    {
                        var cell = i < row.Length ? row[i] : "";
                        table.Cell().Border(0.5f).BorderColor("#ddd").Padding(3).Text(cell).FontSize(7);
                    }
                }
            });
        }

        private static string? ParseRating(string? text, string key)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            foreach (var part in text.Split(',', StringSplitOptions.TrimEntries))
            {
                if (part.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                {
                    var idx = part.IndexOf(':');
                    return idx >= 0 ? part[(idx + 1)..].Trim() : part;
                }
            }
            return null;
        }

        private static (string Label, string? Value) Row(string label, string? value) => (label, value);

        private const string DefaultNotice =
            "To the best of our knowledge, the information contained herein is accurate. However, neither the above-named supplier, nor any of its subsidiaries, assumes any liability whatsoever for the accuracy or completeness of the information contained herein. Final determination of suitability of any material is the sole responsibility of the user. All materials may present unknown hazards and should be used with caution. Although certain hazards are described herein, we cannot guarantee that these are the only hazards that exist.";
    }
}
