using BookReviewApp.Core.Interfaces;
using BookReviewApp.DataAccess;
using BookReviewApp.DataAccess.Extensions;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Core.Services;

public class BookService(Context context) : IBookService
{
    private readonly Context _context = context;

    public async Task<Book> Create(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book> Get(int id)
    {
        var book = await _context.Books
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == id);

        return book ?? throw new KeyNotFoundException($"Book with Id {id} not found.");
    }
    public async Task<List<Book>> GetAll()
    {
        return await _context.Books
            .Include(b => b.Reviews)
            .ToListAsync();
    }

    public async Task<Book> Update(Book book)
    {
        var bookToUpdate = await _context.Books.FindAsync(book.Id);

        if (bookToUpdate is null)
        {
            throw new KeyNotFoundException($"Book with Id {book.Id} not found.");
        }

        _context.UpdateEntity(bookToUpdate, book);
        await _context.SaveChangesAsync();
        return bookToUpdate;
    }

    public async Task Delete(int id)
    {
        var existing = await _context.Books.FindAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Book with Id {id} not found.");

        _context.Books.Remove(existing);
        await _context.SaveChangesAsync();
    }
}
