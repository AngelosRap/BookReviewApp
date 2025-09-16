using BookReviewApp.Api.Controllers;
using BookReviewApp.Api.Mappings;
using BookReviewApp.Api.Models.Request;
using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BookReviewApp.Tests.Api.Controllers
{
    public class ReviewsControllerTests
    {
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly ReviewsController _controller;

        private const string UserId = "user123";

        public ReviewsControllerTests()
        {
            _mockReviewService = new Mock<IReviewService>();
            _controller = new ReviewsController(_mockReviewService.Object);

            // Mock User with ClaimTypes.NameIdentifier
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            var request = new ReviewCreateRequest();
            _controller.ModelState.AddModelError("Rating", "Required");

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUserIdIsNull()
        {
            // Arrange
            var request = new ReviewCreateRequest();
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No claims

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("User ID not found.");
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenValid()
        {
            // Arrange
            var request = new ReviewCreateRequest { Rating = 5, Content = "Great book!" };
            var reviewEntity = request.ToReviewEntity(UserId);
            _mockReviewService.Setup(s => s.Create(It.IsAny<Review>()))
                .ReturnsAsync(Result<Review>.CreateSuccessful(reviewEntity, "Created successfully."));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(ReviewsController.Get));
            createdResult.RouteValues!["id"].Should().Be(reviewEntity.Id);

            _mockReviewService.Verify(s => s.Create(It.Is<Review>(
                r => r.UserId == UserId && r.Rating == request.Rating)), Times.Once);
        }

        [Fact]
        public async Task Vote_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            var voteRequest = new ReviewVoteRequest();
            _controller.ModelState.AddModelError("dummy", "error");

            // Act
            var result = await _controller.Vote(1, voteRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Vote_ShouldReturnBadRequest_WhenUserIdIsNull()
        {
            // Arrange
            var voteRequest = new ReviewVoteRequest();
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No claims

            // Act
            var result = await _controller.Vote(1, voteRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("User ID not found.");
        }

        [Fact]
        public async Task Vote_ShouldReturnBadRequest_WhenVoteFails()
        {
            // Arrange
            var reviewId = 1;
            var voteRequest = new ReviewVoteRequest { IsUpvote = true };
            _mockReviewService.Setup(s => s.Get(reviewId))
                .ReturnsAsync(Result<Review>.CreateSuccessful(It.IsAny<Review>(), "Review found"));
            _mockReviewService.Setup(s => s.Vote(UserId, 1, true))
                .ReturnsAsync(Result.CreateFailed("Vote failed"));

            // Act
            var result = await _controller.Vote(1, voteRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Vote failed" });
        }

        [Fact]
        public async Task Vote_ShouldReturnNotFound_WhenReviewMissing()
        {
            // Arrange
            var voteRequest = new ReviewVoteRequest { IsUpvote = true };
            _mockReviewService.Setup(s => s.Get(1))
                .ReturnsAsync(Result<Review>.CreateFailed("Review not found"));

            // Act
            var result = await _controller.Vote(1, voteRequest);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("Review not found");
        }

        [Fact]
        public async Task Vote_ShouldReturnOk_WhenVoteSucceeds()
        {
            // Arrange
            var voteRequest = new ReviewVoteRequest { IsUpvote = true };
            var reviewEntity = new Review { Id = 1, UserId = UserId, Rating = 5 };
            _mockReviewService.Setup(s => s.Vote(UserId, 1, true))
                .ReturnsAsync(Result.CreateSuccessful("Vote created successfully."));
            _mockReviewService.Setup(s => s.Get(1))
                .ReturnsAsync(Result<Review>.CreateSuccessful(reviewEntity, "Review found."));

            // Act
            var result = await _controller.Vote(1, voteRequest);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(reviewEntity.ToResponse());
        }
    }
}