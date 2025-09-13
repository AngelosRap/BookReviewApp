using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Web.Models.Book;

public class BookCreateViewModel
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Published year is required.")]
    [Range(0, 2025, ErrorMessage = "Published year must be between 0 and 2025.")]
    public int PublishedYear { get; set; }

    [Required(ErrorMessage = "Genre is required.")]
    [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
    public string Genre { get; set; } = string.Empty;
}

