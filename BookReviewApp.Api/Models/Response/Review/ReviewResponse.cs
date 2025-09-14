using Swashbuckle.AspNetCore.Annotations;

namespace BookReviewApp.Api.Models.Response.Review;

[SwaggerSchema("Response model for a review", Title = "ReviewResponse")]
public class ReviewResponse
{
    [SwaggerSchema("Review identifier")]
    public int Id { get; set; }

    [SwaggerSchema("Content of the review")]
    public string Content { get; set; } = string.Empty;

    [SwaggerSchema("Rating given in the review")]
    public int Rating { get; set; }

    [SwaggerSchema("Date the review was created")]
    public DateTime DateCreated { get; set; }

    [SwaggerSchema("Id of the book associated with this review")]
    public int BookId { get; set; }

    [SwaggerSchema("Id of the user who created the review")]
    public string? UserId { get; set; }

    [SwaggerSchema("Votes associated with this review")]
    public ICollection<ReviewVoteResponse> Votes { get; set; } = new List<ReviewVoteResponse>();
}
