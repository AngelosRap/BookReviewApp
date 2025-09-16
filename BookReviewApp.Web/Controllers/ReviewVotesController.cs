using BookReviewApp.Core.Interfaces;
using BookReviewApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Web.Controllers;

[Authorize]
public class ReviewVotesController(IReviewService reviewService, UserManager<AppUser> userManager) : Controller
{
    private readonly IReviewService _reviewService = reviewService;
    private readonly UserManager<AppUser> _userManager = userManager;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int reviewId, int bookId, bool isUpvote)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Unauthorized();
        }

        var reviewRes = await _reviewService.Get(reviewId);

        if (reviewRes.Failed)
        {
            return NotFound($"Review {reviewId} not found.");
        }

        await _reviewService.Vote(userId, reviewId, isUpvote);

        return RedirectToAction("Details", "Books", new { id = bookId });
    }
}
