using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly IMongoCollection<Loan> _loans;

    public LoanRepository(MongoDbContext context)
    {
        _loans = context.GetCollection<Loan>("Loans");
    }

    public async Task<Loan?> GetByIdAsync(string id)
        => await _loans.Find(l => l.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Loan>> GetAllAsync()
        => await _loans.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        => await _loans.Find(l => l.Status == LoanStatus.Active).ToListAsync();

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var now = DateTime.UtcNow;
        return await _loans.Find(l => l.Status == LoanStatus.Active && l.DueDate < now).ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberAsync(string memberId)
        => await _loans.Find(l => l.MemberId == memberId).ToListAsync();

    public async Task<IEnumerable<Loan>> GetLoansByBookAsync(string bookId)
        => await _loans.Find(l => l.BookId == bookId).ToListAsync();

    public async Task AddAsync(Loan loan)
        => await _loans.InsertOneAsync(loan);

    public async Task UpdateAsync(Loan loan)
        => await _loans.ReplaceOneAsync(l => l.Id == loan.Id, loan);

    public async Task DeleteAsync(string id)
        => await _loans.DeleteOneAsync(l => l.Id == id);
}