namespace MusicStoreAPI.Utils;

public static class SeedHelper
{
    public static int CombineSeed(long userSeed, int page, int index = 0)
    {
        // Mix user seed, page, and index to keep generation reproducible.
        long combined = userSeed ^ (page * 1000003L) ^ (index * 1000033L);
        return (int)(Math.Abs(combined) % int.MaxValue);
    }

    public static int CombineSeed(long userSeed, int page)
    {
        return CombineSeed(userSeed, page, 0);
    }
}
