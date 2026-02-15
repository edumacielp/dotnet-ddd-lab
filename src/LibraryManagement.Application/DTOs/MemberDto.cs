namespace LibraryManagement.Application.DTOs;

public record MemberDto(
    string Id,
    string Name,
    string Email,
    string PhoneNumber,
    DateTime MembershipDate,
    string Status,
    int BorrowedBooksCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateMemberDto(
    string Name,
    string Email,
    string PhoneNumber
);

public record UpdateMemberDto(
    string PhoneNumber
);
