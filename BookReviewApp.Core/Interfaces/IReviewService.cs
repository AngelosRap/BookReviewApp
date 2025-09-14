using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Core.Interfaces;

public interface IReviewService
{
    public Task<Result<Review>> Create(Review review);
    public Task<Result<Review>> Get(int id);
    public Task<List<Review>> GetByBookId(int bookId);
    public Task<List<Review>> GetAll();
    public Task<Result<Review>> Update(Review review);
    public Task<Result> Delete(int id);
    public Task<Result> Vote(string userId, int reviewId, bool isUpvote);
}
