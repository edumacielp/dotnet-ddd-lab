using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(string id);
    Task<Book?> GetByISBNAsync(string isbn);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<IEnumerable<Book>> SearchByTitleAsync(string title);
    Task<IEnumerable<Book>> SearchByAuthorAsync(string author);
    Task<IEnumerable<Book>> GetByCategoryAsync(string category);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(string id);
}