using Microsoft.AspNetCore.Mvc;
using SAMsa.Services;

namespace SAMsa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OmdbController : ControllerBase
    {
        private readonly OmdbService _omdb;

        public OmdbController(OmdbService omdb)
        {
            _omdb = omdb;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("q is required");
            var res = await _omdb.SearchAsync(q, page, HttpContext.RequestAborted);
            if (res == null) return BadRequest("OMDB API key not configured");
            return Ok(res);
        }

        [HttpGet("details")]
        public async Task<IActionResult> Details([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("id is required (imdb id or title)");
            var res = await _omdb.GetDetailsAsync(id, HttpContext.RequestAborted);
            if (res == null) return BadRequest("OMDB API key not configured");
            return Ok(res);
        }
    }
}
