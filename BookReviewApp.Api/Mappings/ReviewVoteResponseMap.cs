using BookReviewApp.Api.Models.Response.Review;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Api.Mappings;

public static class ReviewVoteResponseMap
{
    public static ReviewVoteResponse ToResponse(this ReviewVote reviewVote)
    {
        return new()
        {
            Id = reviewVote.Id,
            ReviewId = reviewVote.ReviewId,
            UserId = reviewVote.UserId,
            IsUpvote = reviewVote.IsUpvote,
        };
    }
}
