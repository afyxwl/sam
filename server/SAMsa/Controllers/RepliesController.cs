using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAMsa.Data;
using SAMsa.Models;

namespace SAMsa.Controllers
{
    [ApiController]
    [Route("api/posts/{postId:guid}/[controller]")]
    public class RepliesController : ControllerBase
    {
        private readonly SAMsa.Repositories.IReplyRepository _repo;
        private readonly SAMsa.Repositories.IPostRepository _postRepo;

        public RepliesController(SAMsa.Repositories.IReplyRepository repo, SAMsa.Repositories.IPostRepository postRepo)
        {
            _repo = repo;
            _postRepo = postRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid postId)
        {
            var post = await _postRepo.GetAsync(postId);
            if (post == null) return NotFound();
            var replies = await _repo.GetForPostAsync(postId);
            return Ok(replies);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid postId, [FromBody] Reply reply)
        {
            var post = await _postRepo.GetAsync(postId);
            if (post == null) return NotFound();

            reply.Id = Guid.NewGuid();
            reply.PostId = postId;
            reply.CreatedAt = DateTime.UtcNow;

            await _repo.CreateAsync(reply);

            return CreatedAtAction(null, new { postId = postId, id = reply.Id }, reply);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid postId, Guid id)
        {
            var replies = await _repo.GetForPostAsync(postId);
            var reply = replies.FirstOrDefault(r => r.Id == id);
            if (reply == null) return NotFound();
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
