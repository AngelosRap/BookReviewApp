using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Models;

namespace BookReviewApp.Web.Mappings;

public static class BookMap
{
    public static BookViewModel ToViewModel(this Book book)
    {
        return new BookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishedYear = book.PublishedYear,
            Genre = book.Genre,
            ReviewsCount = book.Reviews?.Count ?? 0,
            AverageRating = book.Reviews != null && book.Reviews.Any()
                ? book.Reviews.Average(r => r.Rating)
                : 0
        };
    }
}
