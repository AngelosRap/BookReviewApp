using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Core.Validators;
using BookReviewApp.DataAccess;
using BookReviewApp.DataAccess.Extensions;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Core.Services;

public class BookService(Context context) : IBookService
{
    private readonly Context _context = context;

    public async Task<Result<Book>> Create(Book book)
    {
        var res = BookValidator.Validate(book);

        if (res.Failed)
        {
            return Result<Book>.CreateFailed(res.Message);
        }

        var exists = await _context.Books.AnyAsync(b => b.Title == book.Title && b.Author == book.Author);
        if (exists)
        {
            return Result<Book>.CreateFailed("A book with the same title and author already exists");
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return Result<Book>.CreateSuccessful(book, "Book created successfully.");
    }

    public async Task<Result<Book>> Get(int id)
    {
        var book = await _context.Books
        .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
        .Include(b => b.Reviews)
            .ThenInclude(r => r.Votes)
        .FirstOrDefaultAsync(b => b.Id == id);

        return book == null
            ? Result<Book>.CreateFailed($"Book with id {id} was not found.")
            : Result<Book>.CreateSuccessful(book, $"Book with id {id} was found.");
    }

    public async Task<List<Book>> GetAll(string? author = null, string? genre = null, int? year = null, bool? withDetails = false)
    {
        IQueryable<Book> query = _context.Books;

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }

        if (year.HasValue)
        {
            query = query.Where(b => b.PublishedYear == year.Value);
        }

        return withDetails.HasValue && withDetails.Value
            ? await query
                .Include(b => b.Reviews)
                .ToListAsync()
            : await query.ToListAsync();
    }

    public async Task<Result<Book>> Update(Book book)
    {
        var bookToUpdate = await _context.Books.FindAsync(book.Id);

        if (bookToUpdate is null)
        {
            return Result<Book>.CreateFailed($"Book with id {book.Id} was not found.");
        }

        var res = BookValidator.Validate(book);

        if (res.Failed)
        {
            return Result<Book>.CreateFailed(res.Message);
        }

        _context.UpdateEntity(bookToUpdate, book);
        await _context.SaveChangesAsync();

        return Result<Book>.CreateSuccessful(bookToUpdate, "Book updated successfully");
    }

    public async Task<Result> Delete(int id)
    {
        var existing = await _context.Books.FindAsync(id);
        if (existing == null)
        {
            return Result.CreateFailed($"Book with Id {id} not found.");
        }

        _context.Books.Remove(existing);
        await _context.SaveChangesAsync();

        return Result.CreateSuccessful("Book deleted successfully.");
    }
}
