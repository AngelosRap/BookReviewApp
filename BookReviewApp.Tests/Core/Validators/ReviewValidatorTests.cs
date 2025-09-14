using BookReviewApp.Core.Validators;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Tests.Core.Validators;
public class ReviewValidatorTests
{
    [Fact]
    public void Validate_ValidReview_ReturnsSuccess()
    {
        var review = new Review
        {
            Content = "Great book!",
            Rating = 5,
            BookId = 1,
            UserId = "user1"
        };

        var result = ReviewValidator.Validate(review);

        Assert.True(result.Success);
        Assert.Equal("Review is validated.", result.Message);
    }

    [Fact]
    public void Validate_NullReview_ReturnsFailed()
    {
        Review? review = null;

        var result = ReviewValidator.Validate(review!);

        Assert.False(result.Success);
        Assert.Equal("Review cannot be null", result.Message);
    }

    [Theory]
    [InlineData("", 5, 1, "Review content is required")]
    [InlineData("Valid content", 0, 1, "Rating must be between 1 and 5")]
    [InlineData("Valid content", 6, 1, "Rating must be between 1 and 5")]
    [InlineData("Valid content", 3, 0, "Invalid BookId for the review")]
    public void Validate_InvalidReviewProperties_ReturnsFailed(string content, int rating, int bookId, string expectedMessage)
    {
        var review = new Review
        {
            Content = content,
            Rating = rating,
            BookId = bookId,
            UserId = "user1"
        };

        var result = ReviewValidator.Validate(review);

        Assert.False(result.Success);
        Assert.Equal(expectedMessage, result.Message);
    }

    [Fact]
    public void Validate_ContentTooLong_ReturnsFailed()
    {
        var review = new Review
        {
            Content = new string('a', 1001),
            Rating = 4,
            BookId = 1,
            UserId = "user1"
        };

        var result = ReviewValidator.Validate(review);

        Assert.False(result.Success);
        Assert.Equal("Review content cannot exceed 1000 characters", result.Message);
    }
}
