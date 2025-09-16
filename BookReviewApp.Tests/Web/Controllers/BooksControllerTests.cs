using BookReviewApp.Core.Interfaces;
using BookReviewApp.Domain.Models;
using BookReviewApp.Tests.Extensions;
using BookReviewApp.Web.Controllers;
using BookReviewApp.Web.Mappings;
using BookReviewApp.Web.Models.Book;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReviewApp.Tests.Web.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BooksController _controller;

    private const string BookFoundMessage = "Book found.";
    private const string BookNotFoundMessage = "Book not Found";
    private const string BookCreatedMessage = "Created successfully.";
    private const string BookUpdatedMessage = "Book updated.";
    private const string BookUpdateFailedMessage = "Book update failed.";

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
            new() { Id = 1, Title = "Book 1", Reviews = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 4 } } },
            new() { Id = 2, Title = "Book 2", Reviews = new List<Review> { new Review { Rating = 3 } } },
        };
        _mockBookService.Setup(s => s.GetAll(null, null, null, true)).ReturnsAsync(books);

        // Act
        var result = await _controller.Index(genre: null, year: null, minRating: 3);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<List<BookDisplayViewModel>>().Subject;
        model.Should().HaveCount(2);
        model.First().Title.Should().Be("Book 1");

        _mockBookService.Verify(s => s.GetAll(null, null, null, true), Times.Once);
    }

    [Fact]
    public void Create_Get_ShouldReturnView()
    {
        // Arrange & Act
        var result = _controller.Create();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ShouldReturnSameView()
    {
        // Arrange
        var bookCreateViewModel = new BookCreateViewModel();
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.Create(bookCreateViewModel);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeEquivalentTo(bookCreateViewModel);
    }

    [Fact]
    public async Task Create_Post_ValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var bookCreateViewModel = new BookCreateViewModel
        {
            Title = "Test Book",
            Author = "Test Author",
            Genre = "Fiction",
            PublishedYear = 2000
        };
        var book = bookCreateViewModel.ToEntity();
        _mockBookService.SetupCreateBookSuccess(book, BookCreatedMessage);

        // Act
        var result = await _controller.Create(bookCreateViewModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
        _mockBookService.Verify(s => s.Create(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenBookMissing()
    {
        // Arrange
        _mockBookService.SetupGetBookFail(1, BookNotFoundMessage);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be(BookNotFoundMessage);
        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnViewWithModel_WhenBookExists()
    {
        // Arrange
        _mockBookService.SetupGetBookSuccess(1, BookFoundMessage);
        var expectedBookEditViewModel = new Book { Id = 1 }.ToEditViewModel();

        // Act
        var result = await _controller.Edit(1);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeEquivalentTo(expectedBookEditViewModel);
        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnNotFound_WhenBookMissing()
    {
        // Arrange
        _mockBookService.SetupGetBookFail(1, BookNotFoundMessage);
        var bookEditViewModel = new BookEditViewModel();

        // Act
        var result = await _controller.Edit(1, bookEditViewModel);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be(BookNotFoundMessage);
        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnSameView_WhenModelInvalid()
    {
        // Arrange
        _mockBookService.SetupGetBookSuccess(1, BookFoundMessage);
        var bookEditViewModel = new Book { Id = 1 }.ToEditViewModel();
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.Edit(1, bookEditViewModel);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeEquivalentTo(bookEditViewModel);
        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ShouldRedirectToIndex_WhenUpdateSucceeds()
    {
        // Arrange
        _mockBookService.SetupGetBookSuccess(1, BookFoundMessage);
        var book = new Book { Id = 1, Title = "Old" };
        _mockBookService.SetupUpdateBookSuccess(book, BookUpdatedMessage);
        var model = new Book { Id = 1, Title = "Updated" }.ToEditViewModel();

        // Act
        var result = await _controller.Edit(1, model);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
        _mockBookService.Verify(s => s.Get(1), Times.Once);
        _mockBookService.Verify(s => s.Update(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        _mockBookService.SetupGetBookSuccess(1, BookFoundMessage);
        _mockBookService.SetupUpdateBookFail(BookUpdateFailedMessage);
        var model = new Book { Id = 1, Title = "Updated" }.ToEditViewModel();

        // Act
        var result = await _controller.Edit(1, model);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be(BookUpdateFailedMessage);
        _mockBookService.Verify(s => s.Get(1), Times.Once);
        _mockBookService.Verify(s => s.Update(It.IsAny<Book>()), Times.Once);
    }
}
