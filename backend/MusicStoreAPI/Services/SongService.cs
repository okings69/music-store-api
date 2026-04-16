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

            var song = new SongDto
            {
                Id = currentIndex,
                Index = currentIndex,
                Title = title,
                Artist = artist,
                Album = album,
                Genre = genre,
                Likes = GenererLikes(songRandom, avgLikes),
                CoverUrl = GenererCoverUrl(title, artist, songSeed),
                AudioUrl = GenererAudioUrl(songSeed, genre),
                Review = GenererAvis(songRandom, locale)
            };

            songs.Add(song);
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
            "uk-UA" => new[] { "Рок", "Поп", "Електроніка", "Джаз", "Класика", "Метал", "Інді" },
            _ => new[] { "Rock", "Pop", "Electronic", "Jazz", "Classical", "Hip Hop", "Metal", "Indie" }
        };

        return faker.PickRandom(genres);
    }

    private int GenererLikes(Random random, double avgLikes)
    {
        if (avgLikes <= 0) return 0;
        if (avgLikes >= 10) return 10;
        
        int floor = (int)Math.Floor(avgLikes);
        double prob = avgLikes - floor;
        
        return floor + (random.NextDouble() < prob ? 1 : 0);
    }

    private string GenererCoverUrl(string title, string artist, int seed)
    {
        var encodedTitle = Uri.EscapeDataString(title);
        var encodedArtist = Uri.EscapeDataString(artist);
        return $"{GetApiBaseUrl()}/cover?title={encodedTitle}&artist={encodedArtist}&seed={Math.Abs(seed)}";
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

    private string GenererAvis(Random random, string locale)
    {
        if (locale == "de-DE")
        {
            var avis = new[]
            { 
                "Absolut brillant! Ein Meisterwerk.",
                "Interessant, aber etwas repetitiv.",
                "Frischer Sound! Sehr empfehlenswert!",
                "Nicht mein Fall, aber technisch gut.",
                "Eine schöne Reise durch Klang und Emotion."
            };
            return avis[random.Next(avis.Length)];
        }
        else if (locale == "uk-UA")
        {
            var avis = new[] 
            { 
                "Абсолютно блискуче! Шедевр.",
                "Цікаво, але трохи повторювано.",
                "Свіжий звук! Дуже рекомендую!",
                "Не мій жанр, але технічно добре.",
                "Чудова подорож звуком та емоціями."
            };
            return avis[random.Next(avis.Length)];
        }
        
        var avisEn = new[] 
        { 
            "Absolutely brilliant! A masterpiece.",
            "Interesting, but somewhat repetitive.",
            "Fresh sound! Highly recommended!",
            "Not my cup of tea, but technically good.",
            "A beautiful journey through sound and emotion."
        };
        return avisEn[random.Next(avisEn.Length)];
    }

    private static string MapBogusLocale(string locale)
    {
        return LocaleMap.TryGetValue(locale, out var mapped) ? mapped : "en";
    }

    private static string SingleLabel(string locale) => locale switch
    {
        "de-DE" => "Single",
        "uk-UA" => "Сингл",
        _ => "Single"
    };

    private static string[] MotsTitre(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Neon", "Wilde", "Leise", "Goldene", "Späte", faker.Address.CityPrefix() },
        "uk-UA" => new[] { "Нічні", "Сонячні", "Тихі", "Золоті", "Дикі", faker.Address.CityPrefix() },
        _ => new[] { "Neon", "Velvet", "Midnight", "Silver", "Electric", faker.Address.CityPrefix() }
    };

    private static string[] NomsTitre(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Wellen", "Rhythmen", "Träume", "Klänge", "Echo", faker.Commerce.ProductAdjective() },
        "uk-UA" => new[] { "Хвилі", "Ритми", "Мрії", "Звуки", "Ехо", faker.Commerce.ProductAdjective() },
        _ => new[] { "Waves", "Rhythm", "Dreams", "Echo", "Groove", faker.Commerce.ProductAdjective() }
    };

    private static string[] GroupDescriptors(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Die", "Neon", "Silber", faker.Commerce.Color(), "Elektrische" },
        "uk-UA" => new[] { "Нічні", "Сонячні", "Міські", faker.Commerce.Color(), "Електричні" },
        _ => new[] { "The", "Neon", "Cosmic", faker.Commerce.Color(), "Electric" }
    };

    private static string[] GroupEndings(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Kollektiv", "Wellen", "Klänge", "Träumer", faker.Commerce.Department() },
        "uk-UA" => new[] { "Хвилі", "Мрійники", "Звуки", "Світанок", faker.Commerce.Department() },
        _ => new[] { "Collective", "Waves", "Echoes", "Dreamers", faker.Commerce.Department() }
    };

    private static string[] AlbumPlaces(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Nächte", "Boulevard", "Studio", "Horizont", faker.Address.City() },
        "uk-UA" => new[] { "Ночі", "Бульвар", "Студія", "Обрій", faker.Address.City() },
        _ => new[] { "Nights", "Boulevard", "Studio", "Horizon", faker.Address.City() }
    };

    private static string[] AlbumMoods(string locale, Faker faker) => locale switch
    {
        "de-DE" => new[] { "Digitale", "Leise", "Goldene", "Späte", faker.Commerce.ProductMaterial() },
        "uk-UA" => new[] { "Цифрові", "Тихі", "Золоті", "Пізні", faker.Commerce.ProductMaterial() },
        _ => new[] { "Digital", "Silent", "Golden", "Late", faker.Commerce.ProductMaterial() }
    };
}
