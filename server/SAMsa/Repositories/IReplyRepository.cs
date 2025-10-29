using SAMsa.Models;

namespace SAMsa.Repositories
{
    public interface IReplyRepository
    {
        Task<List<Reply>> GetForPostAsync(Guid postId);
        Task<Reply> CreateAsync(Reply reply);
        Task DeleteAsync(Guid id);
    }
}
