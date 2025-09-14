using BookReviewApp.Core.Validators;
using BookReviewApp.Domain.Models;
using FluentAssertions;

namespace BookReviewApp.Tests.Core.Validators;

public class BookValidatorTests
{
    [Fact]
    public void Validate_ValidBook_ReturnsSuccess()
    {
        // Arrange
        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Genre = "Programming",
            PublishedYear = 2020
        };

        // Act
        var result = BookValidator.Validate(book);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Book is validated.");
    }

    [Fact]
    public void Validate_NullBook_ReturnsFailed()
    {
        // Arrange
        Book? book = null;

        // Act
        var result = BookValidator.Validate(book!);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Book cannot be null");
    }

    [Theory]
    [InlineData("", "Author", "Genre", 2020, "Book title is required")]
    [InlineData("Title", "", "Genre", 2020, "Book author is required")]
    [InlineData("Title", "Author", "", 2020, "Book genre is required")]
    [InlineData("Title", "Author", "Genre", 0, "Published year must be a positive number and between 0 and 2025")]
    [InlineData("Title", "Author", "Genre", 3000, "Published year must be a positive number and between 0 and 2025")]
    public void Validate_InvalidBookProperties_ReturnsFailed(string title, string author, string genre, int year, string expectedMessage)
    {
        // Arrange
        var book = new Book
        {
            Title = title,
            Author = author,
            Genre = genre,
            PublishedYear = year
        };

        // Act
        var result = BookValidator.Validate(book);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be(expectedMessage);
    }
}