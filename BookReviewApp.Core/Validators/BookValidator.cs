using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Core.Validators;

public static class BookValidator
{
    public static Result<Book> Validate(Book book)
    {
        if (book == null)
        {
            return Result<Book>.CreateFailed("Book cannot be null");
        }

        if (string.IsNullOrWhiteSpace(book.Title))
        {
            return Result<Book>.CreateFailed("Book title is required");
        }

        if (string.IsNullOrWhiteSpace(book.Author))
        {
            return Result<Book>.CreateFailed("Book author is required");
        }

        if (string.IsNullOrWhiteSpace(book.Genre))
        {
            return Result<Book>.CreateFailed("Book genre is required");
        }

        if (book.PublishedYear <= 0 || book.PublishedYear > DateTime.UtcNow.Year)
        {
            return Result<Book>.CreateFailed("Published year must be a positive number and between 0 and 2025");
        }

        return Result<Book>.CreateSuccessful(book, "Book is validated.");
    }
}
