using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Core.Validators;
using BookReviewApp.DataAccess;
using BookReviewApp.DataAccess.Extensions;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Core.Services;

public class ReviewService(Context context) : IReviewService
{
    private readonly Context _context = context;

    public async Task<Result<Review>> Create(Review review)
    {
        var res = ReviewValidator.Validate(review);

        if (res.Failed)
        {
            return Result<Review>.CreateFailed(res.Message);
        }

        var checkBookAndReviewExistance = await CheckBookAndUserExistence(review.BookId, review.UserId!);

        if (checkBookAndReviewExistance.Failed)
        {
            return Result<Review>.CreateFailed(checkBookAndReviewExistance.Message);
        }

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Result<Review>.CreateSuccessful(review, "Review created successfully.");
    }

    public async Task<Result<Review>> Get(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.Votes)
            .FirstOrDefaultAsync(r => r.Id == id);

        return review == null
            ? Result<Review>.CreateFailed($"Review with Id {id} not found.")
            : Result<Review>.CreateSuccessful(review, $"Review with Id {id} found.");
    }

    public async Task<List<Review>> GetByBookId(int bookId)
    {
        return await _context.Reviews
            .Where(r => r.BookId == bookId)
            .Include(r => r.User)
            .Include(r => r.Votes)
            .ToListAsync();
    }

    public async Task<List<Review>> GetAll()
    {
        return await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.Votes)
            .ToListAsync();
    }

    public async Task<Result<Review>> Update(Review review)
    {
        var reviewToUpdate = await _context.Reviews.FindAsync(review.Id);
        if (reviewToUpdate == null)
        {
            return Result<Review>.CreateFailed($"Review with Id {review.Id} not found.");
        }

        var res = ReviewValidator.Validate(review);
        if (res.Failed)
        {
            return Result<Review>.CreateFailed(res.Message);
        }

        var checkBookAndReviewExistance = await CheckBookAndUserExistence(review.BookId, review.UserId!);

        if (checkBookAndReviewExistance.Failed)
        {
            return Result<Review>.CreateFailed(checkBookAndReviewExistance.Message);
        }

        _context.UpdateEntity(reviewToUpdate, review);
        await _context.SaveChangesAsync();

        return Result<Review>.CreateSuccessful(reviewToUpdate, "Review updated successfully.");
    }

    public async Task<Result> Delete(int id)
    {
        var existing = await _context.Reviews.FindAsync(id);
        if (existing == null)
        {
            return Result.CreateFailed($"Review with Id {id} not found.");
        }

        _context.Reviews.Remove(existing);
        await _context.SaveChangesAsync();

        return Result.CreateSuccessful("Review deleted successfully.");
    }

    public async Task<Result> Vote(string userId, int reviewId, bool isUpvote)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return Result.CreateFailed($"User with Id {userId} not found.");
        }

        var review = await _context.Reviews
            .Include(r => r.Votes)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
        {
            return Result.CreateFailed($"Review with Id {reviewId} not found.");
        }

        var existingVote = review.Votes.FirstOrDefault(v => v.UserId == userId);
        if (existingVote != null)
        {
            existingVote.IsUpvote = isUpvote;
        }
        else
        {
            review.Votes.Add(new ReviewVote
            {
                ReviewId = reviewId,
                UserId = userId,
                IsUpvote = isUpvote
            });
        }

        await _context.SaveChangesAsync();
        return Result.CreateSuccessful("Vote registered successfully.");
    }

    private async Task<Result> CheckBookAndUserExistence(int bookId, string userId)
    {
        var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
        if (!bookExists)
        {
            return Result.CreateFailed($"Book with Id {bookId} does not exist.");
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

        return !userExists ? Result.CreateFailed($"User with Id {userId} does not exist.") : Result.CreateSuccessful("User and Book exists.");
    }
}
