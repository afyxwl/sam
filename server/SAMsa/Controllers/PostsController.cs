using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAMsa.Data;
using SAMsa.Models;

namespace SAMsa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly SAMsa.Repositories.IPostRepository _repo;

        public PostsController(SAMsa.Repositories.IPostRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _repo.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var post = await _repo.GetAsync(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post post)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            post.Id = Guid.NewGuid();
            post.CreatedAt = DateTime.UtcNow;
            await _repo.CreateAsync(post);
            return CreatedAtAction(nameof(Get), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Post updated)
        {
            var post = await _repo.GetAsync(id);
            if (post == null) return NotFound();

            post.Title = updated.Title;
            post.Content = updated.Content;
            post.Rating = updated.Rating;
            post.TmdbId = updated.TmdbId;

            await _repo.UpdateAsync(post);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var post = await _repo.GetAsync(id);
            if (post == null) return NotFound();
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
