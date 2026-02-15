using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Repositories;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(string id);
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetLoansByMemberAsync(string memberId);
    Task<IEnumerable<Loan>> GetLoansByBookAsync(string bookId);
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task DeleteAsync(string id);
}