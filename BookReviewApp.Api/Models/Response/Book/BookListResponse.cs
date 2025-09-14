using Swashbuckle.AspNetCore.Annotations;

namespace BookReviewApp.Api.Models.Response.Book;

[SwaggerSchema("Response model for listing books", Title = "BookListResponse")]
public class BookListResponse
{
    [SwaggerSchema("Unique identifier of the book")]
    public int Id { get; set; }

    [SwaggerSchema("Title of the book")]
    public string Title { get; set; } = string.Empty;

    [SwaggerSchema("Author of the book")]
    public string Author { get; set; } = string.Empty;

    [SwaggerSchema("Year the book was published")]
    public int PublishedYear { get; set; }

    [SwaggerSchema("Genre of the book")]
    public string Genre { get; set; } = string.Empty;
}
