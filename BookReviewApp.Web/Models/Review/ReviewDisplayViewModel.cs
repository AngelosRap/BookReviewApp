using BookReviewApp.Domain.Models;

namespace BookReviewApp.Web.Models.Review;

public class ReviewDisplayViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ICollection<ReviewVote> Votes { get; set; } = [];
}