using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response.Review;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Api.Mappings;

public static class ReviewMap
{
	public static ReviewResponse ToResponse(this Review review)
	{
		return new()
		{
			Id = review.Id,
			Content = review.Content,
			Rating = review.Rating,
			DateCreated = review.DateCreated,
			BookId = review.BookId,
			UserId = review.UserId,
			Votes = [.. review.Votes.Select(x => x.ToResponse())],
		};
	}

	public static Review ToReviewEntity(this ReviewCreateRequest reviewCreateRequest, string userId)
	{
		return new()
		{
			Content = reviewCreateRequest.Content,
			Rating = reviewCreateRequest.Rating,
			BookId = reviewCreateRequest.BookId,
			UserId = userId
		};
	}
} 