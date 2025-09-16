using BookReviewApp.Core.Interfaces;
using BookReviewApp.Web.Mappings;
using BookReviewApp.Web.Models.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Web.Controllers;

[Authorize]
public class BooksController(IBookService bookService) : Controller
{
    private readonly IBookService _bookService = bookService;

    public async Task<IActionResult> Index(string? genre, int? year, double? minRating)
    {
        var books = await _bookService.GetAll(author: null, genre: genre, year: year, withDetails: true);

        if (minRating.HasValue)
        {
            books = books
                .Where(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) >= minRating.Value : false)
                .ToList();
        }

        var viewModels = books.Select(b => b.ToViewModel()).ToList();
        return View(viewModels);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel bookCreateViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(bookCreateViewModel);
        }

        var book = bookCreateViewModel.ToEntity();

        await _bookService.Create(book);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var res = await _bookService.Get(id);

        if (res.Failed)
        {
            return NotFound(res.Message);
        }

        var bookEditViewModel = res.Data!.ToEditViewModel();

        return View(bookEditViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookEditViewModel bookEditViewModel)
    {
        var res = await _bookService.Get(id);

        if (res.Failed)
        {
            return NotFound(res.Message);
        }

        if (!ModelState.IsValid)
        {
            return View(bookEditViewModel);
        }

        res.Data!.UpdateWithEditViewModel(bookEditViewModel);

        var updateRes = await _bookService.Update(res.Data!);

        return updateRes.Failed ? BadRequest(updateRes.Message) : RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var res = await _bookService.Get(id);

        if (res.Failed)
        {
            return NotFound(res.Message);
        }

        var viewModel = res.Data!.ToViewModel();

        return View(viewModel);
    }
}
