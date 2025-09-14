using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;

namespace BookReviewApp.Core.Interfaces;

public interface IBookService
{
    public Task<Result<Book>> Create(Book book);
    public Task<Result<Book>> Get(int id);
    public Task<List<Book>> GetAll(string? author = null, string? genre = null, int? year = null, bool? withDetails = false);
    public Task<Result<Book>> Update(Book book);
    public Task<Result> Delete(int id);
}
