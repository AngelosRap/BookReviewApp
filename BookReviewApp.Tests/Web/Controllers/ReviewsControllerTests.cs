using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;
using BookReviewApp.Tests.Extensions;
using BookReviewApp.Web.Controllers;
using BookReviewApp.Web.Models.Review;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BookReviewApp.Tests.Web.Controllers;

public class ReviewsControllerTests
{
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly Mock<IBookService> _mockBookService;
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly ReviewsController _controller;

    private const string UserId = "user123";
    private const string BookFoundMessage = "Book found.";
    private const string BookNotFoundMessage = "Book not found";
    private const string ReviewCreatedMessage = "Review created.";

    public ReviewsControllerTests()
    {
        _mockReviewService = new Mock<IReviewService>();
        _mockBookService = new Mock<IBookService>();

        var store = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        _controller = new ReviewsController(
            _mockReviewService.Object,
            _mockBookService.Object,
            _mockUserManager.Object
        );
    }

    [Fact]
    public async Task Create_Get_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var bookId = 5;
        _mockBookService.SetupGetBookFail(bookId, $"Book with ID {bookId} not found.");

        // Act
        var result = await _controller.Create(bookId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be($"Book with ID {bookId} not found.");

        _mockBookService.Verify(s => s.Get(bookId), Times.Once);
    }

    [Fact]
    public async Task Create_Get_ShouldReturnView_WhenBookExists()
    {
        // Arrange
        var bookId = 5;
        _mockBookService.SetupGetBookSuccess(bookId, BookFoundMessage);

        // Act
        var result = await _controller.Create(bookId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewData["BookId"].Should().Be(bookId);

        _mockBookService.Verify(s => s.Get(bookId), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnSameView_WhenModelInvalid()
    {
        // Arrange
        var bookId = 5;
        var vm = new ReviewCreateViewModel();
        _controller.ModelState.AddModelError("Rating", "Required");

        // Act
        var result = await _controller.Create(vm, bookId);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeEquivalentTo(vm);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnNotFound_WhenUserIdIsNull()
    {
        // Arrange
        var bookId = 5;
        var vm = new ReviewCreateViewModel();

        _mockUserManager
            .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns((string?)null);

        _mockBookService.SetupGetBookSuccess(bookId, BookFoundMessage);

        // Act
        var result = await _controller.Create(vm, bookId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        _mockUserManager.Verify(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var bookId = 5;
        var vm = new ReviewCreateViewModel();

        _mockBookService.SetupGetBookFail(bookId, $"{BookNotFoundMessage} ID {bookId}");

        // Act
        var result = await _controller.Create(vm, bookId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be($"{BookNotFoundMessage} ID {bookId}");

        _mockBookService.Verify(s => s.Get(bookId), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ShouldRedirectToBookDetails_WhenValid()
    {
        // Arrange
        var bookId = 5;
        var vm = new ReviewCreateViewModel { Rating = 5, Content = "Excellent" };

        _mockBookService.SetupGetBookSuccess(bookId, BookFoundMessage);

        _mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                        .Returns(UserId);

        _mockReviewService.Setup(s => s.Create(It.IsAny<Review>()))
            .ReturnsAsync(Result<Review>.CreateSuccessful(It.IsAny<Review>(), ReviewCreatedMessage));

        // Act
        var result = await _controller.Create(vm, bookId);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Details");
        redirect.ControllerName.Should().Be("Books");
        redirect.RouteValues!["id"].Should().Be(bookId);

        _mockReviewService.Verify(s => s.Create(
            It.Is<Review>(r => r.UserId == UserId && r.BookId == bookId && r.Rating == vm.Rating)), Times.Once);
    }
}
