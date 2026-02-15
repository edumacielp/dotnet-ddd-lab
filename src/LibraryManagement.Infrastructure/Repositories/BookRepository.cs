using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IMongoCollection<Book> _books;

    public BookRepository(MongoDbContext context)
    {
        _books = context.GetCollection<Book>("Books");
    }

    public async Task<Book?> GetByIdAsync(string id)
        => await _books.Find(b => b.Id == id).FirstOrDefaultAsync();

    public async Task<Book?> GetByISBNAsync(string isbn)
        => await _books.Find(b => b.ISBN.Value == isbn).FirstOrDefaultAsync();

    public async Task<IEnumerable<Book>> GetAllAsync()
        => await _books.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string title)
    {
        var filter = Builders<Book>.Filter.Regex(b => 
            b.Title, new BsonRegularExpression(title, "i")
        );
        return await _books.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchByAuthorAsync(string author)
    {
        var filter = Builders<Book>.Filter.Regex(b => 
            b.Author, new BsonRegularExpression(author, "i")
        );
        return await _books.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(string category)
        => await _books.Find(b => b.Category == category).ToListAsync();

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        => await _books.Find(b => b.AvailableCopies > 0).ToListAsync();

    public async Task AddAsync(Book book)
        => await _books.InsertOneAsync(book);

    public async Task UpdateAsync(Book book)
        => await _books.ReplaceOneAsync(b => b.Id == book.Id, book);

    public async Task DeleteAsync(string id)
        => await _books.DeleteOneAsync(b => b.Id == id);
}