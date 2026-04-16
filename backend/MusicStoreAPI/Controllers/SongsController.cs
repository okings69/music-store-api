using Microsoft.AspNetCore.Mvc;
using MusicStoreAPI.Services;
using MusicStoreAPI.DTOs;

namespace MusicStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly SongService _songService;

    public SongsController(SongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    public ActionResult<List<SongDto>> GetSongs(
        [FromQuery] long seed = 12345,
        [FromQuery] int page = 1,
        [FromQuery] double avgLikes = 5.0,
        [FromQuery] string locale = "en-US")
    {
        if (page < 1)
        {
            return BadRequest(new { error = "Le paramètre 'page' doit être supérieur ou égal à 1." });
        }

        if (avgLikes < 0 || avgLikes > 10)
        {
            return BadRequest(new { error = "Le paramètre 'avgLikes' doit être compris entre 0 et 10." });
        }

        try
        {
            var songs = _songService.GenerateSongs(seed, page, avgLikes, locale);
            return Ok(songs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erreur lors de la génération des chansons", details = ex.Message });
        }
    }
}
