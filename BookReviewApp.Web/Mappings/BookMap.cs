using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Models.Book;

namespace BookReviewApp.Web.Mappings;

public static class BookMap
{
    public static BookDisplayViewModel ToViewModel(this Book book)
    {
        return new BookDisplayViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishedYear = book.PublishedYear,
            Genre = book.Genre,
            ReviewsCount = book.Reviews?.Count ?? 0,
            AverageRating = book.Reviews != null && book.Reviews.Any()
                ? book.Reviews.Average(r => r.Rating)
                : 0,
            Reviews = book.Reviews?.Select(r => r.ToViewModel()).ToList() ?? []
        };
    }

    public static BookEditViewModel ToEditViewModel(this Book book)
    {
        return new BookEditViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishedYear = book.PublishedYear,
            Genre = book.Genre,
        };
    }

    public static Book UpdateWithEditViewModel(this Book book, BookEditViewModel bookEditViewModel)
    {
        book.Title = bookEditViewModel.Title;
        book.Author = bookEditViewModel.Author;
        book.PublishedYear = bookEditViewModel.PublishedYear;
        book.Genre = bookEditViewModel.Genre;

        return book;
    }

    public static Book ToEntity(this BookCreateViewModel bookCreateViewModel)
    {
        return new Book
        {
            Title = bookCreateViewModel.Title,
            Author = bookCreateViewModel.Author,
            PublishedYear = bookCreateViewModel.PublishedYear,
            Genre = bookCreateViewModel.Genre
        };
    }

    public static Book ToEntity(this BookEditViewModel bookEditViewModel)
    {
        return new Book
        {
            Id = bookEditViewModel.Id,
            Title = bookEditViewModel.Title,
            Author = bookEditViewModel.Author,
            PublishedYear = bookEditViewModel.PublishedYear,
            Genre = bookEditViewModel.Genre
        };
    }
}
