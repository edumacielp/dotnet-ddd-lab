using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetByIdAsync(string id);
    Task<IEnumerable<BookDto>> GetAllAsync();
    Task<IEnumerable<BookDto>> SearchByTitleAsync(string title);
    Task<IEnumerable<BookDto>> SearchByAuthorAsync(string author);
    Task<IEnumerable<BookDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task<BookDto> UpdateAsync(string id, UpdateBookDto dto);
    Task<BookDto> AddCopiesAsync(string id, int quantity);
    Task DeleteAsync(string id);
}

public interface IMemberService
{
    Task<MemberDto?> GetByIdAsync(string id);
    Task<IEnumerable<MemberDto>> GetAllAsync();
    Task<IEnumerable<MemberDto>> GetActiveMembersAsync();
    Task<IEnumerable<MemberDto>> SearchByNameAsync(string name);
    Task<MemberDto> CreateAsync(CreateMemberDto dto);
    Task<MemberDto> UpdateAsync(string id, UpdateMemberDto dto);
    Task<MemberDto> SuspendAsync(string id);
    Task<MemberDto> ReactivateAsync(string id);
    Task DeleteAsync(string id);
}

public interface ILoanService
{
    Task<LoanDto?> GetByIdAsync(string id);
    Task<IEnumerable<LoanDto>> GetAllAsync();
    Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
    Task<IEnumerable<LoanDto>> GetLoansByMemberAsync(string memberId);
    Task<IEnumerable<LoanDto>> GetLoansByBookAsync(string bookId);
    Task<LoanDto> CreateAsync(CreateLoanDto dto);
    Task<LoanDto> ReturnBookAsync(string loanId);
    Task<LoanDto> RenewLoanAsync(string loanId, int additionalDays = 14);
}
