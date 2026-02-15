using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;

    public LoanService(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
    }

    public async Task<LoanDto?> GetByIdAsync(string id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetAllAsync()
    {
        var loans = await _loanRepository.GetAllAsync();
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
    {
        var loans = await _loanRepository.GetActiveLoansAsync();
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
    {
        var loans = await _loanRepository.GetOverdueLoansAsync();
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByMemberAsync(string memberId)
    {
        var loans = await _loanRepository.GetLoansByMemberAsync(memberId);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByBookAsync(string bookId)
    {
        var loans = await _loanRepository.GetLoansByBookAsync(bookId);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateAsync(CreateLoanDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(dto.BookId) 
            ?? throw new InvalidOperationException("Book not found");

        var member = await _memberRepository.GetByIdAsync(dto.MemberId)
            ?? throw new InvalidOperationException("Member not found");

        if (!book.CanBeBorrowed())
            throw new InvalidOperationException("No copies available for borrowing");

        if (!member.CanBorrowBooks())
            throw new InvalidOperationException("Member cannot borrow more books");

        var loan = new Loan(dto.BookId, dto.MemberId);
        
        book.BorrowCopy();
        member.BorrowBook(dto.BookId);

        await _loanRepository.AddAsync(loan);
        await _bookRepository.UpdateAsync(book);
        await _memberRepository.UpdateAsync(member);

        return MapToDto(loan);
    }

    public async Task<LoanDto> ReturnBookAsync(string loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId) 
            ?? throw new InvalidOperationException("Loan not found");

        var book = await _bookRepository.GetByIdAsync(loan.BookId) 
            ?? throw new InvalidOperationException("Book not found");

        var member = await _memberRepository.GetByIdAsync(loan.MemberId) 
            ?? throw new InvalidOperationException("Member not found");

        loan.ReturnBook();
        book.ReturnCopy();
        member.ReturnBook(loan.BookId);

        await _loanRepository.UpdateAsync(loan);
        await _bookRepository.UpdateAsync(book);
        await _memberRepository.UpdateAsync(member);

        return MapToDto(loan);
    }

    public async Task<LoanDto> RenewLoanAsync(string loanId, int additionalDays = 14)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId) 
            ?? throw new InvalidOperationException("Loan not found");

        loan.RenewLoan(additionalDays);
        await _loanRepository.UpdateAsync(loan);

        return MapToDto(loan);
    }

    private static LoanDto MapToDto(Loan loan)
        => new(
            loan.Id,
            loan.BookId,
            loan.MemberId,
            loan.LoanDate,
            loan.DueDate,
            loan.ReturnDate,
            loan.Status.ToString(),
            loan.LateFee,
            loan.GetDaysOverdue(),
            loan.CreatedAt,
            loan.UpdatedAt
        );
}