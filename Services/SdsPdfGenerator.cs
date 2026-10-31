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
                    col.Item().Text("C.I.W.C&E — DGLW Govt of Punjab").FontSize(8).FontColor("#c8e6c9");
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

        private static void RenderSection1(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "1", "Identification", body =>
            {
                var rows = Sec01SubsectionCodec.Parse(Sec01SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Product identifier / No.", c.ProductIdentifier);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private void RenderSection2(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "2", "Hazard(s) identification", body =>
            {
                var blocks = Sec02BlockCodec.Parse(Sec02BlockCodec.ResolveStoredJson(c));
                var pictogramRendered = false;
                foreach (var block in blocks)
                {
                    if (block.Type == "text")
                    {
                        if (string.IsNullOrWhiteSpace(block.Heading) && string.IsNullOrWhiteSpace(block.Description))
                            continue;
                        body.Item().PaddingTop(4).Text(block.Heading).Bold().FontColor("#1565c0").FontSize(8);
                        if (!string.IsNullOrWhiteSpace(block.Description))
                            body.Item().Text(block.Description).FontSize(8);
                        continue;
                    }
                    if (block.Type == "pictograms")
                    {
                        body.Item().PaddingTop(4).Text("Hazard pictograms").Bold().FontColor("#1565c0").FontSize(8);
                        RenderGhsPictograms(body, c, block.Codes);
                        if (!string.IsNullOrWhiteSpace(block.CustomImagePath))
                        {
                            body.Item().PaddingTop(2).Row(row =>
                            {
                                row.ConstantItem(68).Padding(2).Column(fig =>
                                {
                                    fig.Item().Element(cell => EmbedImage(cell, block.CustomImagePath, 56));
                                    fig.Item().AlignCenter().Text("Custom pictogram").FontSize(6);
                                });
                            });
                        }
                        pictogramRendered = true;
                        continue;
                    }
                    if (block.Type == "table" && block.Rows.Any(r =>
                            !string.IsNullOrWhiteSpace(r.Cas) || !string.IsNullOrWhiteSpace(r.ChemicalName)))
                    {
                        var data = string.Join("\n", block.Rows
                            .Where(r => !string.IsNullOrWhiteSpace(r.Cas) || !string.IsNullOrWhiteSpace(r.ChemicalName))
                            .Select(r => $"{r.Cas}|{r.ChemicalName}"));
                        AddPipeTable(body, block.Title, ["CAS No.", "Chemical name"], data);
                    }
                }
                if (!pictogramRendered && (!string.IsNullOrWhiteSpace(c.GhsPictogramCodes) || !string.IsNullOrWhiteSpace(c.GhsPictogramImagePath)))
                    RenderGhsPictograms(body, c, c.GhsPictogramCodes);
            });
        }

        private static void RenderSection3(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "3", "Composition / information on ingredients", body =>
            {
                var blocks = Sec03BlockCodec.Parse(Sec03BlockCodec.ResolveStoredJson(c));
                if (blocks.Count == 0)
                {
                    AddRows(body, c,
                        Row("Substance/Mixture", c.SubstanceMixture),
                        Row("CAS number", c.CASNumber),
                        Row("Ingredient name", c.IngredientName),
                        Row("Percentage", c.Percentage));
                    return;
                }
                foreach (var block in blocks)
                {
                    if (block.Type == "text")
                    {
                        if (string.IsNullOrWhiteSpace(block.Heading) && string.IsNullOrWhiteSpace(block.Description))
                            continue;
                        body.Item().PaddingTop(4).Text(block.Heading).Bold().FontColor("#1565c0").FontSize(8);
                        if (!string.IsNullOrWhiteSpace(block.Description))
                            body.Item().Text(block.Description).FontSize(8);
                        continue;
                    }
                    if (block.Type == "mixtureTable" && block.Rows.Any(r =>
                            !string.IsNullOrWhiteSpace(r.Cas) || !string.IsNullOrWhiteSpace(r.ChemicalName)))
                    {
                        var data = string.Join("\n", block.Rows
                            .Where(r => !string.IsNullOrWhiteSpace(r.Cas) || !string.IsNullOrWhiteSpace(r.ChemicalName))
                            .Select(r => $"{r.Cas}|{r.ChemicalName}|{r.Proportion}|{r.Classification}|{r.Source}"));
                        AddPipeTable(body, "Mixture ingredients",
                            ["CAS No.", "Chemical name", "Proportion (Sch 8)", "Classification (GHS)", "Source"],
                            data);
                        continue;
                    }
                    if (block.Type == "footnote" && !string.IsNullOrWhiteSpace(block.Description))
                        body.Item().PaddingTop(4).Text(block.Description).FontSize(7).Italic();
                }
            });
        }

        private static void RenderSection4(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "4", "First aid measures", body =>
            {
                var rows = Sec04SubsectionCodec.Parse(Sec04SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Inhalation", c.InhalationFirstAid);
                    AddRow(body, "Eye contact", c.EyeContactFirstAid);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection5(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "5", "Firefighting measures", body =>
            {
                var rows = Sec05SubsectionCodec.Parse(Sec05SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Suitable extinguishing agents", c.SuitableExtinguishingMedia);
                    AddRow(body, "Unsuitable extinguishing agents", c.UnsuitableExtinguishingMedia);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection6(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "6", "Accidental release measures", body =>
            {
                var rows = Sec06SubsectionCodec.Parse(Sec06SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "For non-emergency personnel", c.SpillNonEmergencyPersonnel);
                    AddRow(body, "Environmental precautions", c.EnvironmentalPrecautionsSpill);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection7(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "7", "Handling and storage", body =>
            {
                var rows = Sec07SubsectionCodec.Parse(Sec07SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Precautions for safe handling", c.HandlingPrecautions);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection8(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "8", "Exposure controls / personal protection", body =>
            {
                var rows = Sec08SubsectionCodec.Parse(Sec08SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Control parameters", c.OccupationalExposureLimits);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection9(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "9", "Physical and chemical properties", body =>
            {
                var rows = Sec09SubsectionCodec.Parse(Sec09SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "State of aggregation", c.PhysicalState);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection10(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "10", "Stability and reactivity", body =>
            {
                var rows = Sec10SubsectionCodec.Parse(Sec10SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Reactivity", c.Reactivity);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection11(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "11", "Toxicological information", body =>
            {
                var rows = Sec11SubsectionCodec.Parse(Sec11SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Acute toxicity", c.AcuteToxicity);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection12(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "12", "Ecological information", body =>
            {
                var rows = Sec12SubsectionCodec.Parse(Sec12SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Toxicity", c.Ecotoxicity);
                    return;
                }
                foreach (var row in rows)
                    RenderSubsectionItem(body, row);
            });
        }

        private static void RenderSection13(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "13", "Disposal considerations", body =>
            {
                var rows = Sec13SubsectionCodec.Parse(Sec13SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "Waste treatment methods", c.DisposalMethods);
                    return;
                }
                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;
                    body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#1565c0").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(row.Description))
                        body.Item().Text(row.Description).FontSize(8);
                }
            });
        }

        private void RenderSection14(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "14", "Transport information", body =>
            {
                var rows = Sec14SubsectionCodec.Parse(Sec14SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    AddRow(body, "UN number", c.UNNumber);
                    RenderTransportHazardClassImages(body, c);
                    return;
                }
                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;
                    body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#1565c0").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(row.Description))
                    {
                        foreach (var line in row.Description.Split('\n'))
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            if (line.TrimStart().StartsWith("ADR /", StringComparison.OrdinalIgnoreCase))
                                body.Item().Text(line.Trim()).Bold().FontSize(8);
                            else
                                body.Item().Text(line.Trim()).FontSize(8);
                        }
                    }
                }
                RenderTransportHazardClassImages(body, c);
            });
        }

        private static void RenderSection15(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "15", "Regulatory information", body =>
            {
                var rows = Sec15SubsectionCodec.Parse(Sec15SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(c.USFederalRegulations))
                        body.Item().Text(c.USFederalRegulations).FontSize(8);
                    return;
                }

                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;
                    body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#1565c0").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(row.Description))
                    {
                        foreach (var line in row.Description.Split('\n'))
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            body.Item().Text(line.Trim()).FontSize(8);
                        }
                    }
                }
            });
        }

        private static void RenderSection16(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "16", "Other information", body =>
            {
                var rows = Sec16SubsectionCodec.Parse(Sec16SubsectionCodec.ResolveStoredJson(c));
                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;

                    body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#1565c0").FontSize(8);

                    if (row.Heading.TrimStart().StartsWith("16.5", StringComparison.OrdinalIgnoreCase))
                    {
                        var rev = Sec16SubsectionCodec.ComposeRevisionParagraph(c, row);
                        if (!string.IsNullOrWhiteSpace(rev))
                            body.Item().Text(rev).FontSize(8);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(row.Description))
                        body.Item().Text(row.Description).FontSize(8);
                    if (row.Fields is { Count: > 0 })
                        AddSubsectionFieldsTable(body, row.Heading, row.Fields);
                }

                if (c.Quantity > 0 || !string.IsNullOrWhiteSpace(c.StorageLocation) || !string.IsNullOrWhiteSpace(c.Unit))
                {
                    body.Item().PaddingTop(4).Text("Lab inventory (internal)").Bold().FontColor("#1565c0").FontSize(8);
                    AddRows(body, c,
                        Row("Quantity", c.Quantity > 0 ? $"{c.Quantity} {c.Unit}".Trim() : c.Unit),
                        Row("Storage location", c.StorageLocation));
                }
            });
        }

        private void RenderGhsPictograms(ColumnDescriptor body, Chemical chem, string? codesCsv = null)
        {
            var codes = GhsPictogramCatalog.ParseCodes(codesCsv ?? chem.GhsPictogramCodes);
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

        private static string EscapeTableCell(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return value.Replace("|", " / ").Replace("\r\n", " ").Replace("\n", " ").Trim();
        }

        private static void RenderSubsectionItem(ColumnDescriptor body, Sec01SubsectionItem row)
        {
            if (string.IsNullOrWhiteSpace(row.Heading) &&
                string.IsNullOrWhiteSpace(row.Description) &&
                !Sec01SubsectionCodec.HasFieldContent(row))
                return;

            body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#1565c0").FontSize(8);
            if (!string.IsNullOrWhiteSpace(row.Description))
                body.Item().Text(row.Description).FontSize(8);
            if (row.Fields is { Count: > 0 })
                AddSubsectionFieldsTable(body, row.Heading, row.Fields);
        }

        private static void AddSubsectionFieldsTable(ColumnDescriptor body, string heading, IEnumerable<Sec01SubsectionField> fields)
        {
            var rows = fields
                .Where(f => !string.IsNullOrWhiteSpace(f.Label) || !string.IsNullOrWhiteSpace(f.Description))
                .ToList();
            if (rows.Count == 0)
                return;

            var (h1, h2) = SdsSubsectionTableHeaders.ForHeading(heading);
            var data = string.Join("\n", rows.Select(f =>
                $"{EscapeTableCell(f.Label.Trim().TrimEnd(':'))}|{EscapeTableCell(f.Description)}"));
            AddPipeTable(body, "", [h1, h2], data);
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
