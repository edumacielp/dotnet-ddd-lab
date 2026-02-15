namespace LibraryManagement.Application.DTOs;

public record BookDto(
    string Id,
    string Title,
    string Author,
    string ISBN,
    int PublicationYear,
    string Category,
    int TotalCopies,
    int AvailableCopies,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateBookDto(
    string Title,
    string Author,
    string ISBN,
    int PublicationYear,
    string Category,
    int TotalCopies
);

public record UpdateBookDto(
    string Title,
    string Author,
    int PublicationYear,
    string Category
);
