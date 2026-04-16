using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;

[ApiController]
[Route("api/cover")]
public class CoverController : ControllerBase
{
    [HttpGet]
    public IActionResult Generate(string title, string artist, int seed = 1234)
    {
        var random = new Random(seed);

        int width = 300;
        int height = 300;

        using var image = new Image<Rgba32>(width, height);

        var baseColor = Color.FromRgb(
            (byte)random.Next(50, 200),
            (byte)random.Next(50, 200),
            (byte)random.Next(50, 200)
        );

        var accentColor = Color.FromRgb(
            (byte)random.Next(120, 255),
            (byte)random.Next(120, 255),
            (byte)random.Next(120, 255)
        );

        image.Mutate(ctx =>
        {
            ctx.Fill(baseColor);

            for (int i = 0; i < 8; i++)
            {
                var x = random.Next(-50, width);
                var y = random.Next(-50, height);
                var ellipse = new EllipsePolygon(x, y, random.Next(30, 120));
                ctx.Fill(accentColor.WithAlpha(0.12f + (float)random.NextDouble() * 0.18f), ellipse);
            }

            for (int i = 0; i < 6; i++)
            {
                var points = new PointF[]
                {
                    new(random.Next(width), random.Next(height)),
                    new(random.Next(width), random.Next(height)),
                    new(random.Next(width), random.Next(height))
                };
                ctx.DrawLine(accentColor.WithAlpha(0.4f), random.Next(3, 8), points);
            }
        });

        image.Mutate(ctx =>
        {
            ctx.Fill(Color.Black.WithAlpha(0.18f), new RectangularPolygon(18, 188, width - 36, 88));
        });

        TryDrawText(image, title, artist, width);

        var ms = new MemoryStream();
        image.SaveAsPng(ms);
        ms.Seek(0, SeekOrigin.Begin);

        return File(ms, "image/png");
    }

    private static void TryDrawText(Image<Rgba32> image, string title, string artist, int width)
    {
        try
        {
            var titleFont = CreateSafeFont(26, FontStyle.Bold);
            var artistFont = CreateSafeFont(18);

            image.Mutate(ctx =>
            {
                ctx.DrawText(new RichTextOptions(titleFont)
                {
                    Origin = new PointF(24, 198),
                    WrappingLength = width - 48
                }, title, Color.White);
                ctx.DrawText(new RichTextOptions(artistFont)
                {
                    Origin = new PointF(24, 246),
                    WrappingLength = width - 48
                }, artist, Color.WhiteSmoke);
            });
        }
        catch
        {
        }
    }

    private static Font CreateSafeFont(float size, FontStyle style = FontStyle.Regular)
    {
        var preferredFamilies = new[]
        {
            "DejaVu Sans",
            "Liberation Sans",
            "Noto Sans",
            "Arial"
        };

        foreach (var familyName in preferredFamilies)
        {
            try
            {
                return SystemFonts.CreateFont(familyName, size, style);
            }
            catch
            {
            }
        }

        if (SystemFonts.Collection.Families.Any())
        {
            var fallbackFamily = SystemFonts.Collection.Families.First();
            return fallbackFamily.CreateFont(size, style);
        }

        throw new InvalidOperationException("No system fonts are available.");
    }
}
