using BookReviewApp.Api.Models.Response.Review;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace BookReviewApp.Api.Models.Response.Book;

[SwaggerSchema("Detailed response model for a single book with reviews", Title = "BookDetailResponse")]
public class BookDetailResponse : BookListResponse
{
    [SwaggerSchema("List of reviews for the book")]
    [JsonPropertyOrder(6)] // Ensures Reviews appear last in JSON
    public ICollection<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>();
}
