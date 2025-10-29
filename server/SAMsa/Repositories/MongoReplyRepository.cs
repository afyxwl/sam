using MongoDB.Driver;
using SAMsa.Models;

namespace SAMsa.Repositories
{
    public class MongoReplyRepository : IReplyRepository
    {
        private readonly IMongoCollection<Reply> _replies;

        public MongoReplyRepository(IMongoDatabase db)
        {
            _replies = db.GetCollection<Reply>("replies");
        }

        public async Task<Reply> CreateAsync(Reply reply)
        {
            await _replies.InsertOneAsync(reply);
            return reply;
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<Reply>.Filter.Eq(r => r.Id, id);
            await _replies.DeleteOneAsync(filter);
        }

        public async Task<List<Reply>> GetForPostAsync(Guid postId)
        {
            var filter = Builders<Reply>.Filter.Eq(r => r.PostId, postId);
            return await _replies.Find(filter).SortBy(r => r.CreatedAt).ToListAsync();
        }
    }
}
