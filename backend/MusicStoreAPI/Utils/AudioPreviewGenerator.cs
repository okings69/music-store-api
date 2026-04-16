namespace MusicStoreAPI.Utils;

public static class AudioPreviewGenerator
{
    private const int SampleRate = 22050;
    private const short BitsPerSample = 16;
    private const short Channels = 1;
    private const int ClipSeconds = 6;

    public static byte[] GenerateWav(int seed, string genre)
    {
        var random = new Random(seed);
        var profile = AudioProfile.ForGenre(genre);
        var totalSamples = SampleRate * ClipSeconds;
        var samples = new short[totalSamples];
        var scale = PickScale(random, profile);
        var progression = profile.Progression;
        var samplesPerBeat = Math.Max(1, (int)(SampleRate * 60.0 / profile.Bpm));

        for (int start = 0; start < totalSamples; start += samplesPerBeat)
        {
            var beatIndex = start / samplesPerBeat;
            var note = scale[(beatIndex * profile.MelodyStep + random.Next(scale.Length)) % scale.Length];
            var root = progression[beatIndex % progression.Length];
            var octaveLift = beatIndex % 8 >= 4 ? 12 : 0;
            var leadFrequency = MidiToFrequency(profile.LeadBaseMidi + root + note + octaveLift);
            var bassFrequency = MidiToFrequency(profile.BassBaseMidi + root);
            var padFrequency = MidiToFrequency(profile.PadBaseMidi + root + scale[(beatIndex + 2) % scale.Length]);
            var beatLength = Math.Min(samplesPerBeat, totalSamples - start);

            for (int i = 0; i < beatLength; i++)
            {
                var sampleIndex = start + i;
                var time = sampleIndex / (double)SampleRate;
                var noteProgress = i / (double)beatLength;
                var envelope = Math.Exp(-profile.Decay * noteProgress) * (0.8 + 0.2 * Math.Sin(Math.PI * noteProgress));
                var lead = Wave(profile.LeadWave, leadFrequency, time);
                var harmony = 0.4 * Wave(profile.HarmonyWave, leadFrequency * profile.HarmonyRatio, time);
                var bass = 0.3 * Wave("sine", bassFrequency, time);
                var pad = 0.2 * Wave("triangle", padFrequency, time);
                var kick = beatIndex % profile.KickEvery == 0
                    ? 0.22 * Math.Exp(-12 * noteProgress) * Math.Sin(2 * Math.PI * (70 - 30 * noteProgress) * time)
                    : 0;
                var hat = beatIndex % profile.HatEvery == 0
                    ? 0.06 * (random.NextDouble() * 2 - 1) * Math.Exp(-24 * noteProgress)
                    : 0;
                var mixed = (lead * profile.LeadGain + harmony + bass + pad + kick + hat) * envelope;
                samples[sampleIndex] = (short)Math.Clamp(mixed * short.MaxValue * 0.75, short.MinValue, short.MaxValue);
            }
        }

        return BuildWav(samples);
    }

    private static int[] PickScale(Random random, AudioProfile profile)
    {
        return profile.Scales[random.Next(profile.Scales.Length)];
    }

    private static double Wave(string wave, double frequency, double time)
    {
        var phase = 2 * Math.PI * frequency * time;
        return wave switch
        {
            "square" => Math.Sign(Math.Sin(phase)),
            "saw" => 2 * (frequency * time - Math.Floor(0.5 + frequency * time)),
            "triangle" => 2 * Math.Asin(Math.Sin(phase)) / Math.PI,
            _ => Math.Sin(phase)
        };
    }

    private static double MidiToFrequency(int midiNote)
    {
        return 440.0 * Math.Pow(2, (midiNote - 69) / 12.0);
    }

    private static byte[] BuildWav(short[] samples)
    {
        var dataSize = samples.Length * sizeof(short);
        using var stream = new MemoryStream(44 + dataSize);
        using var writer = new BinaryWriter(stream);

        writer.Write("RIFF"u8.ToArray());
        writer.Write(36 + dataSize);
        writer.Write("WAVE"u8.ToArray());
        writer.Write("fmt "u8.ToArray());
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(Channels);
        writer.Write(SampleRate);
        writer.Write(SampleRate * Channels * BitsPerSample / 8);
        writer.Write((short)(Channels * BitsPerSample / 8));
        writer.Write(BitsPerSample);
        writer.Write("data"u8.ToArray());
        writer.Write(dataSize);

        foreach (var sample in samples)
        {
            writer.Write(sample);
        }

        writer.Flush();
        return stream.ToArray();
    }

    private sealed record AudioProfile(
        int Bpm,
        int LeadBaseMidi,
        int BassBaseMidi,
        int PadBaseMidi,
        int MelodyStep,
        int KickEvery,
        int HatEvery,
        double Decay,
        double HarmonyRatio,
        double LeadGain,
        string LeadWave,
        string HarmonyWave,
        int[] Progression,
        int[][] Scales)
    {
        public static AudioProfile ForGenre(string genre)
        {
            return genre.ToLowerInvariant() switch
            {
                "rock" => new(132, 64, 40, 52, 2, 2, 1, 2.2, 0.5, 0.56, "saw", "square",
                    [0, 5, 7, 3], [[0, 2, 4, 5, 7, 9, 11], [0, 2, 4, 7, 9]]),
                "electronic" => new(124, 67, 36, 48, 3, 1, 1, 1.6, 2.0, 0.5, "square", "saw",
                    [0, 3, 5, 7], [[0, 2, 3, 5, 7, 8, 10], [0, 2, 5, 7, 9]]),
                "jazz" => new(108, 65, 38, 50, 1, 4, 2, 2.8, 1.5, 0.44, "triangle", "sine",
                    [0, 5, 2, 7], [[0, 2, 4, 6, 7, 9, 11], [0, 2, 3, 5, 7, 9, 10]]),
                "hip hop" => new(92, 60, 34, 46, 1, 2, 1, 1.9, 0.5, 0.52, "square", "triangle",
                    [0, 0, 5, 3], [[0, 3, 5, 7, 10], [0, 2, 3, 5, 7, 8, 10]]),
                "metal" => new(156, 62, 34, 46, 2, 1, 1, 1.3, 0.5, 0.62, "saw", "saw",
                    [0, 1, 5, 3], [[0, 1, 3, 5, 7, 8, 10], [0, 3, 5, 6, 7, 10]]),
                "classical" => new(100, 72, 48, 60, 1, 4, 4, 3.1, 2.0, 0.42, "sine", "triangle",
                    [0, 4, 5, 3], [[0, 2, 4, 5, 7, 9, 11], [0, 2, 3, 5, 7, 8, 10]]),
                "indie" => new(118, 67, 40, 52, 2, 2, 2, 2.4, 1.0, 0.46, "triangle", "sine",
                    [0, 7, 5, 3], [[0, 2, 4, 7, 9], [0, 2, 3, 5, 7, 10]]),
                _ => new(116, 65, 38, 50, 2, 2, 2, 2.1, 1.0, 0.48, "sine", "triangle",
                    [0, 5, 3, 4], [[0, 2, 4, 5, 7, 9, 11], [0, 2, 3, 5, 7, 8, 10]])
            };
        }
    }
}
