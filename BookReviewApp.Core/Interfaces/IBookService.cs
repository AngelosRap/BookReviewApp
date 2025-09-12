using BookReviewApp.Domain.Models;

namespace BookReviewApp.Core.Interfaces;

public interface IBookService
{
    public Task<Book> Create(Book book);
    public Task<Book> Get(int id);
    public Task<List<Book>> GetAll();
    public Task<Book> Update(Book book);
    public Task Delete(int id);
}
