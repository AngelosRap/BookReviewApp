using BookReviewApp.Api.Mappings;
using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response.Review;
using BookReviewApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace BookReviewApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
	private readonly IReviewService _reviewService = reviewService;

	[HttpPost]
	[SwaggerOperation(Summary = "Add a new review", Description = "Create a new review for a book.")]
	[SwaggerResponse(StatusCodes.Status201Created, "Review created successfully", typeof(ReviewResponse))]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed or user/book does not exist")]
	public async Task<IActionResult> Create([FromBody] ReviewCreateRequest reviewCreateRequest)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
		{
			return BadRequest("User ID not found.");
		}

		var review = reviewCreateRequest.ToReviewEntity(userId);
		var res = await _reviewService.Create(review);

		if (res.Failed)
		{
			return BadRequest(res.Message);
		}

		var response = res.Data!.ToResponse();
		return CreatedAtAction(nameof(Get), new { id = review.Id }, response);
	}

	[HttpPost("{id}/vote")]
	[SwaggerOperation(Summary = "Upvote or downvote a review", Description = "Registers a vote for a review.")]
	[SwaggerResponse(StatusCodes.Status200OK, "Vote registered successfully", typeof(ReviewResponse))]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "Vote failed or user/review does not exist")]
	public async Task<IActionResult> Vote(int id, [FromBody] ReviewVoteRequest reviewVote)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
		{
			return BadRequest("User ID not found.");
		}

		var reviewResult = await _reviewService.Get(id);
		if (reviewResult.Failed)
		{
			return NotFound(reviewResult.Message);
		}

		var voteResult = await _reviewService.Vote(userId, id, reviewVote.IsUpvote);
		if (!voteResult.Success)
		{
			return BadRequest(new { message = voteResult.Message });
		}

		var updatedReview = await _reviewService.Get(id);
		var reviewResponse = updatedReview.Data!.ToResponse();
		return Ok(reviewResponse);
	}

	[HttpGet("{id}")]
	[SwaggerOperation(Summary = "Get a review by id", Description = "Retrieve details of a single review by its ID.")]
	[SwaggerResponse(StatusCodes.Status200OK, "Review found", typeof(ReviewResponse))]
	[SwaggerResponse(StatusCodes.Status404NotFound, "Review not found")]
	public async Task<IActionResult> Get(int id)
	{
		var res = await _reviewService.Get(id);
		if (res.Failed)
		{
			return NotFound(res.Message);
		}

		var response = res.Data!.ToResponse();
		return Ok(response);
	}
} 