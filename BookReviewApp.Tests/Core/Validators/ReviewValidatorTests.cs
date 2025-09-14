using BookReviewApp.Core.Validators;
using BookReviewApp.Domain.Models;
using FluentAssertions;

namespace BookReviewApp.Tests.Core.Validators;

public class ReviewValidatorTests
{
    [Fact]
    public void Validate_ValidReview_ReturnsSuccess()
    {
        // Arrange
        var review = new Review
        {
            Content = "Great book!",
            Rating = 5,
            BookId = 1,
            UserId = "user1"
        };

        // Act
        var result = ReviewValidator.Validate(review);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Review is validated.");
    }

    [Fact]
    public void Validate_NullReview_ReturnsFailed()
    {
        // Arrange
        Review? review = null;

        // Act
        var result = ReviewValidator.Validate(review!);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Review cannot be null");
    }

    [Theory]
    [InlineData("", 5, 1, "Review content is required")]
    [InlineData("Valid content", 0, 1, "Rating must be between 1 and 5")]
    [InlineData("Valid content", 6, 1, "Rating must be between 1 and 5")]
    [InlineData("Valid content", 3, 0, "Invalid BookId for the review")]
    public void Validate_InvalidReviewProperties_ReturnsFailed(string content, int rating, int bookId, string expectedMessage)
    {
        // Arrange
        var review = new Review
        {
            Content = content,
            Rating = rating,
            BookId = bookId,
            UserId = "user1"
        };

        // Act
        var result = ReviewValidator.Validate(review);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void Validate_ContentTooLong_ReturnsFailed()
    {
        // Arrange
        var review = new Review
        {
            Content = new string('a', 1001),
            Rating = 4,
            BookId = 1,
            UserId = "user1"
        };

        // Act
        var result = ReviewValidator.Validate(review);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Review content cannot exceed 1000 characters");
    }
}