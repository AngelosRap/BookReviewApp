using BookReviewApp.Api.Controllers;
using BookReviewApp.Api.Mappings;
using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response.Book;
using BookReviewApp.Api.Models.Response.Review;
using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReviewApp.Tests.Api.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly BooksController _controller;

    private const string BookNotFoundMsg = "Book not found";
    private const string BookCreatedMsg = "Book created successfully";

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _mockReviewService = new Mock<IReviewService>();
        _controller = new BooksController(_mockBookService.Object, _mockReviewService.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenBooksExist()
    {
        // Arrange
        var books = new List<Book> { new() { Id = 1, Title = "Book 1" } };
        _mockBookService.Setup(s => s.GetAll(null, null, null, false)).ReturnsAsync(books);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var model = okResult.Value.Should().BeAssignableTo<List<BookListResponse>>().Subject;
        model.Should().HaveCount(1);
        _mockBookService.Verify(s => s.GetAll(null, null, null, false), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnNotFound_WhenNoBooks()
    {
        // Arrange
        _mockBookService.Setup(s => s.GetAll(null, null, null, false)).ReturnsAsync([]);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockBookService.Verify(s => s.GetAll(null, null, null, false), Times.Once);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Book 1" };
        _mockBookService.Setup(s => s.Get(1)).ReturnsAsync(Result<Book>.CreateSuccessful(book, "Found"));

        // Act
        var result = await _controller.Get(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<BookDetailResponse>();

        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _mockBookService.Setup(s => s.Get(1)).ReturnsAsync(Result<Book>.CreateFailed(BookNotFoundMsg));

        // Act
        var result = await _controller.Get(1);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(BookNotFoundMsg);
        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task GetReviews_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        var book = new Book { Id = 1 };
        var reviews = new List<Review> { new() { Id = 1, Content = "Great!" } };

        _mockBookService.Setup(s => s.Get(1)).ReturnsAsync(Result<Book>.CreateSuccessful(book, "Found"));
        _mockReviewService.Setup(s => s.GetByBookId(1)).ReturnsAsync(reviews);

        // Act
        var result = await _controller.GetReviews(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeAssignableTo<IEnumerable<ReviewResponse>>();
        _mockBookService.Verify(s => s.Get(1), Times.Once);
        _mockReviewService.Verify(s => s.GetByBookId(1), Times.Once);
    }

    [Fact]
    public async Task GetReviews_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _mockBookService.Setup(s => s.Get(1)).ReturnsAsync(Result<Book>.CreateFailed(BookNotFoundMsg));

        // Act
        var result = await _controller.GetReviews(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockBookService.Verify(s => s.Get(1), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        var request = new BookCreateRequest { Title = "Book 1", Author = "Author" };
        var book = request.ToBookEntity();
        _mockBookService.Setup(s => s.Create(It.IsAny<Book>())).ReturnsAsync(Result<Book>.CreateSuccessful(book, BookCreatedMsg));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(BooksController.Get));
        createdResult.Value.Should().BeOfType<BookDetailResponse>();
        _mockBookService.Verify(s => s.Create(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelInvalid()
    {
        // Arrange
        var request = new BookCreateRequest();
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenServiceFails()
    {
        // Arrange
        var request = new BookCreateRequest { Title = "Book 1", Author = "Author" };
        _mockBookService.Setup(s => s.Create(It.IsAny<Book>())).ReturnsAsync(Result<Book>.CreateFailed("Failed"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Failed");
        _mockBookService.Verify(s => s.Create(It.IsAny<Book>()), Times.Once);
    }
}
