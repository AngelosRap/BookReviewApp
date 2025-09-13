using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Models.Review;

namespace BookReviewApp.Web.Mappings;

public static class ReviewMap
{
    public static ReviewDisplayViewModel ToViewModel(this Review review)
    {
        return new ReviewDisplayViewModel
        {
            Id = review.Id,
            Content = review.Content,
            Rating = review.Rating,
            UserName = review.User?.UserName ?? string.Empty,
            Votes = review.Votes
        };
    }

    public static Review ToEntity(this ReviewCreateViewModel reviewCreateViewModel, string userId, int bookId)
    {
        return new Review
        {
            Content = reviewCreateViewModel.Content,
            Rating = reviewCreateViewModel.Rating,
            DateCreated = DateTime.UtcNow,
            UserId = userId,
            BookId = bookId
        };
    }

    public static Review ToEntity(this ReviewDisplayViewModel reviewViewModel, string userId, int bookId)
    {
        return new Review
        {
            Content = reviewViewModel.Content,
            Rating = reviewViewModel.Rating,
            DateCreated = DateTime.UtcNow,
            UserId = userId,
            BookId = bookId
        };
    }
}
