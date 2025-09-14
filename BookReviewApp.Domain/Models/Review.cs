namespace BookReviewApp.Domain.Models;

public class Review
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public int BookId { get; set; }
    public Book? Book { get; set; }

    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    public ICollection<ReviewVote> Votes { get; set; } = [];
}
