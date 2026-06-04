using SkiaSharp;

namespace ChemicalSDS.Services;

/// <summary>Renders NFPA 704 diamond graphic for PDF (matches web .sds-nfpa-diamond layout).</summary>
public static class Nfpa704PdfRenderer
{
    private const int Cell = 72;
    private const int Gap = 2;
    private const int HalfDiamond = 36;

    public static byte[]? Render(string? flammability, string? health, string? reactivity, string? special)
    {
        var flam = DisplayValue(flammability);
        var h = DisplayValue(health);
        var r = DisplayValue(reactivity);
        var sp = string.IsNullOrWhiteSpace(special) || special == "—" ? "—" : special.Trim();

        var width = Cell * 2 + Gap;
        var height = Cell * 3 + Gap * 2;

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        var centerX = width / 2f;
        DrawDiamond(canvas, centerX, HalfDiamond, HalfDiamond, flam, "#e53935", SKColors.White);
        DrawDiamond(canvas, HalfDiamond, Cell + Gap + HalfDiamond, HalfDiamond, h, "#1565c0", SKColors.White);
        DrawDiamond(canvas, Cell + Gap + HalfDiamond, Cell + Gap + HalfDiamond, HalfDiamond, r, "#fdd835", SKColor.Parse("#222222"));
        DrawDiamond(canvas, centerX, (Cell + Gap) * 2 + HalfDiamond, HalfDiamond, sp, "#ffffff", SKColor.Parse("#333333"), stroke: true);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data?.ToArray();
    }

    private static string DisplayValue(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "—" : value.Trim();

    private static void DrawDiamond(
        SKCanvas canvas,
        float centerX,
        float centerY,
        float halfSize,
        string text,
        string fillHex,
        SKColor textColor,
        bool stroke = false)
    {
        using var path = new SKPath();
        path.MoveTo(centerX, centerY - halfSize);
        path.LineTo(centerX + halfSize, centerY);
        path.LineTo(centerX, centerY + halfSize);
        path.LineTo(centerX - halfSize, centerY);
        path.Close();

        using var fillPaint = new SKPaint
        {
            Color = SKColor.Parse(fillHex),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawPath(path, fillPaint);

        if (stroke)
        {
            using var strokePaint = new SKPaint
            {
                Color = SKColor.Parse("#999999"),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };
            canvas.DrawPath(path, strokePaint);
        }

        using var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
        using var font = new SKFont(typeface, 26);
        using var textPaint = new SKPaint { Color = textColor, IsAntialias = true };
        var textWidth = font.MeasureText(text);
        var metrics = font.Metrics;
        var textY = centerY - (metrics.Ascent + metrics.Descent) / 2f;
        canvas.DrawText(text, centerX - textWidth / 2f, textY, font, textPaint);
    }
}
