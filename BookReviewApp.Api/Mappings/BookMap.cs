using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response.Book;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Api.Mappings;

public static class BookMap
{
    public static BookListResponse ToListResponse(this Book book)
    {
        return new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishedYear = book.PublishedYear,
            Genre = book.Genre,
        };
    }

    public static BookDetailResponse ToDetailResponse(this Book book)
    {
        return new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishedYear = book.PublishedYear,
            Genre = book.Genre,
            Reviews = book.Reviews.Select(x => x.ToResponse()).ToList(),
        };
    }

    public static Book ToBookEntity(this BookCreateRequest bookCreateRequest)
    {
        return new()
        {
            Title = bookCreateRequest.Title,
            Author = bookCreateRequest.Author,
            PublishedYear = bookCreateRequest.PublishedYear,
            Genre = bookCreateRequest.Genre,
        };
    }
}
