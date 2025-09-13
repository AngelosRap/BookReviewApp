using System.ComponentModel.DataAnnotations;
namespace BookReviewApp.Web.Models.Review;

public class ReviewCreateViewModel
{
    [Required(ErrorMessage = "Review content is required.")]
    [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }
}