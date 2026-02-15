using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Repositories;
using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto?> GetByIdAsync(string id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : MapToDto(book);
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> SearchByTitleAsync(string title)
    {
        var books = await _bookRepository.SearchByTitleAsync(title);
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> SearchByAuthorAsync(string author)
    {
        var books = await _bookRepository.SearchByAuthorAsync(author);
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetByCategoryAsync(string category)
    {
        var books = await _bookRepository.GetByCategoryAsync(category);
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _bookRepository.GetAvailableBooksAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        var existingBook = await _bookRepository.GetByISBNAsync(dto.ISBN);
        if (existingBook != null)
            throw new InvalidOperationException("A book with this ISBN already exists");

        var isbn = new ISBN(dto.ISBN);
        var book = new Book(
            dto.Title, dto.Author, isbn, dto.PublicationYear, dto.Category, dto.TotalCopies
        );

        await _bookRepository.AddAsync(book);
        return MapToDto(book);
    }

    public async Task<BookDto> UpdateAsync(string id, UpdateBookDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException("Book not found");

        book.UpdateDetails(dto.Title, dto.Author, dto.PublicationYear, dto.Category);
        await _bookRepository.UpdateAsync(book);

        return MapToDto(book);
    }

    public async Task<BookDto> AddCopiesAsync(string id, int quantity)
    {
        var book = await _bookRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException("Book not found");
        
        book.AddCopies(quantity);
        await _bookRepository.UpdateAsync(book);

        return MapToDto(book);
    }

    public async Task DeleteAsync(string id)
        => await _bookRepository.DeleteAsync(id);

    private static BookDto MapToDto(Book book)
        => new(
            book.Id,
            book.Title,
            book.Author,
            book.ISBN.Value,
            book.PublicationYear,
            book.Category,
            book.TotalCopies,
            book.AvailableCopies,
            book.CreatedAt,
            book.UpdatedAt
        );
}