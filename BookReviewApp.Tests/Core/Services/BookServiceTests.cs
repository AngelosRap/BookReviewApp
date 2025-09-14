using BookReviewApp.Core.Services;
using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using FluentAssertions;
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

        result.Success.Should().BeTrue();
        book.Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_DuplicateBook_ReturnsFailure()
    {
        var book1 = new Book { Title = "Test Book", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        var book2 = new Book { Title = "Test Book", Author = "Author A", PublishedYear = 2024, Genre = "Fiction" };

        await _bookService.Create(book1);
        var result = await _bookService.Create(book2);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_InvalidBook_ReturnsValidationError()
    {
        var book = new Book { Title = "", Author = "", PublishedYear = 2023, Genre = "" };

        var result = await _bookService.Create(book);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Get_BookExists_ReturnsBook()
    {
        // Arrange
        var book = new Book { Title = "DDD", Author = "Evans", Genre = "Tech", PublishedYear = 2003 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.Get(book.Id);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_BookDoesNotExists_ReturnsFailedResult()
    {
        // Arrange
        var nonExistentBookId = 999;

        // Act
        var result = await _bookService.Get(nonExistentBookId);

        // Assert
        result.Failed.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Contain(nonExistentBookId.ToString());
    }

    [Fact]
    public async Task Get_BookWithReviewsAndVotes_ReturnsBookWithDetails()
    {
        // Arrange
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

        // Act
        var result = await _bookService.Get(book.Id);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(book.Title);

        // Check that Reviews are included
        result.Data.Reviews.Should().NotBeEmpty();
        var returnedReview = result.Data.Reviews.First();
        returnedReview.Content.Should().Be(review.Content);

        // Check that Votes are included
        returnedReview.Votes.Should().NotBeEmpty();
        var returnedVote = returnedReview.Votes.First();
        returnedVote.IsUpvote.Should().BeTrue();
        returnedVote.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetAll_NoFilters_ReturnsAllBooks()
    {
        // Arrange
        var book1 = new Book { Title = "Book 1", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        var book2 = new Book { Title = "Book 2", Author = "Author B", PublishedYear = 2021, Genre = "Non-Fiction" };

        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetAll();

        // Assert
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetAll_FilterByAuthor_ReturnsMatchingBooks()
    {
        // Arrange
        var book1 = new Book { Title = "Book 1", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        var book2 = new Book { Title = "Book 2", Author = "Author B", PublishedYear = 2021, Genre = "Non-Fiction" };

        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetAll(author: "Author A");

        // Assert
        result.Should().ContainSingle()
            .Which.Author.Should().Be("Author A");
    }

    [Fact]
    public async Task GetAll_FilterByGenreAndYear_ReturnsMatchingBooks()
    {
        // Arrange
        var book1 = new Book { Title = "Book 1", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        var book2 = new Book { Title = "Book 2", Author = "Author B", PublishedYear = 2021, Genre = "Fiction" };
        var book3 = new Book { Title = "Book 3", Author = "Author C", PublishedYear = 2021, Genre = "Non-Fiction" };

        _context.Books.AddRange(book1, book2, book3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetAll(genre: "Fiction", year: 2021);

        // Assert
        result.Should().ContainSingle()
            .Which.Title.Should().Be("Book 2");
    }

    [Fact]
    public async Task GetAll_WithDetails_IncludesReviews()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };

        var book1 = new Book
        {
            Title = "Test Book A",
            Author = "Author A",
            PublishedYear = 2023,
            Genre = "Fiction"
        };
        var book2 = new Book
        {
            Title = "Test Book B",
            Author = "Author B",
            PublishedYear = 2022,
            Genre = "Non-Fiction"
        };

        var review1 = new Review
        {
            Content = "Amazing book!",
            Rating = 5,
            UserId = user.Id,
            Book = book1
        };

        var review2 = new Review
        {
            Content = "Informative read.",
            Rating = 4,
            UserId = user.Id,
            Book = book2
        };

        var vote1 = new ReviewVote
        {
            IsUpvote = true,
            UserId = user.Id,
            Review = review1
        };

        var vote2 = new ReviewVote
        {
            IsUpvote = true,
            UserId = user.Id,
            Review = review2
        };
        review1.Votes.Add(vote1);
        review2.Votes.Add(vote2);

        book1.Reviews.Add(review1);
        book2.Reviews.Add(review2);

        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetAll(withDetails: true);

        // Assert
        Assert.Equal(2, result.Count);

        var firstBook = result.First(b => b.Title == "Test Book A");
        var secondBook = result.First(b => b.Title == "Test Book B");

        firstBook.Reviews.Should().ContainSingle()
            .Which.Content.Should().Contain("Amazing");

        firstBook.Reviews.First().Votes.Should().ContainSingle()
            .Which.IsUpvote.Should().Be(vote1.IsUpvote);

        secondBook.Reviews.Should().ContainSingle()
            .Which.Content.Should().Contain("Informative");

        secondBook.Reviews.First().Votes.Should().ContainSingle()
            .Which.IsUpvote.Should().Be(vote2.IsUpvote);
    }


    [Fact]
    public async Task Update_ExistingBook_ReturnsSuccessfulResult()
    {
        // Arrange
        var book = new Book { Title = "Old Title", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var updatedBook = new Book
        {
            Id = book.Id,
            Title = "New Title",
            Author = "Author A",
            PublishedYear = 2021,
            Genre = "Fiction"
        };

        // Act
        var result = await _bookService.Update(updatedBook);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("New Title");
        result.Data.PublishedYear.Should().Be(2021);
    }

    [Fact]
    public async Task Update_NonExistentBook_ReturnsFailedResult()
    {
        // Arrange
        var book = new Book { Id = 999, Title = "Title", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };

        // Act
        var result = await _bookService.Update(book);

        // Assert
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_InvalidBook_ReturnsValidationError()
    {
        // Arrange
        var book = new Book { Title = "Valid Title", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var invalidBook = new Book
        {
            Id = book.Id,
            Title = "", // invalid
            Author = "Author A",
            PublishedYear = 2020,
            Genre = "Fiction"
        };

        // Act
        var result = await _bookService.Update(invalidBook);

        // Assert
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ExistingBook_ReturnsSuccessfulResult()
    {
        // Arrange
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2020, Genre = "Fiction" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.Delete(book.Id);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Book deleted successfully.");

        var exists = await _context.Books.AnyAsync(b => b.Id == book.Id);
        exists.Should().BeFalse(); // Confirm it was removed from the database
    }

    [Fact]
    public async Task Delete_NonExistentBook_ReturnsFailedResult()
    {
        // Act
        var result = await _bookService.Delete(999);

        // Assert
        result.Failed.Should().BeTrue();
    }
}