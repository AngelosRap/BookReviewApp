using BookReviewApp.Core.Interfaces;
using BookReviewApp.Web.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Web.Controllers;
public class BooksController(IBookService bookService) : Controller
{
    private readonly IBookService _bookService = bookService;

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAll();

        var viewModels = books.Select(b => b.ToViewModel()).ToList();
        return View(viewModels);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {

        return View();
    }

    // GET: Books/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.Get(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book); // confirmation page (optional)
    }

    // POST: Books/Delete/5
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _bookService.Get(id);
        if (book != null)
        {
            await _bookService.Delete(id);
        }

        // ✅ Redirects to Index
        return RedirectToAction(nameof(Index));
    }
}
