using BookReviewApp.Core.Interfaces;
using BookReviewApp.DataAccess;
using BookReviewApp.DataAccess.Extensions;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Core.Services;

public class ReviewService(Context context) : IReviewService
{
    private readonly Context _context = context;

    public async Task<Review> Create(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return review;
    }

    public async Task<Review> Get(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.Votes)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
            throw new KeyNotFoundException($"Review with Id {id} not found.");

        return review;
    }

    public async Task<List<Review>> GetAll()
    {
        return await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.Votes)
            .ToListAsync();
    }

    public async Task<Review> Update(Review review)
    {
        var reviewToUpdate = await _context.Reviews.FindAsync(review.Id);

        if (reviewToUpdate is null)
        {
            throw new KeyNotFoundException($"Review with Id {review.Id} not found.");
        }

        _context.UpdateEntity(reviewToUpdate, review);
        await _context.SaveChangesAsync();
        return reviewToUpdate;
    }

    public async Task Delete(int id)
    {
        var existing = await _context.Reviews.FindAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Review with Id {id} not found.");

        _context.Reviews.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Vote(string userId, int reviewId, bool isUpvote)
    {
        var review = await _context.Reviews
            .Include(r => r.Votes)
            .FirstOrDefaultAsync(r => r.Id == reviewId) ?? throw new KeyNotFoundException($"Review with Id {reviewId} not found.");

        var existingVote = review.Votes.FirstOrDefault(v => v.UserId == userId);

        if (existingVote != null)
        {
            // Update the vote if it already exists
            existingVote.IsUpvote = isUpvote;
        }
        else
        {
            // Create a new vote
            review.Votes.Add(new ReviewVote
            {
                ReviewId = reviewId,
                UserId = userId,
                IsUpvote = isUpvote
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
