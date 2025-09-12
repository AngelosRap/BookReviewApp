namespace BookReviewApp.Web.Models;

public class BookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int ReviewsCount { get; set; }
    public double AverageRating { get; set; }
}
