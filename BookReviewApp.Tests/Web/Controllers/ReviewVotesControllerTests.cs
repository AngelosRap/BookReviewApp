using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BookReviewApp.Tests.Web.Controllers;

public class ReviewVotesControllerTests
{
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly ReviewVotesController _controller;

    private const string UserId = "user123";

    public ReviewVotesControllerTests()
    {
        _mockReviewService = new Mock<IReviewService>();

        var store = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        _controller = new ReviewVotesController(_mockReviewService.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task Vote_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        int reviewId = 1, bookId = 5;
        var isUpvote = true;

        _mockUserManager
            .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns((string?)null);

        // Act
        var result = await _controller.Vote(reviewId, bookId, isUpvote);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
        _mockUserManager.Verify(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()), Times.Once);
    }

    [Fact]
    public async Task Vote_ShouldReturnNotFound_WhenReviewDoesNotExist()
    {
        // Arrange
        int reviewId = 1, bookId = 5;
        var isUpvote = true;

        _mockUserManager
            .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(UserId);
        _mockReviewService
            .Setup(s => s.Get(reviewId)).ReturnsAsync(Result<Review>.CreateFailed($"Review {reviewId} not found."));

        // Act
        var result = await _controller.Vote(reviewId, bookId, isUpvote);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
           .Which.Value.Should().Be($"Review {reviewId} not found.");

        _mockReviewService.Verify(s => s.Get(reviewId), Times.Once);
        _mockUserManager.Verify(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()), Times.Once);
    }

    [Fact]
    public async Task Vote_ShouldCallVoteAndRedirect_WhenValid()
    {
        // Arrange
        int reviewId = 1, bookId = 5;
        var isUpvote = true;
        var review = new Review { Id = reviewId };

        _mockUserManager
            .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(UserId);
        _mockReviewService
            .Setup(s => s.Get(reviewId)).ReturnsAsync(Result<Review>.CreateSuccessful(review, "Created successfully."));
        _mockReviewService
            .Setup(s => s.Vote(UserId, reviewId, isUpvote)).ReturnsAsync(Result.CreateSuccessful("Vote created successfully."));

        // Act
        var result = await _controller.Vote(reviewId, bookId, isUpvote);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Details");
        redirect.ControllerName.Should().Be("Books");
        redirect.RouteValues!["id"].Should().Be(bookId);

        _mockReviewService.Verify(s => s.Get(reviewId), Times.Once);
        _mockReviewService.Verify(s => s.Vote(UserId, reviewId, isUpvote), Times.Once);
        _mockUserManager.Verify(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()), Times.Once);
    }
}
