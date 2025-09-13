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

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAll();

        var viewModels = books.Select(b => b.ToViewModel()).ToList();
        return View(viewModels);
    }

    // GET: Books/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Books/Create
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

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.Get(id);

        if (book is null)
        {
            return NotFound();
        }

        var bookEditViewModel = book.ToEditViewModel();
        return View(bookEditViewModel);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookEditViewModel bookEditViewModel)
    {
        var existingBookToUpdate = await _bookService.Get(id);

        if (existingBookToUpdate is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(bookEditViewModel);
        }

        existingBookToUpdate.UpdateWithEditViewModel(bookEditViewModel);

        await _bookService.Update(existingBookToUpdate);
        return RedirectToAction(nameof(Index));
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.Get(id);

        if (book == null)
        {
            return NotFound();
        }

        var viewModel = book.ToViewModel();

        return View(viewModel);
    }
}
