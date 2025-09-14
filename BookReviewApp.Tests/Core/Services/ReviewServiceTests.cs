using BookReviewApp.Core.Services;
using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Core.Services;

public class ReviewServiceTests
{
    private readonly Context _context;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new Context(options);
        _reviewService = new ReviewService(_context);
    }

    [Fact]
    public async Task Create_ValidReview_ReturnsSuccess()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };

        _context.Users.Add(user);
        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        var review = new Review
        {
            Content = "Excellent book!",
            Rating = 5,
            UserId = user.Id,
            BookId = book.Id
        };

        // Act
        var result = await _reviewService.Create(review);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Content.Should().Be("Excellent book!");
    }

    [Fact]
    public async Task Create_InvalidReview_ReturnsValidationError()
    {
        // Arrange
        var review = new Review { Content = "", Rating = 6, UserId = "user1", BookId = 1 };

        // Act
        var result = await _reviewService.Create(review);

        // Assert
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_NonExistentUserOrBook_ReturnsFailedResult()
    {
        // Arrange
        var review = new Review { Content = "Test", Rating = 5, UserId = "invalidUser", BookId = 999 };

        // Act
        var result = await _reviewService.Create(review);

        // Assert
        result.Failed.Should().BeTrue();
        result.Message.Should().Contain("does not exist");
    }

    [Fact]
    public async Task Get_ExistingReview_ReturnsReview()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Books.Add(book);
        _context.Users.Add(user);

        var review = new Review { Content = "Great!", Rating = 5, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        // Act
        var result = await _reviewService.Get(review.Id);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Content.Should().Be("Great!");
    }

    [Fact]
    public async Task Get_NonExistentReview_ReturnsFailedResult()
    {
        // Arrange, Act & Assert
        var result = await _reviewService.Get(999);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task GetByBookId_ReturnsAllReviewsForBook()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review1 = new Review { Content = "Review 1", Rating = 5, UserId = user.Id, BookId = book.Id };
        var review2 = new Review { Content = "Review 2", Rating = 4, UserId = user.Id, BookId = book.Id };

        _context.Reviews.AddRange(review1, review2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reviewService.GetByBookId(book.Id);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Update_ExistingReview_ReturnsSuccess()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review = new Review { Content = "Good", Rating = 4, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        review.Content = "Excellent";

        // Act
        var result = await _reviewService.Update(review);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(book.Reviews.First().Id, result.Data!.BookId);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("Excellent", result.Data!.Content);
    }

    [Fact]
    public async Task Update_NonExistentReview_ReturnsFailedResult()
    {
        // Arrange
        var review = new Review { Id = 999, Content = "Test", Rating = 5, UserId = "user1", BookId = 1 };

        // Act
        var result = await _reviewService.Update(review);

        // Assert
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ExistingReview_ReturnsSuccess()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review = new Review { Content = "Good", Rating = 4, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        // Act
        var result = await _reviewService.Delete(review.Id);

        // Assert
        Assert.True(result.Success);
        Assert.False(await _context.Reviews.AnyAsync(r => r.Id == review.Id));
    }

    [Fact]
    public async Task Delete_NonExistentReview_ReturnsFailedResult()
    {
        // Arrange, Act & Assert
        var result = await _reviewService.Delete(999);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task Vote_NewVote_AddsVoteSuccessfully()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review = new Review { Content = "Nice", Rating = 5, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        // Act
        var result = await _reviewService.Vote(user.Id, review.Id, true);

        // Assert
        result.Success.Should().BeTrue();

        var updatedReview = await _context.Reviews
            .Include(r => r.Votes)
            .FirstAsync(r => r.Id == review.Id);

        updatedReview.Votes.Should().HaveCount(1);
        updatedReview.Votes.First().IsUpvote.Should().BeTrue();
    }

    [Fact]
    public async Task Vote_ExistingVote_UpdatesVoteSuccessfully()
    {
        // Arrange
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        var review = new Review { Content = "Nice", Rating = 5, UserId = user.Id, BookId = book.Id };
        var vote = new ReviewVote { UserId = user.Id, Review = review, IsUpvote = false };

        review.Votes.Add(vote);
        _context.Users.Add(user);
        _context.Books.Add(book);
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reviewService.Vote(user.Id, review.Id, true);

        // Assert
        result.Success.Should().BeTrue();

        var updatedReview = await _context.Reviews
            .Include(r => r.Votes)
            .FirstAsync(r => r.Id == review.Id);

        updatedReview.Votes.Should().HaveCount(1);
        updatedReview.Votes.First().IsUpvote.Should().BeTrue();
    }
}