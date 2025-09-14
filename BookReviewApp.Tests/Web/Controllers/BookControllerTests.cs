using BookReviewApp.Core.Interfaces;
using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Controllers;
using BookReviewApp.Web.Models.Book;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReviewApp.Tests.Web.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithFilteredBooks_WhenMinRatingIsProvided()
    {
        // Arrange
        var books = new List<Book>
        {
            new() { Id = 1, Title = "Book 1", Reviews = [new Review { Rating = 5 }, new Review { Rating = 4}] },
            new() { Id = 2, Title = "Book 2", Reviews = [new Review { Rating = 3 }] },
        };
        _mockBookService.Setup(s => s.GetAll(null, null, null, true)).ReturnsAsync(books);

        // Act
        var result = await _controller.Index(genre: null, year: null, minRating: 3);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<List<BookDisplayViewModel>>().Subject;
        model.Should().HaveCount(2);
        model.First().Title.Should().Be("Book 1");
    }
}