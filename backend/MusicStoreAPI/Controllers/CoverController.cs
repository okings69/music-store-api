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
    public IActionResult Generate(string title, string artist, string? genre = null, int seed = 1234)
    {
        var random = new Random(seed);
        const int width = 300;
        const int height = 300;

        using var image = new Image<Rgba32>(width, height);
        var style = CreateStyle(genre, random);

        image.Mutate(ctx =>
        {
            ctx.Fill(style.Palette.Background);
            DrawBackgroundGlow(ctx, width, height, style.Palette, random);
            DrawGenreArt(ctx, width, height, style, random);
            DrawFrame(ctx, width, height, style.Palette);
            DrawLabelPanel(ctx, width, height, style.Palette);
            DrawGenreBadge(ctx, genre, style, random);
        });

        TryDrawText(image, title, artist, width, style.Palette);

        var ms = new MemoryStream();
        image.SaveAsPng(ms);
        ms.Seek(0, SeekOrigin.Begin);
        return File(ms, "image/png");
    }

    private static void DrawGenreArt(IImageProcessingContext ctx, int width, int height, CoverStyle style, Random random)
    {
        switch (style.Layout)
        {
            case CoverLayout.SharpStripes:
                DrawSharpStripes(ctx, width, height, style.Palette, random);
                break;
            case CoverLayout.NeonGrid:
                DrawNeonGrid(ctx, width, height, style.Palette, random);
                break;
            case CoverLayout.BrassClub:
                DrawBrassClub(ctx, width, height, style.Palette, random);
                break;
            case CoverLayout.ChamberWaves:
                DrawChamberWaves(ctx, width, height, style.Palette, random);
                break;
            case CoverLayout.StencilBurst:
                DrawStencilBurst(ctx, width, height, style.Palette, random);
                break;
            case CoverLayout.SoftBlocks:
                DrawSoftBlocks(ctx, width, height, style.Palette, random);
                break;
            default:
                DrawOrbitPoster(ctx, width, height, style.Palette, random);
                break;
        }
    }

    private static void DrawBackgroundGlow(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        ctx.Fill(palette.Highlight.WithAlpha(0.18f), new EllipsePolygon(width * 0.74f, height * 0.18f, random.Next(70, 120)));
        ctx.Fill(palette.Accent.WithAlpha(0.1f), new EllipsePolygon(width * 0.16f, height * 0.82f, random.Next(90, 150)));
    }

    private static void DrawSharpStripes(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        for (int i = -2; i < 9; i++)
        {
            float stripeWidth = random.Next(20, 34);
            float x = i * 38f + random.Next(-8, 8);

            var path = new PathBuilder();
            path.AddLine(x, 0, x + stripeWidth, 0);
            path.AddLine(x + stripeWidth - 34, height, x - 34, height);
            path.CloseFigure();

            var color = i % 2 == 0 ? palette.Accent : palette.Secondary;
            ctx.Fill(color.WithAlpha(0.78f), path.Build());
        }

        ctx.DrawLine(palette.Highlight.WithAlpha(0.6f), 3, new PointF(0, height * 0.34f), new PointF(width, height * 0.56f));
        ctx.DrawLine(palette.Highlight.WithAlpha(0.35f), 2, new PointF(0, height * 0.18f), new PointF(width, height * 0.42f));
    }

    private static void DrawNeonGrid(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        const int margin = 28;
        const int cell = 30;

        for (int x = margin; x < width - margin; x += cell)
        {
            ctx.DrawLine(palette.Secondary.WithAlpha(0.28f), 1, new PointF(x, margin), new PointF(x, height - margin - 40));
        }

        for (int y = margin; y < height - margin - 40; y += cell)
        {
            ctx.DrawLine(palette.Secondary.WithAlpha(0.28f), 1, new PointF(margin, y), new PointF(width - margin, y));
        }

        for (int i = 0; i < 6; i++)
        {
            float rectX = margin + random.Next(0, 6) * cell;
            float rectY = margin + random.Next(0, 5) * cell;
            float rectW = cell * random.Next(2, 4);
            float rectH = cell * random.Next(1, 3);
            ctx.Fill((i % 2 == 0 ? palette.Accent : palette.Highlight).WithAlpha(0.72f), new RectangularPolygon(rectX, rectY, rectW, rectH));
        }

        var diamond = new PathBuilder();
        diamond.AddLine(width * 0.5f, 42, width - 54, height * 0.42f);
        diamond.AddLine(width - 54, height * 0.42f, width * 0.5f, height * 0.74f);
        diamond.AddLine(width * 0.5f, height * 0.74f, 54, height * 0.42f);
        diamond.CloseFigure();

        ctx.Draw(palette.Highlight.WithAlpha(0.75f), 4, diamond.Build());
    }

    private static void DrawBrassClub(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        float horizon = height * 0.62f;
        ctx.Fill(palette.Shadow.WithAlpha(0.85f), new RectangularPolygon(0, horizon, width, height - horizon));

        for (int i = 0; i < 5; i++)
        {
            float radius = 26 + i * 16;
            ctx.Draw(palette.Highlight.WithAlpha(0.14f + i * 0.08f), 2, new EllipsePolygon(width * 0.5f, height * 0.4f, radius, radius * 0.72f));
        }

        var spotlight = new PathBuilder();
        spotlight.AddLine(width * 0.16f, 0, width * 0.42f, height * 0.56f);
        spotlight.AddLine(width * 0.42f, height * 0.56f, width * 0.68f, 0);
        spotlight.CloseFigure();
        ctx.Fill(palette.Accent.WithAlpha(0.16f), spotlight.Build());

        for (int i = 0; i < 3; i++)
        {
            float x = 46 + i * 78 + random.Next(-8, 8);
            ctx.Fill(palette.Highlight.WithAlpha(0.8f), new EllipsePolygon(x, horizon - 26, 10, 34));
            ctx.Fill(palette.Accent.WithAlpha(0.72f), new EllipsePolygon(x + 14, horizon - 40, 22));
        }
    }

    private static void DrawChamberWaves(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        for (int i = 0; i < 7; i++)
        {
            float y = 28 + i * 22;
            var curve = new PathBuilder();
            curve.AddQuadraticBezier(
                new PointF(0, y + random.Next(-8, 8)),
                new PointF(width * 0.35f, y - random.Next(10, 24)),
                new PointF(width, y + random.Next(-8, 8)));
            ctx.Draw((i % 2 == 0 ? palette.Secondary : palette.Highlight).WithAlpha(0.48f), 3, curve.Build());
        }

        ctx.Draw(palette.Accent.WithAlpha(0.8f), 3, new RectangularPolygon(36, 36, width - 72, height - 128));

        for (int i = 0; i < 4; i++)
        {
            float inset = 54 + i * 20;
            ctx.Draw(palette.Highlight.WithAlpha(0.16f), 1, new RectangularPolygon(inset, inset, width - inset * 2, height - 150));
        }
    }

    private static void DrawStencilBurst(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        var center = new PointF(width * 0.5f, height * 0.36f);

        for (int i = 0; i < 12; i++)
        {
            float angle = i * (MathF.PI * 2 / 12f) + (float)random.NextDouble() * 0.14f;
            float inner = 26 + random.Next(8, 18);
            float outer = 82 + random.Next(18, 40);

            var shard = new PathBuilder();
            shard.AddLine(
                center.X + MathF.Cos(angle - 0.08f) * inner,
                center.Y + MathF.Sin(angle - 0.08f) * inner,
                center.X + MathF.Cos(angle) * outer,
                center.Y + MathF.Sin(angle) * outer);
            shard.AddLine(
                center.X + MathF.Cos(angle) * outer,
                center.Y + MathF.Sin(angle) * outer,
                center.X + MathF.Cos(angle + 0.08f) * inner,
                center.Y + MathF.Sin(angle + 0.08f) * inner);
            shard.CloseFigure();

            var color = i % 3 == 0 ? palette.Highlight : palette.Accent;
            ctx.Fill(color.WithAlpha(0.72f), shard.Build());
        }

        ctx.Fill(palette.Shadow.WithAlpha(0.88f), new EllipsePolygon(center, 24));
    }

    private static void DrawSoftBlocks(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        for (int i = 0; i < 6; i++)
        {
            float x = 26 + i * 36 + random.Next(-6, 6);
            float y = 24 + random.Next(0, 110);
            float blockHeight = 80 + random.Next(12, 72);
            float blockWidth = random.Next(24, 40);
            var color = i % 2 == 0 ? palette.Secondary : palette.Accent;
            ctx.Fill(color.WithAlpha(0.5f), new RectangularPolygon(x, y, blockWidth, blockHeight));
        }

        ctx.Fill(palette.Highlight.WithAlpha(0.14f), new RectangularPolygon(20, 30, width - 40, 112));
        ctx.DrawLine(palette.Highlight.WithAlpha(0.42f), 2, new PointF(20, 144), new PointF(width - 20, 144));
    }

    private static void DrawOrbitPoster(IImageProcessingContext ctx, int width, int height, CoverPalette palette, Random random)
    {
        var center = new PointF(width * 0.5f, height * 0.42f);
        ctx.Fill(palette.Highlight.WithAlpha(0.86f), new EllipsePolygon(center, 26 + random.Next(8, 18)));

        for (int i = 0; i < 4; i++)
        {
            float orbitWidth = 60 + i * 26;
            float orbitHeight = 22 + i * 10;
            var orbit = new EllipsePolygon(center.X, center.Y, orbitWidth, orbitHeight);
            var color = i % 2 == 0 ? palette.Accent : palette.Secondary;
            ctx.Draw(color.WithAlpha(0.55f), 3, orbit);
        }

        for (int i = 0; i < 8; i++)
        {
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float radius = 42 + random.Next(18, 94);
            float x = center.X + MathF.Cos(angle) * radius;
            float y = center.Y + MathF.Sin(angle) * (radius * 0.45f);
            ctx.Fill(palette.Secondary.WithAlpha(0.82f), new EllipsePolygon(x, y, random.Next(3, 8)));
        }
    }

    private static void DrawFrame(IImageProcessingContext ctx, int width, int height, CoverPalette palette)
    {
        ctx.Draw(palette.Highlight.WithAlpha(0.28f), 2, new RectangularPolygon(12, 12, width - 24, height - 24));
        ctx.Draw(palette.Secondary.WithAlpha(0.18f), 1, new RectangularPolygon(20, 20, width - 40, height - 40));
    }

    private static void DrawLabelPanel(IImageProcessingContext ctx, int width, int height, CoverPalette palette)
    {
        ctx.Fill(palette.Shadow.WithAlpha(0.82f), new RectangularPolygon(18, 188, width - 36, 94));
        ctx.Draw(palette.Highlight.WithAlpha(0.45f), 2, new RectangularPolygon(18, 188, width - 36, 94));
    }

    private static void DrawGenreBadge(IImageProcessingContext ctx, string? genre, CoverStyle style, Random random)
    {
        var badgeWidth = 88f + random.Next(0, 12);
        ctx.Fill(style.Palette.Accent.WithAlpha(0.82f), new RectangularPolygon(18, 18, badgeWidth, 24));
        ctx.Draw(style.Palette.Highlight.WithAlpha(0.7f), 1, new RectangularPolygon(18, 18, badgeWidth, 24));

        var badgeText = NormalizeGenre(genre).ToUpperInvariant();
        if (badgeText.Length > 10)
        {
            badgeText = badgeText[..10];
        }

        try
        {
            var badgeFont = CreateSafeFont(12, FontStyle.Bold);
            using var badgeImage = new Image<Rgba32>(300, 300);
            badgeImage.Mutate(img =>
            {
                img.DrawText(new RichTextOptions(badgeFont)
                {
                    Origin = new PointF(24, 24),
                    WrappingLength = badgeWidth - 12
                }, badgeText, style.Palette.Background);
            });
            ctx.DrawImage(badgeImage, 1f);
        }
        catch
        {
        }
    }

    private static CoverStyle CreateStyle(string? genre, Random random)
    {
        var normalizedGenre = NormalizeGenre(genre);

        return normalizedGenre switch
        {
            "rock" => new CoverStyle(
                CoverLayout.SharpStripes,
                new CoverPalette(
                    Color.FromRgb(26, 18, 22),
                    Color.FromRgb(220, 72, 54),
                    Color.FromRgb(245, 214, 116),
                    Color.FromRgb(255, 245, 219),
                    Color.FromRgb(10, 8, 10))),
            "metal" => new CoverStyle(
                CoverLayout.StencilBurst,
                new CoverPalette(
                    Color.FromRgb(18, 18, 24),
                    Color.FromRgb(198, 56, 78),
                    Color.FromRgb(132, 138, 150),
                    Color.FromRgb(238, 236, 231),
                    Color.FromRgb(6, 6, 9))),
            "electronic" => new CoverStyle(
                CoverLayout.NeonGrid,
                new CoverPalette(
                    Color.FromRgb(14, 24, 58),
                    Color.FromRgb(255, 72, 152),
                    Color.FromRgb(73, 231, 255),
                    Color.FromRgb(227, 249, 255),
                    Color.FromRgb(4, 9, 24))),
            "jazz" => new CoverStyle(
                CoverLayout.BrassClub,
                new CoverPalette(
                    Color.FromRgb(34, 22, 18),
                    Color.FromRgb(188, 112, 34),
                    Color.FromRgb(87, 166, 150),
                    Color.FromRgb(252, 232, 183),
                    Color.FromRgb(12, 8, 7))),
            "classical" => new CoverStyle(
                CoverLayout.ChamberWaves,
                new CoverPalette(
                    Color.FromRgb(238, 232, 220),
                    Color.FromRgb(132, 93, 63),
                    Color.FromRgb(116, 142, 154),
                    Color.FromRgb(255, 252, 246),
                    Color.FromRgb(54, 42, 34))),
            "hip hop" => new CoverStyle(
                CoverLayout.StencilBurst,
                new CoverPalette(
                    Color.FromRgb(30, 24, 16),
                    Color.FromRgb(240, 174, 52),
                    Color.FromRgb(86, 210, 184),
                    Color.FromRgb(255, 244, 214),
                    Color.FromRgb(8, 8, 8))),
            "indie" => new CoverStyle(
                CoverLayout.SoftBlocks,
                new CoverPalette(
                    Color.FromRgb(44, 60, 78),
                    Color.FromRgb(227, 126, 97),
                    Color.FromRgb(146, 194, 171),
                    Color.FromRgb(248, 239, 220),
                    Color.FromRgb(18, 28, 38))),
            "pop" => new CoverStyle(
                CoverLayout.SoftBlocks,
                new CoverPalette(
                    Color.FromRgb(255, 207, 193),
                    Color.FromRgb(255, 116, 154),
                    Color.FromRgb(94, 191, 191),
                    Color.FromRgb(255, 249, 241),
                    Color.FromRgb(92, 68, 86))),
            _ => random.Next(3) switch
            {
                0 => new CoverStyle(
                    CoverLayout.OrbitPoster,
                    new CoverPalette(
                        Color.FromRgb(22, 40, 78),
                        Color.FromRgb(255, 126, 92),
                        Color.FromRgb(114, 224, 221),
                        Color.FromRgb(247, 248, 233),
                        Color.FromRgb(9, 15, 32))),
                1 => new CoverStyle(
                    CoverLayout.SoftBlocks,
                    new CoverPalette(
                        Color.FromRgb(50, 55, 78),
                        Color.FromRgb(227, 150, 104),
                        Color.FromRgb(120, 182, 197),
                        Color.FromRgb(249, 241, 225),
                        Color.FromRgb(20, 24, 36))),
                _ => new CoverStyle(
                    CoverLayout.ChamberWaves,
                    new CoverPalette(
                        Color.FromRgb(226, 230, 222),
                        Color.FromRgb(128, 110, 76),
                        Color.FromRgb(108, 154, 165),
                        Color.FromRgb(255, 252, 245),
                        Color.FromRgb(62, 66, 70)))
            }
        };
    }

    private static string NormalizeGenre(string? genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
        {
            return "default";
        }

        var trimmed = genre.Trim().ToLowerInvariant();

        return trimmed switch
        {
            "elektronisch" => "electronic",
            "\u0435\u043b\u0435\u043a\u0442\u0440\u043e\u043d\u0456\u043a\u0430" => "electronic",
            "\u0434\u0436\u0430\u0437" => "jazz",
            "\u043a\u043b\u0430\u0441\u0438\u043a\u0430" => "classical",
            "\u0440\u043e\u043a" => "rock",
            "\u043c\u0435\u0442\u0430\u043b" => "metal",
            "\u0456\u043d\u0434\u0456" => "indie",
            "\u043f\u043e\u043f" => "pop",
            _ => trimmed
        };
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

    private sealed record CoverStyle(CoverLayout Layout, CoverPalette Palette);

    private enum CoverLayout
    {
        OrbitPoster,
        SharpStripes,
        NeonGrid,
        BrassClub,
        ChamberWaves,
        StencilBurst,
        SoftBlocks
    }
}
