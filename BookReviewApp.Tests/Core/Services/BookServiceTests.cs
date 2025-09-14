using BookReviewApp.Core.Services;
using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Core.Services;

public class BookServiceTests
{
    private readonly Context _context;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new Context(options);
        _bookService = new BookService(_context);
    }

    [Fact]
    public async Task Create_ValidBook_ReturnsSuccess()
    {
        var book = new Book { Title = "Test Book", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };

        var result = await _bookService.Create(book);

        Assert.True(result.Success);
        Assert.Equal(1, book.Id);
    }

    [Fact]
    public async Task Create_DuplicateBook_ReturnsFailure()
    {
        var book1 = new Book { Title = "Test Book", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        var book2 = new Book { Title = "Test Book", Author = "Author A", PublishedYear = 2024, Genre = "Fiction" };

        await _bookService.Create(book1);
        var result = await _bookService.Create(book2);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Create_InvalidBook_ReturnsValidationError()
    {
        var book = new Book { Title = "", Author = "", PublishedYear = 2023, Genre = "" };

        var result = await _bookService.Create(book);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Get_BookExists_ReturnsBook()
    {
        var book = new Book { Title = "DDD", Author = "Evans", Genre = "Tech", PublishedYear = 2003 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var result = await _bookService.Get(book.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Get_BookDoesNotExist_ReturnsFailedResult()
    {
        var nonExistentBookId = 999;

        var result = await _bookService.Get(nonExistentBookId);

        Assert.True(result.Failed);
        Assert.Null(result.Data);
        Assert.Contains(nonExistentBookId.ToString(), result.Message);
    }

    [Fact]
    public async Task Get_BookWithReviewsAndVotes_ReturnsBookWithDetails()
    {
        var book = new Book { Title = "Test Book", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        var user = new AppUser { Id = "user1", Email = "test@example.com" };

        var review = new Review
        {
            Content = "Great book!",
            Rating = 5,
            UserId = user.Id,
            Book = book
        };

        var vote = new ReviewVote
        {
            IsUpvote = true,
            UserId = user.Id,
            Review = review
        };

        review.Votes.Add(vote);
        book.Reviews.Add(review);

        _context.Users.Add(user);
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var result = await _bookService.Get(book.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(book.Title, result.Data!.Title);

        var returnedReview = result.Data.Reviews.First();
        Assert.Equal(review.Content, returnedReview.Content);

        var returnedVote = returnedReview.Votes.First();
        Assert.True(returnedVote.IsUpvote);
        Assert.Equal(user.Id, returnedVote.UserId);
    }

    [Fact]
    public async Task GetAll_NoFilters_ReturnsAllBooks()
    {
        var book1 = new Book { Title = "Book 1", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        var book2 = new Book { Title = "Book 2", Author = "Author B", PublishedYear = 2021, Genre = "Non-Fiction" };

        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        var result = await _bookService.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_FilterByAuthor_ReturnsMatchingBooks()
    {
        var book1 = new Book { Title = "Book 1", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        var book2 = new Book { Title = "Book 2", Author = "Author B", PublishedYear = 2021, Genre = "Non-Fiction" };

        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        var result = await _bookService.GetAll(author: "Author A");

        Assert.Single(result);
        Assert.Equal("Author A", result[0].Author);
    }

    [Fact]
    public async Task GetAll_FilterByGenreAndYear_ReturnsMatchingBooks()
    {
        var book1 = new Book { Title = "Book 1", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        var book2 = new Book { Title = "Book 2", Author = "Author B", PublishedYear = 2021, Genre = "Fiction" };
        var book3 = new Book { Title = "Book 3", Author = "Author C", PublishedYear = 2021, Genre = "Non-Fiction" };

        _context.Books.AddRange(book1, book2, book3);
        await _context.SaveChangesAsync();

        var result = await _bookService.GetAll(genre: "Fiction", year: 2021);

        Assert.Single(result);
        Assert.Equal("Book 2", result[0].Title);
    }

    [Fact]
    public async Task GetAll_WithDetails_IncludesReviewsAndVotes()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };

        var book1 = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        var book2 = new Book { Title = "Book B", Author = "Author B", PublishedYear = 2022, Genre = "Non-Fiction" };

        var review1 = new Review { Content = "Amazing book!", Rating = 5, UserId = user.Id, Book = book1 };
        var review2 = new Review { Content = "Informative read.", Rating = 4, UserId = user.Id, Book = book2 };

        var vote1 = new ReviewVote { IsUpvote = true, UserId = user.Id, Review = review1 };
        var vote2 = new ReviewVote { IsUpvote = true, UserId = user.Id, Review = review2 };

        review1.Votes.Add(vote1);
        review2.Votes.Add(vote2);

        book1.Reviews.Add(review1);
        book2.Reviews.Add(review2);

        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        var result = await _bookService.GetAll(withDetails: true);

        Assert.Equal(2, result.Count);

        var firstBook = result.First(b => b.Title == "Book A");
        var secondBook = result.First(b => b.Title == "Book B");

        Assert.Single(firstBook.Reviews);
        Assert.Contains("Amazing", firstBook.Reviews.First().Content);
        Assert.Single(firstBook.Reviews.First().Votes);
        Assert.True(firstBook.Reviews.First().Votes.First().IsUpvote);

        Assert.Single(secondBook.Reviews);
        Assert.Contains("Informative", secondBook.Reviews.First().Content);
        Assert.Single(secondBook.Reviews.First().Votes);
        Assert.True(secondBook.Reviews.First().Votes.First().IsUpvote);
    }

    [Fact]
    public async Task Update_ExistingBook_ReturnsSuccessfulResult()
    {
        var book = new Book { Title = "Old Title", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var updatedBook = new Book { Id = book.Id, Title = "New Title", Author = "Author A", PublishedYear = 2021, Genre = "Fiction" };

        var result = await _bookService.Update(updatedBook);

        Assert.True(result.Success);
        Assert.Equal("New Title", result.Data!.Title);
        Assert.Equal(2021, result.Data.PublishedYear);
    }

    [Fact]
    public async Task Update_NonExistentBook_ReturnsFailedResult()
    {
        var book = new Book { Id = 999, Title = "Title", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };

        var result = await _bookService.Update(book);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Update_InvalidBook_ReturnsValidationError()
    {
        var book = new Book { Title = "Valid Title", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var invalidBook = new Book { Id = book.Id, Title = "", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };

        var result = await _bookService.Update(invalidBook);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Delete_ExistingBook_ReturnsSuccessfulResult()
    {
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var result = await _bookService.Delete(book.Id);

        Assert.True(result.Success);
        Assert.Equal("Book deleted successfully.", result.Message);

        var exists = await _context.Books.AnyAsync(b => b.Id == book.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task Delete_NonExistentBook_ReturnsFailedResult()
    {
        var result = await _bookService.Delete(999);

        Assert.True(result.Failed);
    }
}