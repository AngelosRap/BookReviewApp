using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Api.Models.Request;

[SwaggerSchema("Vote request for a review", Title = "ReviewVote")]
public class ReviewVoteRequest
{
    [SwaggerSchema("Indicates whether the vote is an upvote (true) or downvote (false)", Nullable = false)]
    [Required(ErrorMessage = "Vote type is required.")]
    public bool IsUpvote { get; set; }
}
