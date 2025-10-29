using Microsoft.EntityFrameworkCore;
using SAMsa.Data;
using SAMsa.Models;

namespace SAMsa.Repositories
{
    public class EfReplyRepository : IReplyRepository
    {
        private readonly AppDbContext _db;
        public EfReplyRepository(AppDbContext db) => _db = db;

        public async Task<Reply> CreateAsync(Reply reply)
        {
            _db.Replies.Add(reply);
            await _db.SaveChangesAsync();
            return reply;
        }

        public async Task DeleteAsync(Guid id)
        {
            var r = await _db.Replies.FindAsync(id);
            if (r == null) return;
            _db.Replies.Remove(r);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Reply>> GetForPostAsync(Guid postId)
        {
            return await _db.Replies.Where(r => r.PostId == postId).OrderBy(r => r.CreatedAt).ToListAsync();
        }
    }
}
