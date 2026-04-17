using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

[ApiController]
[Route("api/cover")]
public class CoverController : ControllerBase
{
    [HttpGet]
    public IActionResult Generate(string title, string artist, int seed = 1234)
    {
        var random = new Random(seed);
        const int width = 300;
        const int height = 300;

        using var image = new Image<Rgba32>(width, height);
        var palette = CreatePalette(random);
        int style = Math.Abs(seed) % 4;

        image.Mutate(ctx =>
        {
            ctx.Fill(palette.Background);
            DrawBackgroundGlow(ctx, width, height, palette, random);

            switch (style)
            {
                case 0:
                    DrawStripes(ctx, width, height, palette, random);
                    break;
                case 1:
                    DrawSunset(ctx, width, height, palette, random);
                    break;
                case 2:
                    DrawGrid(ctx, width, height, palette, random);
                    break;
                default:
                    DrawOrbit(ctx, width, height, palette, random);
                    break;
            }

            ctx.Fill(palette.Shadow.WithAlpha(0.78f), new RectangularPolygon(18, 188, width - 36, 94));
            ctx.Draw(palette.Highlight.WithAlpha(0.45f), 2, new RectangularPolygon(18, 188, width - 36, 94));
        });

        TryDrawText(image, title, artist, width, palette);

        var ms = new MemoryStream();
        image.SaveAsPng(ms);
        ms.Seek(0, SeekOrigin.Begin);
        return File(ms, "image/png");
    }

    private static void DrawBackgroundGlow(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        ctx.Fill(palette.Highlight.WithAlpha(0.18f), new EllipsePolygon(width * 0.72f, height * 0.18f, random.Next(70, 120)));
        ctx.Fill(palette.Accent.WithAlpha(0.12f), new EllipsePolygon(width * 0.18f, height * 0.82f, random.Next(90, 150)));
    }

    private static void DrawStripes(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        for (int i = -2; i < 8; i++)
        {
            float stripeWidth = random.Next(24, 42);
            float x = i * 44f + random.Next(-10, 10);

            var path = new PathBuilder();
            path.AddLine(x, 0, x + stripeWidth, 0);
            path.AddLine(x + stripeWidth - 26, height, x - 26, height);
            path.CloseFigure();

            var color = i % 2 == 0 ? palette.Accent : palette.Secondary;
            ctx.Fill(color.WithAlpha(0.7f), path.Build());
        }

        ctx.Fill(palette.Highlight.WithAlpha(0.16f), new RectangularPolygon(0, height * 0.62f, width, height * 0.12f));
    }

    private static void DrawSunset(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        float horizon = height * 0.58f;

        for (int i = 0; i < 7; i++)
        {
            float bandHeight = 14 + i * 6;
            var color = i % 2 == 0 ? palette.Accent : palette.Secondary;
            ctx.Fill(color.WithAlpha(0.18f + i * 0.05f), new RectangularPolygon(0, horizon - bandHeight, width, 5));
        }

        ctx.Fill(palette.Highlight.WithAlpha(0.82f), new EllipsePolygon(width * 0.5f, horizon - 12, 58));
        ctx.Fill(palette.Background.WithAlpha(0.92f), new RectangularPolygon(0, horizon, width, height - horizon));

        var mountain = new PathBuilder();
        mountain.AddLine(0, horizon + 20, width * 0.24f, horizon - random.Next(12, 38));
        mountain.AddLine(width * 0.24f, horizon - random.Next(12, 38), width * 0.48f, horizon + 26);
        mountain.AddLine(width * 0.48f, horizon + 26, width * 0.7f, horizon - random.Next(28, 54));
        mountain.AddLine(width * 0.7f, horizon - random.Next(28, 54), width, horizon + 12);
        mountain.AddLine(width, horizon + 12, width, height);
        mountain.AddLine(width, height, 0, height);
        mountain.CloseFigure();

        ctx.Fill(palette.Shadow.WithAlpha(0.96f), mountain.Build());
    }

    private static void DrawGrid(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        const int margin = 28;
        const int cell = 34;

        ctx.Draw(palette.Secondary.WithAlpha(0.32f), 2, new RectangularPolygon(margin, margin, width - margin * 2, height - margin * 2));

        for (int x = margin; x < width - margin; x += cell)
        {
            ctx.DrawLine(palette.Secondary.WithAlpha(0.28f), 1, new PointF(x, margin), new PointF(x, height - margin));
        }

        for (int y = margin; y < height - margin; y += cell)
        {
            ctx.DrawLine(palette.Secondary.WithAlpha(0.28f), 1, new PointF(margin, y), new PointF(width - margin, y));
        }

        for (int i = 0; i < 5; i++)
        {
            float rectX = margin + random.Next(0, 5) * cell;
            float rectY = margin + random.Next(0, 5) * cell;
            float rectW = cell * random.Next(2, 4);
            float rectH = cell * random.Next(1, 3);
            var color = i % 2 == 0 ? palette.Accent : palette.Highlight;
            ctx.Fill(color.WithAlpha(0.72f), new RectangularPolygon(rectX, rectY, rectW, rectH));
        }
    }

    private static void DrawOrbit(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        var center = new PointF(width * 0.5f, height * 0.42f);
        ctx.Fill(palette.Highlight.WithAlpha(0.85f), new EllipsePolygon(center, 26 + random.Next(8, 18)));

        for (int i = 0; i < 4; i++)
        {
            float orbitWidth = 60 + i * 26;
            float orbitHeight = 22 + i * 10;
            var orbit = new EllipsePolygon(center.X, center.Y, orbitWidth, orbitHeight);
            var color = i % 2 == 0 ? palette.Accent : palette.Secondary;
            ctx.Draw(color.WithAlpha(0.55f), 3, orbit);
        }

        for (int i = 0; i < 6; i++)
        {
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float radius = 44 + random.Next(18, 92);
            float x = center.X + MathF.Cos(angle) * radius;
            float y = center.Y + MathF.Sin(angle) * (radius * 0.45f);
            ctx.Fill(palette.Secondary.WithAlpha(0.8f), new EllipsePolygon(x, y, random.Next(4, 9)));
        }
    }

    private static CoverPalette CreatePalette(Random random)
    {
        var background = Color.FromRgb(
            (byte)random.Next(18, 70),
            (byte)random.Next(24, 90),
            (byte)random.Next(40, 120));

        var accent = Color.FromRgb(
            (byte)random.Next(190, 255),
            (byte)random.Next(90, 220),
            (byte)random.Next(90, 220));

        var secondary = Color.FromRgb(
            (byte)random.Next(90, 180),
            (byte)random.Next(180, 255),
            (byte)random.Next(160, 255));

        var highlight = Color.FromRgb(
            (byte)random.Next(235, 255),
            (byte)random.Next(220, 255),
            (byte)random.Next(190, 255));

        return new CoverPalette(background, accent, secondary, highlight, Color.Black);
    }

    private static void TryDrawText(Image<Rgba32> image, string title, string artist, int width, CoverPalette palette)
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
                }, title, palette.Highlight);

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

    private sealed record CoverPalette(Color Background, Color Accent, Color Secondary, Color Highlight, Color Shadow);
}
