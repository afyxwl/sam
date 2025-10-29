using Microsoft.AspNetCore.Mvc;
using SAMsa.Services;

namespace SAMsa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TmdbController : ControllerBase
    {
        private readonly TmdbService _tmdb;

        public TmdbController(TmdbService tmdb)
        {
            _tmdb = tmdb;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("q is required");
            var res = await _tmdb.SearchAsync(q, page, HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpGet("{mediaType}/{id}")]
        public async Task<IActionResult> Details(string mediaType, int id)
        {
            var res = await _tmdb.GetDetailsAsync(mediaType, id, HttpContext.RequestAborted);
            return Ok(res);
        }
    }
}
