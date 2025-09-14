using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
namespace BookReviewApp.Api.Models.Request;

[SwaggerSchema("Request model for creating a new book", Title = "BookCreate")]
public class BookCreateRequest
{
    [SwaggerSchema("Title of the book", Nullable = false)]
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [SwaggerSchema("Author of the book", Nullable = false)]
    [Required(ErrorMessage = "Author is required.")]
    [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters.")]
    public string Author { get; set; } = string.Empty;

    [SwaggerSchema("Year the book was published", Nullable = false)]
    [Required(ErrorMessage = "Published year is required.")]
    [Range(0, 2025, ErrorMessage = "Published year must be between 0 and 2025.")]
    public int PublishedYear { get; set; }

    [SwaggerSchema("Genre of the book", Nullable = false)]
    [Required(ErrorMessage = "Genre is required.")]
    [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
    public string Genre { get; set; } = string.Empty;
}

