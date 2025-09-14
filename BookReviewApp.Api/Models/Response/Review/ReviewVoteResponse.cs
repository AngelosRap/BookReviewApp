using Swashbuckle.AspNetCore.Annotations;

namespace BookReviewApp.Api.Models.Response.Review;

[SwaggerSchema("Response model for a vote on a review", Title = "ReviewVoteResponse")]
public class ReviewVoteResponse
{
    [SwaggerSchema("Vote identifier")]
    public int Id { get; set; }

    [SwaggerSchema("Id of the review this vote belongs to")]
    public int ReviewId { get; set; }

    [SwaggerSchema("Id of the user who made the vote")]
    public string? UserId { get; set; }

    [SwaggerSchema("Indicates if the vote is an upvote (true) or downvote (false)")]
    public bool IsUpvote { get; set; }
}
