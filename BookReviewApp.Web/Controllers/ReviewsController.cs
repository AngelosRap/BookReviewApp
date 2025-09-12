using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Web.Controllers;
public class ReviewsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
