using Bogus;
using MusicStoreAPI.DTOs;
using MusicStoreAPI.Utils;

namespace MusicStoreAPI.Services;

public class SongService
{
    private static readonly Dictionary<string, string> LocaleMap = new()
    {
        ["en-US"] = "en",
        ["de-DE"] = "de",
        ["uk-UA"] = "uk"
    };

    private readonly IHttpContextAccessor _httpContextAccessor;

    public SongService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<SongDto> GenerateSongs(long seed, int page, double avgLikes, string locale)
    {
        var songs = new List<SongDto>();

        for (int i = 0; i < 20; i++)
        {
            int currentIndex = (page - 1) * 20 + i + 1;
            int songSeed = SeedHelper.CombineSeed(seed, page, currentIndex);
            var songRandom = new Random(songSeed);
            Randomizer.Seed = new Random(songSeed);

            var faker = new Faker(MapBogusLocale(locale));
            var title = GenererTitre(songRandom, faker, locale);
            var artist = GenererArtiste(songRandom, faker, locale);
            var album = songRandom.NextDouble() < 0.2 ? SingleLabel(locale) : GenererAlbum(songRandom, faker, locale);
            var genre = GenererGenre(songRandom, faker, locale);
            var likes = GenererLikes(songRandom, avgLikes);

            songs.Add(new SongDto
            {
                Id = currentIndex,
                Index = currentIndex,
                Title = title,
                Artist = artist,
                Album = album,
                Genre = genre,
                Likes = likes,
                CoverUrl = GenererCoverUrl(title, artist, genre, songSeed),
                AudioUrl = GenererAudioUrl(songSeed, genre),
                Review = GenererAvis(songRandom, locale, title, artist, album, genre, likes)
            });
        }

        return songs;
    }

    private string GenererTitre(Random random, Faker faker, string locale)
    {
        var adjectives = MotsTitre(locale, faker);
        var nouns = NomsTitre(locale, faker);
        var adjective = adjectives[random.Next(adjectives.Length)];
        var noun = nouns[random.Next(nouns.Length)];

        return random.NextDouble() > 0.5
            ? $"{adjective} {noun}"
            : $"{adjective} {noun} {random.Next(1, 100)}";
    }

    private string GenererArtiste(Random random, Faker faker, string locale)
    {
        if (random.NextDouble() < 0.72)
        {
            return faker.Name.FullName();
        }

        var descriptors = GroupDescriptors(locale, faker);
        var endings = GroupEndings(locale, faker);
        return $"{descriptors[random.Next(descriptors.Length)]} {endings[random.Next(endings.Length)]}";
    }

    private string GenererAlbum(Random random, Faker faker, string locale)
    {
        var places = AlbumPlaces(locale, faker);
        var moods = AlbumMoods(locale, faker);

        return random.NextDouble() < 0.45
            ? $"{moods[random.Next(moods.Length)]} {places[random.Next(places.Length)]}"
            : $"{faker.Lorem.Word()} {places[random.Next(places.Length)]}";
    }

    private string GenererGenre(Random random, Faker faker, string locale)
    {
        var genres = locale switch
        {
            "de-DE" => new[] { "Rock", "Pop", "Elektronisch", "Jazz", "Klassik", "Metal", "Indie" },
            "uk-UA" => new[]
            {
                "\u0420\u043e\u043a",
                "\u041f\u043e\u043f",
                "\u0415\u043b\u0435\u043a\u0442\u0440\u043e\u043d\u0456\u043a\u0430",
                "\u0414\u0436\u0430\u0437",
                "\u041a\u043b\u0430\u0441\u0438\u043a\u0430",
                "\u041c\u0435\u0442\u0430\u043b",
                "\u0406\u043d\u0434\u0456"
            },
            _ => new[] { "Rock", "Pop", "Electronic", "Jazz", "Classical", "Hip Hop", "Metal", "Indie" }
        };

        return faker.PickRandom(genres);
    }

    private static int GenererLikes(Random random, double avgLikes)
    {
        if (avgLikes <= 0)
        {
            return 0;
        }

        if (avgLikes >= 10)
        {
            return 10;
        }

        int floor = (int)Math.Floor(avgLikes);
        double probability = avgLikes - floor;
        return floor + (random.NextDouble() < probability ? 1 : 0);
    }

    private string GenererCoverUrl(string title, string artist, string genre, int seed)
    {
        var encodedTitle = Uri.EscapeDataString(title);
        var encodedArtist = Uri.EscapeDataString(artist);
        var encodedGenre = Uri.EscapeDataString(genre);
        return $"{GetApiBaseUrl()}/cover?title={encodedTitle}&artist={encodedArtist}&genre={encodedGenre}&seed={Math.Abs(seed)}";
    }

    private string GenererAudioUrl(int seed, string genre)
    {
        var encodedGenre = Uri.EscapeDataString(genre);
        return $"{GetApiBaseUrl()}/audio?seed={Math.Abs(seed)}&genre={encodedGenre}";
    }

    private string GetApiBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request is not null && request.Host.HasValue)
        {
            return $"{request.Scheme}://{request.Host}/api";
        }

        return "http://localhost:5000/api";
    }

    private static string GenererAvis(Random random, string locale, string title, string artist, string album, string genre, int likes)
    {
        return locale switch
        {
            "de-DE" => BuildGermanReview(random, title, artist, album, genre, likes),
            "uk-UA" => BuildUkrainianReview(random, title, artist, album, genre, likes),
            _ => BuildEnglishReview(random, title, artist, album, genre, likes)
        };
    }

    private static string BuildEnglishReview(Random random, string title, string artist, string album, string genre, int likes)
    {
        var opener = Pick(random, new[]
        {
            $"\"{title}\"",
            $"{artist}'s take on {genre.ToLowerInvariant()}",
            $"This cut from {album}",
            $"On \"{title}\", {artist}"
        });

        var focus = Pick(random, new[]
        {
            "leans on a strong melodic hook",
            "sells its mood with confident production",
            "balances polish and immediacy surprisingly well",
            "has enough texture to feel thought through",
            "stands out more for atmosphere than pure volume",
            "builds momentum without sounding crowded"
        });

        var detail = Pick(random, new[]
        {
            "The rhythm section keeps it moving.",
            "The arrangement leaves enough space for the main motif to breathe.",
            "Small production touches make the chorus land harder.",
            "It feels more deliberate than flashy, which helps.",
            "The contrast between sections gives it a memorable shape.",
            "Its best moments come from restraint rather than excess."
        });

        var closing = likes switch
        {
            >= 8 => Pick(random, new[]
            {
                "One of the more convincing tracks in the set.",
                "Easy to come back to after the first listen.",
                "It earns the enthusiasm around it."
            }),
            >= 5 => Pick(random, new[]
            {
                "Not flawless, but it has a clear identity.",
                "It may not be huge, yet it feels complete.",
                "There is enough personality here to keep it interesting."
            }),
            _ => Pick(random, new[]
            {
                "It does not fully stick, but the craft is there.",
                "A few sharper ideas would make it hit harder.",
                "More character would push it from decent to memorable."
            })
        };

        return $"{opener} {focus}. {detail} {closing}";
    }

    private static string BuildGermanReview(Random random, string title, string artist, string album, string genre, int likes)
    {
        var opener = Pick(random, new[]
        {
            $"\"{title}\"",
            $"{artist}s Blick auf {genre}",
            $"Dieser Track von {album}",
            $"Auf \"{title}\" zeigt {artist}"
        });

        var focus = Pick(random, new[]
        {
            "arbeitet mit einer klaren melodischen Idee",
            "setzt stark auf Stimmung und saubere Produktion",
            "wirkt kontrolliert, ohne steril zu klingen",
            "hat genug Struktur, um im Kopf zu bleiben",
            "lebt eher von Atmosphaere als von blosser Lautstaerke",
            "baut Spannung auf, ohne ueberladen zu wirken"
        });

        var detail = Pick(random, new[]
        {
            "Vor allem der Groove haelt den Song zusammen.",
            "Die Uebergaenge zwischen den Teilen sind ueberzeugend gesetzt.",
            "Kleine Details im Arrangement machen den Refrain staerker.",
            "Der Song wirkt bewusst gestaltet statt zufaellig gebaut.",
            "Gerade die ruhigeren Momente geben ihm Profil.",
            "Die Produktion bleibt praesent, draengt sich aber nicht auf."
        });

        var closing = likes switch
        {
            >= 8 => Pick(random, new[]
            {
                "Das gehoert zu den staerkeren Nummern hier.",
                "Man hoert ihn gern ein zweites Mal.",
                "Die positive Resonanz ist nachvollziehbar."
            }),
            >= 5 => Pick(random, new[]
            {
                "Nicht perfekt, aber klar erkennbar in seiner Handschrift.",
                "Kein riesiger Wurf, doch insgesamt stimmig.",
                "Genug eigene Farbe, um interessant zu bleiben."
            }),
            _ => Pick(random, new[]
            {
                "Ganz zuendet er nicht, handwerklich ist aber einiges da.",
                "Mit etwas mehr Mut waere er deutlich einpraegsamer.",
                "Die Basis stimmt, nur der letzte Funke fehlt."
            })
        };

        return $"{opener} {focus}. {detail} {closing}";
    }

    private static string BuildUkrainianReview(Random random, string title, string artist, string album, string genre, int likes)
    {
        var opener = Pick(random, new[]
        {
            $"\"{title}\"",
            $"{artist} \u0443 \u0436\u0430\u043d\u0440\u0456 {genre}",
            $"\u0426\u0435\u0439 \u0442\u0440\u0435\u043a \u0437 {album}",
            $"\u0423 \"{title}\" {artist}"
        });

        var focus = Pick(random, new[]
        {
            "\u0442\u0440\u0438\u043c\u0430\u0454\u0442\u044c\u0441\u044f \u043d\u0430 \u0441\u0438\u043b\u044c\u043d\u043e\u043c\u0443 \u043c\u0435\u043b\u043e\u0434\u0456\u0439\u043d\u043e\u043c\u0443 \u0433\u0430\u043a\u0443",
            "\u043f\u0435\u0440\u0435\u043a\u043e\u043d\u0443\u0454 \u0430\u0442\u043c\u043e\u0441\u0444\u0435\u0440\u043e\u044e \u0442\u0430 \u0432\u043f\u0435\u0432\u043d\u0435\u043d\u043e\u044e \u043f\u0440\u043e\u0434\u0430\u043a\u0448\u043d-\u0440\u043e\u0431\u043e\u0442\u043e\u044e",
            "\u0437\u0432\u0443\u0447\u0438\u0442\u044c \u0437\u0456\u0431\u0440\u0430\u043d\u043e \u0439 \u0431\u0435\u0437 \u0437\u0430\u0439\u0432\u043e\u0433\u043e \u0448\u0443\u043c\u0443",
            "\u043c\u0430\u0454 \u0434\u043e\u0441\u0442\u0430\u0442\u043d\u044c\u043e \u0434\u0435\u0442\u0430\u043b\u0435\u0439, \u0449\u043e\u0431 \u043d\u0435 \u0437\u0430\u0433\u0443\u0431\u0438\u0442\u0438\u0441\u044f",
            "\u0431\u0435\u0440\u0435 \u043d\u0430\u0441\u0442\u0440\u043e\u0454\u043c \u0431\u0456\u043b\u044c\u0448\u0435, \u043d\u0456\u0436 \u0433\u0443\u0447\u043d\u0456\u0441\u0442\u044e",
            "\u043d\u0430\u0440\u043e\u0449\u0443\u0454 \u0435\u043d\u0435\u0440\u0433\u0456\u044e \u0431\u0435\u0437 \u0432\u0456\u0434\u0447\u0443\u0442\u0442\u044f \u043f\u0435\u0440\u0435\u0432\u0430\u043d\u0442\u0430\u0436\u0435\u043d\u043d\u044f"
        });

        var detail = Pick(random, new[]
        {
            "\u0420\u0438\u0442\u043c\u0456\u0447\u043d\u0430 \u043e\u0441\u043d\u043e\u0432\u0430 \u0434\u043e\u0431\u0440\u0435 \u0432\u0435\u0434\u0435 \u043a\u043e\u043c\u043f\u043e\u0437\u0438\u0446\u0456\u044e \u0432\u043f\u0435\u0440\u0435\u0434.",
            "\u041f\u0435\u0440\u0435\u0445\u043e\u0434\u0438 \u043c\u0456\u0436 \u0447\u0430\u0441\u0442\u0438\u043d\u0430\u043c\u0438 \u0437\u0440\u043e\u0431\u043b\u0435\u043d\u0456 \u0434\u043e\u0441\u0438\u0442\u044c \u0442\u043e\u0447\u043d\u043e.",
            "\u041d\u0435\u0432\u0435\u043b\u0438\u043a\u0456 \u0441\u0442\u0443\u0434\u0456\u0439\u043d\u0456 \u0434\u0435\u0442\u0430\u043b\u0456 \u0440\u043e\u0431\u043b\u044f\u0442\u044c \u043f\u0440\u0438\u0441\u043f\u0456\u0432 \u0441\u0438\u043b\u044c\u043d\u0456\u0448\u0438\u043c.",
            "\u0422\u0440\u0435\u043a \u0437\u0432\u0443\u0447\u0438\u0442\u044c \u043f\u0440\u043e\u0434\u0443\u043c\u0430\u043d\u043e, \u0430 \u043d\u0435 \u0432\u0438\u043f\u0430\u0434\u043a\u043e\u0432\u043e \u0437\u0456\u0431\u0440\u0430\u043d\u043e.",
            "\u041d\u0430\u0439\u043a\u0440\u0430\u0449\u0435 \u043f\u0440\u0430\u0446\u044e\u044e\u0442\u044c \u0441\u0430\u043c\u0435 \u0441\u0442\u0440\u0438\u043c\u0430\u043d\u0456 \u043c\u043e\u043c\u0435\u043d\u0442\u0438.",
            "\u0423 \u043d\u044c\u043e\u043c\u0443 \u0454 \u0444\u043e\u0440\u043c\u0430, \u044f\u043a\u0443 \u043b\u0435\u0433\u043a\u043e \u0437\u0430\u043f\u0430\u043c'\u044f\u0442\u0430\u0442\u0438."
        });

        var closing = likes switch
        {
            >= 8 => Pick(random, new[]
            {
                "\u041e\u0434\u0438\u043d \u0456\u0437 \u0441\u0438\u043b\u044c\u043d\u0456\u0448\u0438\u0445 \u043d\u043e\u043c\u0435\u0440\u0456\u0432 \u0443 \u0434\u043e\u0431\u0456\u0440\u0446\u0456.",
                "\u0414\u043e \u043d\u044c\u043e\u0433\u043e \u043b\u0435\u0433\u043a\u043e \u0445\u043e\u0447\u0435\u0442\u044c\u0441\u044f \u043f\u043e\u0432\u0435\u0440\u043d\u0443\u0442\u0438\u0441\u044f \u0449\u0435 \u0440\u0430\u0437.",
                "\u041f\u043e\u0437\u0438\u0442\u0438\u0432\u043d\u0430 \u0440\u0435\u0430\u043a\u0446\u0456\u044f \u0442\u0443\u0442 \u0446\u0456\u043b\u043a\u043e\u043c \u0437\u0430\u0441\u043b\u0443\u0436\u0435\u043d\u0430."
            }),
            >= 5 => Pick(random, new[]
            {
                "\u041d\u0435 \u0431\u0435\u0437 \u0432\u0430\u0434, \u0437\u0430\u0442\u0435 \u0437 \u0432\u043b\u0430\u0441\u043d\u0438\u043c \u0445\u0430\u0440\u0430\u043a\u0442\u0435\u0440\u043e\u043c.",
                "\u041d\u0435 \u0433\u0456\u0433\u0430\u043d\u0442\u0441\u044c\u043a\u0438\u0439 \u0445\u0456\u0442, \u0430\u043b\u0435 \u0446\u0456\u043b\u0456\u0441\u043d\u0430 \u0440\u043e\u0431\u043e\u0442\u0430.",
                "\u0406\u043d\u0434\u0438\u0432\u0456\u0434\u0443\u0430\u043b\u044c\u043d\u043e\u0441\u0442\u0456 \u0432\u0438\u0441\u0442\u0430\u0447\u0430\u0454, \u0449\u043e\u0431 \u0442\u0440\u0438\u043c\u0430\u0442\u0438 \u0443\u0432\u0430\u0433\u0443."
            }),
            _ => Pick(random, new[]
            {
                "\u0414\u043e \u043a\u0456\u043d\u0446\u044f \u043d\u0435 \u0447\u0456\u043f\u043b\u044f\u0454, \u0445\u043e\u0447\u0430 \u0431\u0430\u0437\u0430 \u0442\u0443\u0442 \u043d\u0435\u043f\u043e\u0433\u0430\u043d\u0430.",
                "\u0422\u0440\u043e\u0445\u0438 \u0431\u0456\u043b\u044c\u0448\u0435 \u0441\u043c\u0456\u043b\u0438\u0432\u043e\u0441\u0442\u0456 \u0437\u0440\u043e\u0431\u0438\u043b\u043e \u0431 \u0439\u043e\u0433\u043e \u043f\u043e\u043c\u0456\u0442\u043d\u0456\u0448\u0438\u043c.",
                "\u0420\u0435\u043c\u0435\u0441\u043b\u043e \u0432\u0456\u0434\u0447\u0443\u0432\u0430\u0454\u0442\u044c\u0441\u044f, \u0430\u043b\u0435 \u0431\u0440\u0430\u043a\u0443\u0454 \u043e\u0441\u0442\u0430\u043d\u043d\u044c\u043e\u0433\u043e \u0430\u043a\u0446\u0435\u043d\u0442\u0443."
            })
        };

        return $"{opener} {focus}. {detail} {closing}";
    }

    private static string Pick(Random random, string[] options) => options[random.Next(options.Length)];

    private static string MapBogusLocale(string locale)
    {
        return LocaleMap.TryGetValue(locale, out var mapped) ? mapped : "en";
    }

    private static string SingleLabel(string locale) => locale switch
    {
        "de-DE" => "Single",
        "uk-UA" => "\u0421\u0438\u043d\u0433\u043b",
        _ => "Single"
    };

    private static string[] MotsTitre(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Neon", "Wilde", "Leise", "Goldene", "Spaete", faker.Address.CityPrefix() },
        "uk-UA" => new[]
        {
            "\u041d\u0456\u0447\u043d\u0456",
            "\u0421\u043e\u043d\u044f\u0447\u043d\u0456",
            "\u0422\u0438\u0445\u0456",
            "\u0417\u043e\u043b\u043e\u0442\u0456",
            "\u0414\u0438\u043a\u0456",
            faker.Address.CityPrefix()
        },
        _ => new[] { "Neon", "Velvet", "Midnight", "Silver", "Electric", faker.Address.CityPrefix() }
    };

    private static string[] NomsTitre(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Wellen", "Rhythmen", "Traeume", "Klaenge", "Echo", faker.Commerce.ProductAdjective() },
        "uk-UA" => new[]
        {
            "\u0425\u0432\u0438\u043b\u0456",
            "\u0420\u0438\u0442\u043c\u0438",
            "\u041c\u0440\u0456\u0457",
            "\u0417\u0432\u0443\u043a\u0438",
            "\u0415\u0445\u043e",
            faker.Commerce.ProductAdjective()
        },
        _ => new[] { "Waves", "Rhythm", "Dreams", "Echo", "Groove", faker.Commerce.ProductAdjective() }
    };

    private static string[] GroupDescriptors(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Die", "Neon", "Silber", faker.Commerce.Color(), "Elektrische" },
        "uk-UA" => new[]
        {
            "\u041d\u0456\u0447\u043d\u0456",
            "\u0421\u043e\u043d\u044f\u0447\u043d\u0456",
            "\u041c\u0456\u0441\u044c\u043a\u0456",
            faker.Commerce.Color(),
            "\u0415\u043b\u0435\u043a\u0442\u0440\u0438\u0447\u043d\u0456"
        },
        _ => new[] { "The", "Neon", "Cosmic", faker.Commerce.Color(), "Electric" }
    };

    private static string[] GroupEndings(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Kollektiv", "Wellen", "Klaenge", "Traeumer", faker.Commerce.Department() },
        "uk-UA" => new[]
        {
            "\u0425\u0432\u0438\u043b\u0456",
            "\u041c\u0440\u0456\u0439\u043d\u0438\u043a\u0438",
            "\u0417\u0432\u0443\u043a\u0438",
            "\u0421\u0432\u0456\u0442\u0430\u043d\u043e\u043a",
            faker.Commerce.Department()
        },
        _ => new[] { "Collective", "Waves", "Echoes", "Dreamers", faker.Commerce.Department() }
    };

    private static string[] AlbumPlaces(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Naechte", "Boulevard", "Studio", "Horizont", faker.Address.City() },
        "uk-UA" => new[]
        {
            "\u041d\u043e\u0447\u0456",
            "\u0411\u0443\u043b\u044c\u0432\u0430\u0440",
            "\u0421\u0442\u0443\u0434\u0456\u044f",
            "\u041e\u0431\u0440\u0456\u0439",
            faker.Address.City()
        },
        _ => new[] { "Nights", "Boulevard", "Studio", "Horizon", faker.Address.City() }
    };

    private static string[] AlbumMoods(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Digitale", "Leise", "Goldene", "Spaete", faker.Commerce.ProductMaterial() },
        "uk-UA" => new[]
        {
            "\u0426\u0438\u0444\u0440\u043e\u0432\u0456",
            "\u0422\u0438\u0445\u0456",
            "\u0417\u043e\u043b\u043e\u0442\u0456",
            "\u041f\u0456\u0437\u043d\u0456",
            faker.Commerce.ProductMaterial()
        },
        _ => new[] { "Digital", "Silent", "Golden", "Late", faker.Commerce.ProductMaterial() }
    };
}
