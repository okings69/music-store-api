using Microsoft.AspNetCore.Mvc;
using MusicStoreAPI.Utils;

[ApiController]
[Route("api/audio")]
public class AudioController : ControllerBase
{
    [HttpGet]
    public IActionResult Generate([FromQuery] int seed = 1234, [FromQuery] string genre = "Pop")
    {
        var safeSeed = Math.Abs(seed);
        var wav = AudioPreviewGenerator.GenerateWav(safeSeed, genre);
        return File(wav, "audio/wav", enableRangeProcessing: true);
    }
}
