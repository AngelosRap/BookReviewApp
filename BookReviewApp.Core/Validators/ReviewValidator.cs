using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Core.Validators;

internal static class ReviewValidator
{
    public static Result<Review> Validate(Review review)
    {
        if (review == null)
        {
            return Result<Review>.CreateFailed("Review cannot be null");
        }

        if (string.IsNullOrWhiteSpace(review.Content))
        {
            return Result<Review>.CreateFailed("Review content is required");
        }

        if (review.Content.Length > 1000)
        {
            return Result<Review>.CreateFailed("Review content cannot exceed 1000 characters");
        }

        if (review.Rating < 1 || review.Rating > 5)
        {
            return Result<Review>.CreateFailed("Rating must be between 1 and 5");
        }

        if (review.BookId <= 0)
        {
            return Result<Review>.CreateFailed("Invalid BookId for the review");
        }

        return Result<Review>.CreateSuccessful(review, "Review is validated.");
    }
}
