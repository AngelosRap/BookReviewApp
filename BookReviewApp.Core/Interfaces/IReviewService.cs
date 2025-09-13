using BookReviewApp.Domain.Models;

namespace BookReviewApp.Core.Interfaces;

public interface IReviewService
{
    public Task<Review> Create(Review review);
    public Task<Review> Get(int id);
    public Task<List<Review>> GetAll();
    public Task<Review> Update(Review review);
    public Task Delete(int id);
    public Task<bool> Vote(string userId, int reviewId, bool isUpvote);
}
