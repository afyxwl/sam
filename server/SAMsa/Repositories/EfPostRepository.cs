using Microsoft.EntityFrameworkCore;
using SAMsa.Data;
using SAMsa.Models;

namespace SAMsa.Repositories
{
    public class EfPostRepository : IPostRepository
    {
        private readonly AppDbContext _db;
        public EfPostRepository(AppDbContext db) => _db = db;

        public async Task<Post> CreateAsync(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        public async Task DeleteAsync(Guid id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return;
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Post>> GetAllAsync()
        {
            return await _db.Posts.Include(p => p.Replies).OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<Post?> GetAsync(Guid id)
        {
            return await _db.Posts.Include(p => p.Replies).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }
    }
}
