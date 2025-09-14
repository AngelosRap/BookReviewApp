using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Api.Models.Request;

[SwaggerSchema("Request model for creating a new review", Title = "ReviewCreate")]
public class ReviewCreateRequest
{
    [SwaggerSchema("Content of the review (max 1000 characters)", Nullable = false)]
    [Required(ErrorMessage = "Review content is required.")]
    [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
    public string Content { get; set; } = string.Empty;

    [SwaggerSchema("Rating of the book (1-5)", Nullable = false)]
    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [SwaggerSchema("Id of the book being reviewed", Nullable = false)]
    [Required(ErrorMessage = "BookId is required.")]
    public int BookId { get; set; }
}
