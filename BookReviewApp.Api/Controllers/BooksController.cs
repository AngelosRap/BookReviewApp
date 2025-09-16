using BookReviewApp.Api.Mappings;
using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response.Book;
using BookReviewApp.Api.Models.Response.Review;
using BookReviewApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookReviewApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
public class BooksController(IBookService bookService, IReviewService reviewService) : ControllerBase
{
    private readonly IBookService _bookService = bookService;
    private readonly IReviewService _reviewService = reviewService;

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all books",
        Description = "Retrieve a list of books, optionally filtered by author, genre, or year."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "List of books", typeof(IEnumerable<BookListResponse>))]
    public async Task<IActionResult> GetAll([FromQuery] string? author = null, string? genre = null, int? year = null)
    {
        var books = await _bookService.GetAll(author, genre, year);
        var bookListResponse = books.Select(x => x.ToListResponse()).ToList();

        return Ok(bookListResponse);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get book details by Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Book details", typeof(BookDetailResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Book not found")]
    public async Task<IActionResult> Get(int id)
    {
        var bookRes = await _bookService.Get(id);

        if (bookRes.Failed)
        {
            return NotFound(bookRes.Message);
        }

        var bookDetailResponse = bookRes.Data!.ToDetailResponse();

        return Ok(bookDetailResponse);
    }

    [HttpGet("{bookId}/reviews")]
    [SwaggerOperation(Summary = "Get reviews for a book")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of reviews", typeof(IEnumerable<ReviewResponse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Book not found")]
    public async Task<IActionResult> GetReviews(int bookId)
    {
        var bookRes = await _bookService.Get(bookId);

        if (bookRes.Data is null)
        {
            return NotFound(bookRes.Message);
        }

        var reviews = await _reviewService.GetByBookId(bookId);

        return Ok(reviews.Select(x => x.ToResponse()));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new book")]
    [SwaggerResponse(StatusCodes.Status201Created, "Book created", typeof(BookDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> Create([FromBody] BookCreateRequest bookCreateRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = bookCreateRequest.ToBookEntity();

        var bookRes = await _bookService.Create(book);

        if (bookRes.Failed)
        {
            return BadRequest(bookRes.Message);
        }

        var response = bookRes.Data!.ToDetailResponse();

        return CreatedAtAction(
            nameof(Get),
            new { id = book.Id },
            response
        );
    }
}
