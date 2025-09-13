using BookReviewApp.Core.Interfaces;
using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Mappings;
using BookReviewApp.Web.Models.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Web.Controllers;

[Authorize]
public class ReviewsController(IReviewService reviewService, UserManager<AppUser> userManager) : Controller
{
    private readonly IReviewService _reviewService = reviewService;
    private readonly UserManager<AppUser> _userManager = userManager;

    public IActionResult Index()
    {
        return View();
    }

    // GET: Reviews/Create?bookId=5
    public IActionResult Create(int bookId)
    {
        ViewBag.BookId = bookId;
        return View();
    }

    // POST: Reviews/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewCreateViewModel vm, int bookId)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return NotFound();
        }

        var review = vm.ToEntity(userId, bookId);

        await _reviewService.Create(review);
        return RedirectToAction("Details", "Books", new { id = bookId });
    }
}
