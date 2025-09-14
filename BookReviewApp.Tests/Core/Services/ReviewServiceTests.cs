using BookReviewApp.Core.Services;
using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Core.Services;

public class ReviewServiceTests
{
    private readonly Context _context;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new Context(options);
        _reviewService = new ReviewService(_context);
    }

    [Fact]
    public async Task Create_ValidReview_ReturnsSuccess()
    {
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

        var result = await _reviewService.Create(review);

        Assert.True(result.Success);
        Assert.Equal("Excellent book!", result.Data!.Content);
    }

    [Fact]
    public async Task Create_InvalidReview_ReturnsValidationError()
    {
        var review = new Review { Content = "", Rating = 6, UserId = "user1", BookId = 1 };

        var result = await _reviewService.Create(review);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Create_NonExistentUserOrBook_ReturnsFailedResult()
    {
        var review = new Review { Content = "Test", Rating = 5, UserId = "invalidUser", BookId = 999 };

        var result = await _reviewService.Create(review);

        Assert.True(result.Failed);
        Assert.Contains("does not exist", result.Message);
    }

    [Fact]
    public async Task Get_ExistingReview_ReturnsReview()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Books.Add(book);
        _context.Users.Add(user);

        var review = new Review { Content = "Great!", Rating = 5, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        var result = await _reviewService.Get(review.Id);

        Assert.True(result.Success);
        Assert.Equal("Great!", result.Data!.Content);
    }

    [Fact]
    public async Task Get_NonExistentReview_ReturnsFailedResult()
    {
        var result = await _reviewService.Get(999);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task GetByBookId_ReturnsAllReviewsForBook()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review1 = new Review { Content = "Review 1", Rating = 5, UserId = user.Id, BookId = book.Id };
        var review2 = new Review { Content = "Review 2", Rating = 4, UserId = user.Id, BookId = book.Id };

        _context.Reviews.AddRange(review1, review2);
        await _context.SaveChangesAsync();

        var result = await _reviewService.GetByBookId(book.Id);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Update_ExistingReview_ReturnsSuccess()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review = new Review { Content = "Good", Rating = 4, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        review.Content = "Excellent";
        var result = await _reviewService.Update(review);

        Assert.True(result.Success);
        Assert.Equal(book.Reviews.First().Id, result.Data!.BookId);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("Excellent", result.Data!.Content);
    }

    [Fact]
    public async Task Update_NonExistentReview_ReturnsFailedResult()
    {
        var review = new Review { Id = 999, Content = "Test", Rating = 5, UserId = "user1", BookId = 1 };

        var result = await _reviewService.Update(review);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Delete_ExistingReview_ReturnsSuccess()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review = new Review { Content = "Good", Rating = 4, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        var result = await _reviewService.Delete(review.Id);

        Assert.True(result.Success);
        Assert.False(await _context.Reviews.AnyAsync(r => r.Id == review.Id));
    }

    [Fact]
    public async Task Delete_NonExistentReview_ReturnsFailedResult()
    {
        var result = await _reviewService.Delete(999);

        Assert.True(result.Failed);
    }

    [Fact]
    public async Task Vote_NewVote_AddsVoteSuccessfully()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        _context.Users.Add(user);
        _context.Books.Add(book);

        var review = new Review { Content = "Nice", Rating = 5, UserId = user.Id, BookId = book.Id };
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        var result = await _reviewService.Vote(user.Id, review.Id, true);

        Assert.True(result.Success);

        var updatedReview = await _context.Reviews.Include(r => r.Votes).FirstAsync(r => r.Id == review.Id);
        Assert.Single(updatedReview.Votes);
        Assert.True(updatedReview.Votes.First().IsUpvote);
    }

    [Fact]
    public async Task Vote_ExistingVote_UpdatesVoteSuccessfully()
    {
        var user = new AppUser { Id = "user1", Email = "test@example.com" };
        var book = new Book { Title = "Book A", Author = "Author A", PublishedYear = 2023, Genre = "Fiction" };
        var review = new Review { Content = "Nice", Rating = 5, UserId = user.Id, BookId = book.Id };
        var vote = new ReviewVote { UserId = user.Id, Review = review, IsUpvote = false };

        review.Votes.Add(vote);
        _context.Users.Add(user);
        _context.Books.Add(book);
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var result = await _reviewService.Vote(user.Id, review.Id, true);

        Assert.True(result.Success);

        var updatedReview = await _context.Reviews.Include(r => r.Votes).FirstAsync(r => r.Id == review.Id);
        Assert.Single(updatedReview.Votes);
        Assert.True(updatedReview.Votes.First().IsUpvote);
    }
}
