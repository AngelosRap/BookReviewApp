namespace BookReviewApp.Domain.Models;

public class ReviewVote
{
    public int Id { get; set; }

    public int ReviewId { get; set; }
    public Review Review { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public bool IsUpvote { get; set; }
}
