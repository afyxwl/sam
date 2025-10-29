using MongoDB.Driver;
using SAMsa.Models;

namespace SAMsa.Repositories
{
    public class MongoPostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _posts;

        public MongoPostRepository(IMongoDatabase db)
        {
            _posts = db.GetCollection<Post>("posts");
        }

        public async Task<Post> CreateAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
            return post;
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
            await _posts.DeleteOneAsync(filter);
        }

        public async Task<List<Post>> GetAllAsync()
        {
            var list = await _posts.Find(_ => true).SortByDescending(p => p.CreatedAt).ToListAsync();
            return list;
        }

        public async Task<Post?> GetAsync(Guid id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
            return await _posts.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, post.Id);
            await _posts.ReplaceOneAsync(filter, post, new ReplaceOptions { IsUpsert = false });
        }
    }
}
