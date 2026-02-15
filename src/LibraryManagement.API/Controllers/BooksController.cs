using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(string id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpGet("search/title/{title}")]
    public async Task<ActionResult<IEnumerable<BookDto>>> SearchByTitle(string title)
    {
        var books = await _bookService.SearchByTitleAsync(title);
        return Ok(books);
    }

    [HttpGet("search/author/{author}")]
    public async Task<ActionResult<IEnumerable<BookDto>>> SearchByAuthor(string author)
    {
        var books = await _bookService.SearchByAuthorAsync(author);
        return Ok(books);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetByCategory(string category)
    {
        var books = await _bookService.GetByCategoryAsync(category);
        return Ok(books);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAvailable()
    {
        var books = await _bookService.GetAvailableBooksAsync();
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
    {
        try
        {
            var book = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> Update(string id, [FromBody] UpdateBookDto dto)
    {
        try
        {
            var book = await _bookService.UpdateAsync(id, dto);
            return Ok(book);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/add-copies")]
    public async Task<ActionResult<BookDto>> AddCopies(string id, [FromBody] int quantity)
    {
        try
        {
            var book = await _bookService.AddCopiesAsync(id, quantity);
            return Ok(book);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _bookService.DeleteAsync(id);
        return NoContent();
    }
}