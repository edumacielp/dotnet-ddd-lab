namespace LibraryManagement.Application.DTOs;

public record LoanDto(
    string Id,
    string BookId,
    string MemberId,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status,
    decimal? LateFee,
    int DaysOverdue,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateLoanDto(
    string BookId,
    string MemberId
);
