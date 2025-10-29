using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAMsa.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty; // Movie/Show title or shorthand

        [Required]
        public string Content { get; set; } = string.Empty; // Review content

        public int Rating { get; set; } // Optional rating 0-10

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // External movie/show id if linked (e.g. OMDB imdbID)
    public string? ExternalId { get; set; }

        // Navigation
        public List<Reply> Replies { get; set; } = new List<Reply>();
    }
}
