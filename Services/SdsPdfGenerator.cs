using System.Net;
using System.Text.RegularExpressions;
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
            var title = Sec01SubsectionCodec.ShortProductLabel(c.ProductIdentifier);
            if (string.IsNullOrWhiteSpace(title))
                title = "Chemical";

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(18);
                    page.MarginVertical(22);
                    page.DefaultTextStyle(x => x.FontSize(9).LineHeight(1.35f));

                    page.Header().Element(h => RenderHeader(h, title));
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

        private void RenderHeader(IContainer container, string title)
        {
            container.Background("#dc1147").Padding(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SAFETY DATA SHEET").FontSize(14).Bold().FontColor(Colors.White);
                    col.Item().Text("Chemical Safety Data Sheet System").FontSize(8).FontColor("#fdecef");
                });
                row.ConstantItem(130).AlignRight().Column(col =>
                {
                    col.Item().Text(title).FontSize(11).Bold().FontColor(Colors.White);
                });
            });
        }

        private void RenderFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(t =>
                {
                    t.Span("Chemical SDS System | ").FontSize(7).FontColor(Colors.Grey.Medium);
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
                foreach (var block in blocks)
                {
                    if (block.Type == "text")
                    {
                        if (string.IsNullOrWhiteSpace(block.Heading) && string.IsNullOrWhiteSpace(block.Description))
                            continue;
                        if (ShouldPrintHeading(block.Heading, block.Description))
                            body.Item().PaddingTop(4).Text(block.Heading).Bold().FontColor("#e60a36").FontSize(8);
                        WriteRichText(body, block.Description);
                        continue;
                    }
                    if (block.Type == "pictograms")
                    {
                        body.Item().PaddingTop(4).Text("Hazard pictograms").Bold().FontColor("#e60a36").FontSize(8);
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
            });
        }

        private static void RenderSection3(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "3", "Composition / information on ingredients", body =>
            {
                var blocks = Sec03BlockCodec.Parse(Sec03BlockCodec.ResolveStoredJson(c));
                if (blocks.Count == 0)
                    return;
                foreach (var block in blocks)
                {
                    if (block.Type == "text")
                    {
                        if (string.IsNullOrWhiteSpace(block.Heading) && string.IsNullOrWhiteSpace(block.Description))
                            continue;
                        if (ShouldPrintHeading(block.Heading, block.Description))
                            body.Item().PaddingTop(4).Text(block.Heading).Bold().FontColor("#e60a36").FontSize(8);
                        WriteRichText(body, block.Description);
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
                    {
                        if (Sec01SubsectionCodec.LooksLikeRichText(block.Description))
                            WriteRichText(body, block.Description);
                        else
                            body.Item().PaddingTop(4).Text(block.Description).FontSize(7).Italic();
                    }
                }
            });
        }

        private static void RenderSection4(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "4", "First aid measures", body =>
            {
                var rows = Sec04SubsectionCodec.Parse(Sec04SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                    return;
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
                    return;
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
                    return;
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
                    return;
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
                    return;
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
                    return;
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
                    return;
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
                    return;
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
                    return;
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
                    return;
                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;
                    if (ShouldPrintHeading(row.Heading, row.Description))
                        body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#e60a36").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(row.Description))
                        WriteRichText(body, row.Description);
                }
            });
        }

        private void RenderSection14(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "14", "Transport information", body =>
            {
                var rows = Sec14SubsectionCodec.Parse(Sec14SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                    return;
                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;
                    if (ShouldPrintHeading(row.Heading, row.Description))
                        body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#e60a36").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(row.Description))
                    {
                        if (Sec01SubsectionCodec.LooksLikeRichText(row.Description))
                            WriteRichText(body, row.Description);
                        else
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
                }
            });
        }

        private static void RenderSection15(ColumnDescriptor col, Chemical c)
        {
            SectionStart(col, "15", "Regulatory information", body =>
            {
                var rows = Sec15SubsectionCodec.Parse(Sec15SubsectionCodec.ResolveStoredJson(c));
                if (rows.Count == 0)
                    return;

                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Heading) &&
                        string.IsNullOrWhiteSpace(row.Description) &&
                        !Sec01SubsectionCodec.HasFieldContent(row))
                        continue;
                    if (ShouldPrintHeading(row.Heading, row.Description))
                        body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#e60a36").FontSize(8);
                    if (!string.IsNullOrWhiteSpace(row.Description))
                        WriteRichText(body, row.Description);
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

                    if (ShouldPrintHeading(row.Heading, row.Description))
                        body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#e60a36").FontSize(8);

                    if (row.Heading.TrimStart().StartsWith("16.5", StringComparison.OrdinalIgnoreCase))
                    {
                        var rev = Sec16SubsectionCodec.ComposeRevisionParagraph(row);
                        if (!string.IsNullOrWhiteSpace(rev))
                            body.Item().Text(rev).FontSize(8);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(row.Description))
                        WriteRichText(body, row.Description);
                    if (row.Fields is { Count: > 0 })
                        AddSubsectionFieldsTable(body, row.Heading, row.Fields);
                }

            });
        }

        private void RenderGhsPictograms(ColumnDescriptor body, Chemical chem, string? codesCsv = null)
        {
            var codes = GhsPictogramCatalog.ParseCodes(codesCsv);
            if (codes.Count == 0)
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
                sec.Item().Background("#c4113a").Padding(6).Text($"Section {num}. {title}")
                    .FontSize(10).Bold().FontColor(Colors.White);
                sec.Item().Border(1).BorderColor("#d0d0d0").Padding(8).Column(buildBody);
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
                r.ConstantItem(155).Text(label).Bold().FontSize(8).FontColor("#e60a36");
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
                    header.Cell().Background("#fdecef").Border(0.5f).BorderColor("#ccc")
                        .Padding(3).Text("Ingredient name").Bold().FontSize(7);
                    header.Cell().Background("#fdecef").Border(0.5f).BorderColor("#ccc")
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

        private static void WriteRichText(ColumnDescriptor body, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!Sec01SubsectionCodec.LooksLikeRichText(value))
            {
                foreach (var line in value.Replace("\r\n", "\n").Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    body.Item().PaddingTop(2).Text(line.Trim()).FontSize(9).LineHeight(1.35f);
                }
                return;
            }

            var nodes = DropRepeatedLeadingHeading(ParseRichNodes(Sec01SubsectionCodec.SanitizeRichText(value)));
            foreach (var node in nodes)
            {
                if (node is PdfParagraph paragraph)
                    WriteParagraph(body, paragraph);
                else if (node is PdfGrid grid)
                    WriteGrid(body, grid);
            }
        }

        private static bool ShouldPrintHeading(string? heading, string? description) =>
            !string.IsNullOrWhiteSpace(heading) &&
            !Sec01SubsectionCodec.HeadingAlreadyInDescription(heading, description);

        private static string ParagraphText(PdfParagraph paragraph) =>
            Regex.Replace(string.Concat(paragraph.Runs.Select(run => run.Text)), @"\s+", " ").Trim();

        private static List<PdfNode> DropRepeatedLeadingHeading(List<PdfNode> nodes)
        {
            if (nodes.Count < 2 || nodes[0] is not PdfParagraph first || nodes[1] is not PdfParagraph second)
                return nodes;
            var leading = ParagraphText(first);
            var next = ParagraphText(second);
            if (leading.Length == 0) return nodes;
            if (next.Equals(leading, StringComparison.OrdinalIgnoreCase) || next.StartsWith(leading, StringComparison.OrdinalIgnoreCase))
                return nodes.Skip(1).ToList();
            return nodes;
        }

        private static void WriteParagraph(ColumnDescriptor body, PdfParagraph paragraph)
        {
            if (paragraph.Runs.All(run => string.IsNullOrWhiteSpace(run.Text))) return;
            body.Item().PaddingTop(paragraph.Heading ? 7 : 3).Text(text =>
            {
                text.DefaultTextStyle(style => style.FontSize(paragraph.Heading ? 10 : 9).LineHeight(1.4f).FontColor("#222"));
                if (paragraph.Bullet)
                    text.Span("• ").FontSize(9);
                foreach (var run in paragraph.Runs)
                {
                    if (run.Text.Length == 0) continue;
                    var span = text.Span(run.Text);
                    if (run.Bold || paragraph.Heading) span = span.Bold();
                    if (run.Italic) span = span.Italic();
                    if (run.Underline) span = span.Underline();
                    if (run.Strike) span = span.Strikethrough();
                    if (!string.IsNullOrEmpty(run.Color)) span = span.FontColor(run.Color);
                    if (!string.IsNullOrEmpty(run.Background)) span = span.BackgroundColor(run.Background);
                }
            });
        }

        private static void WriteGrid(ColumnDescriptor body, PdfGrid grid)
        {
            if (grid.Rows.Count == 0) return;
            var columns = grid.Rows.Max(row => row.Sum(cell => Math.Max(1, cell.ColSpan)));
            if (columns < 1) return;
            body.Item().PaddingTop(3).PaddingBottom(3).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    for (var i = 0; i < columns; i++)
                        cols.RelativeColumn();
                });
                foreach (var row in grid.Rows)
                {
                    foreach (var cell in row)
                    {
                        var box = table.Cell();
                        if (cell.ColSpan > 1)
                            box = box.ColumnSpan((uint)Math.Min(cell.ColSpan, columns));
                        var background = cell.Background ?? (cell.Header ? "#e7eef8" : "#ffffff");
                        box.Border(0.4f).BorderColor("#c5c9d1").Background(background).Padding(3).Text(text =>
                        {
                            text.DefaultTextStyle(style => style.FontSize(7).FontColor("#222"));
                            if (cell.Runs.Count == 0 || cell.Runs.All(run => string.IsNullOrWhiteSpace(run.Text)))
                            {
                                text.Span(" ");
                                return;
                            }
                            foreach (var run in cell.Runs)
                            {
                                if (run.Text.Length == 0) continue;
                                var span = text.Span(run.Text);
                                if (run.Bold || cell.Header) span = span.Bold();
                                if (run.Italic) span = span.Italic();
                                if (run.Underline) span = span.Underline();
                                if (run.Strike) span = span.Strikethrough();
                                if (!string.IsNullOrEmpty(run.Color)) span = span.FontColor(run.Color);
                                if (!string.IsNullOrEmpty(run.Background)) span = span.BackgroundColor(run.Background);
                            }
                        });
                    }
                }
            });
        }

        private sealed class PdfRun
        {
            public string Text { get; init; } = "";
            public bool Bold { get; init; }
            public bool Italic { get; init; }
            public bool Underline { get; init; }
            public bool Strike { get; init; }
            public string? Color { get; init; }
            public string? Background { get; init; }
        }

        private sealed class PdfParagraph : PdfNode
        {
            public List<PdfRun> Runs { get; init; } = [];
            public bool Bullet { get; init; }
            public bool Heading { get; init; }
        }

        private sealed class PdfCell
        {
            public List<PdfRun> Runs { get; } = [];
            public bool Header { get; init; }
            public string? Background { get; init; }
            public int ColSpan { get; init; } = 1;
        }

        private sealed class PdfGrid : PdfNode
        {
            public List<List<PdfCell>> Rows { get; } = [];
        }

        private abstract class PdfNode;

        private sealed class PdfStyle
        {
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public bool Strike;
            public string? Color;
            public string? Background;

            public PdfStyle Clone() => new()
            {
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Strike = Strike,
                Color = Color,
                Background = Background
            };
        }

        private readonly record struct HtmlTok(bool IsTag, bool Closing, string Name, string Attrs, string Text);

        private static List<PdfNode> ParseRichNodes(string html)
        {
            var tokens = TokenizeHtml(html);
            var index = 0;
            return ParseBlocks(tokens, ref index, null, new PdfStyle());
        }

        private static List<HtmlTok> TokenizeHtml(string html)
        {
            var tokens = new List<HtmlTok>();
            foreach (Match match in Regex.Matches(html, @"(</?)([a-zA-Z0-9]+)([^>]*)>|([^<]+)", RegexOptions.IgnoreCase))
            {
                if (match.Groups[4].Success)
                {
                    tokens.Add(new HtmlTok(false, false, "", "", match.Groups[4].Value));
                    continue;
                }
                var name = match.Groups[2].Value.ToLowerInvariant();
                var closing = match.Groups[1].Value.StartsWith("</", StringComparison.Ordinal);
                tokens.Add(new HtmlTok(true, closing, name, match.Groups[3].Value, ""));
            }
            return tokens;
        }

        private static bool IsInlineTag(string name) =>
            name is "b" or "strong" or "i" or "em" or "u" or "s" or "strike" or "span" or "font" or "sub" or "sup";

        private static bool IsBlockTag(string name) =>
            name is "p" or "div" or "li" or "h1" or "h2" or "h3" or "h4" or "h5" or "h6";

        private static List<PdfNode> ParseBlocks(List<HtmlTok> tokens, ref int index, string? until, PdfStyle style)
        {
            var nodes = new List<PdfNode>();
            var runs = new List<PdfRun>();
            var current = style.Clone();
            var stack = new Stack<PdfStyle>();

            void Flush(bool bullet, bool heading)
            {
                if (runs.Count == 0 || runs.All(run => string.IsNullOrWhiteSpace(run.Text)))
                {
                    runs.Clear();
                    return;
                }
                nodes.Add(new PdfParagraph { Runs = runs.ToList(), Bullet = bullet, Heading = heading });
                runs.Clear();
            }

            while (index < tokens.Count)
            {
                var token = tokens[index++];
                if (!token.IsTag)
                {
                    AddPdfRun(runs, current, token.Text);
                    continue;
                }

                var name = token.Name;
                if (token.Closing)
                {
                    if (until != null && name == until)
                    {
                        Flush(until == "li", until.StartsWith('h'));
                        return nodes;
                    }
                    if (IsInlineTag(name) && stack.Count > 0)
                        current = stack.Pop();
                    else if (IsBlockTag(name))
                        Flush(name == "li", name.StartsWith('h'));
                    continue;
                }

                if (name is "br")
                {
                    Flush(until == "li", until != null && until.StartsWith('h'));
                    continue;
                }
                if (name == "table")
                {
                    Flush(false, false);
                    nodes.Add(ParseGrid(tokens, ref index));
                    continue;
                }
                if (IsBlockTag(name))
                {
                    Flush(false, false);
                    var blockStyle = current.Clone();
                    ApplyAttrs(blockStyle, token.Attrs);
                    if (name.StartsWith('h')) blockStyle.Bold = true;
                    nodes.AddRange(ParseBlocks(tokens, ref index, name, blockStyle));
                    continue;
                }
                if (IsInlineTag(name))
                {
                    stack.Push(current.Clone());
                    current = current.Clone();
                    if (name is "b" or "strong") current.Bold = true;
                    if (name is "i" or "em") current.Italic = true;
                    if (name == "u") current.Underline = true;
                    if (name is "s" or "strike") current.Strike = true;
                    ApplyAttrs(current, token.Attrs);
                }
            }

            Flush(false, false);
            return nodes;
        }

        private static PdfGrid ParseGrid(List<HtmlTok> tokens, ref int index)
        {
            var grid = new PdfGrid();
            List<PdfCell>? row = null;
            while (index < tokens.Count)
            {
                var token = tokens[index++];
                if (!token.IsTag)
                    continue;
                var name = token.Name;
                if (token.Closing && name == "table")
                    break;
                if (!token.Closing && name == "tr")
                {
                    row = [];
                    grid.Rows.Add(row);
                    continue;
                }
                if (!token.Closing && name is "td" or "th")
                {
                    row ??= [];
                    if (grid.Rows.Count == 0 || grid.Rows[^1] != row)
                        grid.Rows.Add(row);
                    var span = 1;
                    var spanMatch = Regex.Match(token.Attrs, @"colspan\s*=\s*[""']?(\d+)", RegexOptions.IgnoreCase);
                    if (spanMatch.Success) span = Math.Max(1, int.Parse(spanMatch.Groups[1].Value));
                    var cellStyle = new PdfStyle();
                    ApplyAttrs(cellStyle, token.Attrs);
                    var cell = new PdfCell
                    {
                        Header = name == "th",
                        ColSpan = span,
                        Background = cellStyle.Background
                    };
                    foreach (var node in ParseBlocks(tokens, ref index, name, cellStyle))
                    {
                        if (node is not PdfParagraph paragraph) continue;
                        if (cell.Runs.Count > 0)
                            cell.Runs.Add(new PdfRun { Text = " " });
                        cell.Runs.AddRange(paragraph.Runs);
                    }
                    row.Add(cell);
                }
            }
            return grid;
        }

        private static void AddPdfRun(List<PdfRun> runs, PdfStyle style, string raw)
        {
            var text = WebUtility.HtmlDecode(raw).Replace('\u00a0', ' ').Replace("\u200b", "").Replace('\r', ' ').Replace('\n', ' ');
            if (text.Length == 0) return;
            if (string.IsNullOrWhiteSpace(text))
            {
                if (runs.Count > 0 && !runs[^1].Text.EndsWith(' '))
                    runs.Add(new PdfRun { Text = " " });
                return;
            }
            var collapsed = Regex.Replace(text, @"[ \t]{2,}", " ");
            runs.Add(new PdfRun
            {
                Text = collapsed,
                Bold = style.Bold,
                Italic = style.Italic,
                Underline = style.Underline,
                Strike = style.Strike,
                Color = style.Color,
                Background = style.Background
            });
        }

        private static void ApplyAttrs(PdfStyle style, string attrs)
        {
            var colorAttr = Regex.Match(attrs, @"\bcolor\s*=\s*[""']?([^""'\s>]+)", RegexOptions.IgnoreCase);
            if (colorAttr.Success)
            {
                var color = NormalizePdfColor(colorAttr.Groups[1].Value);
                if (color != null) style.Color = color;
            }
            var styleAttr = Regex.Match(attrs, @"\bstyle\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase);
            if (!styleAttr.Success) return;
            foreach (var part in styleAttr.Groups[1].Value.Split(';'))
            {
                var split = part.Split(':', 2);
                if (split.Length < 2) continue;
                var prop = split[0].Trim().ToLowerInvariant();
                var val = split[1].Trim();
                if (prop is "color")
                {
                    var color = NormalizePdfColor(val);
                    if (color != null) style.Color = color;
                }
                else if (prop is "background-color" or "background")
                {
                    var color = NormalizePdfColor(val);
                    if (color != null) style.Background = color;
                }
                else if (prop == "font-weight")
                {
                    if (Regex.IsMatch(val, @"^(bold|[6-9]00)$", RegexOptions.IgnoreCase)) style.Bold = true;
                    else if (Regex.IsMatch(val, @"^(normal|[1-5]00)$", RegexOptions.IgnoreCase)) style.Bold = false;
                }
                else if (prop == "font-style")
                    style.Italic = Regex.IsMatch(val, @"italic|oblique", RegexOptions.IgnoreCase);
                else if (prop is "text-decoration" or "text-decoration-line")
                {
                    style.Underline = val.Contains("underline", StringComparison.OrdinalIgnoreCase);
                    style.Strike = val.Contains("line-through", StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private static string? NormalizePdfColor(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var value = raw.Trim().TrimEnd(';');
            if (value.Equals("transparent", StringComparison.OrdinalIgnoreCase) || value.Equals("inherit", StringComparison.OrdinalIgnoreCase))
                return null;
            var rgb = Regex.Match(value, @"rgba?\(\s*(\d+)[,\s]+(\d+)[,\s]+(\d+)", RegexOptions.IgnoreCase);
            if (rgb.Success)
                return $"#{byte.Parse(rgb.Groups[1].Value):X2}{byte.Parse(rgb.Groups[2].Value):X2}{byte.Parse(rgb.Groups[3].Value):X2}";
            if (Regex.IsMatch(value, @"^#[0-9a-f]{6}$", RegexOptions.IgnoreCase))
                return value;
            if (Regex.IsMatch(value, @"^#[0-9a-f]{3}$", RegexOptions.IgnoreCase))
            {
                var hex = value[1..];
                return $"#{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
            }
            return value.ToLowerInvariant() switch
            {
                "black" => "#222222",
                "white" => "#ffffff",
                "blue" => "#0000ff",
                "red" => "#ff0000",
                "yellow" => "#ffff00",
                "green" => "#008000",
                _ => null
            };
        }

        private static void RenderSubsectionItem(ColumnDescriptor body, Sec01SubsectionItem row)
        {
            if (string.IsNullOrWhiteSpace(row.Heading) &&
                string.IsNullOrWhiteSpace(row.Description) &&
                !Sec01SubsectionCodec.HasFieldContent(row))
                return;

            if (ShouldPrintHeading(row.Heading, row.Description))
                body.Item().PaddingTop(4).Text(row.Heading).Bold().FontColor("#e60a36").FontSize(8);
            if (!string.IsNullOrWhiteSpace(row.Description))
                WriteRichText(body, row.Description);
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
                        header.Cell().Background("#fdecef").Border(0.5f).BorderColor("#ccc")
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
