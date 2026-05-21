using SkiaSharp;
using Svg.Skia;

namespace ChemicalSDS.Services;

/// <summary>Loads wwwroot images for QuestPDF (raster + SVG→PNG).</summary>
public static class SdsPdfImageHelper
{
    public static string? ResolveFullPath(string? webRoot, string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(webRoot) || string.IsNullOrWhiteSpace(relativePath))
            return null;

        var rel = relativePath.TrimStart('/', '\\');
        var full = Path.Combine(webRoot, rel);
        return File.Exists(full) ? full : null;
    }

    public static byte[]? LoadImageBytes(string? webRoot, string? relativePath, int maxWidth = 120)
    {
        var full = ResolveFullPath(webRoot, relativePath);
        if (full == null)
            return null;

        var ext = Path.GetExtension(full).ToLowerInvariant();
        return ext switch
        {
            ".svg" => RasterizeSvg(full, maxWidth),
            ".png" or ".jpg" or ".jpeg" or ".webp" or ".gif" => File.ReadAllBytes(full),
            _ => null
        };
    }

    private static byte[]? RasterizeSvg(string path, int width)
    {
        try
        {
            var svg = new SKSvg();
            if (svg.Load(path) is null || svg.Picture is null)
                return null;

            var bounds = svg.Picture.CullRect;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return null;

            var scale = width / bounds.Width;
            var height = Math.Max(1, (int)(bounds.Height * scale));

            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);
            canvas.Scale(scale);
            canvas.DrawPicture(svg.Picture);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data?.ToArray();
        }
        catch
        {
            return null;
        }
    }
}
