using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Core.Entities;

public class Book : Entity
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public ISBN ISBN { get; private set; }
    public int PublicationYear { get; private set; }
    public string Category { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }

    private Book(): base() { }

    public Book(
        string title, string author, ISBN isbn, 
        int publicationYear, string category, int totalCopies)
        : base()
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));

        if (publicationYear < 1000 || publicationYear > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid publication year", nameof(publicationYear));

        if (totalCopies < 1)
            throw new ArgumentException("Total copies must be at least 1", nameof(totalCopies));

        Title = title;
        Author = author;
        ISBN = isbn;
        PublicationYear = publicationYear;
        Category = category;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
    }

    public void AddCopies(int quantity)
    {
        if (quantity < 1)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        TotalCopies += quantity;
        AvailableCopies += quantity;
        MarkAsUpdated();
    }

    public bool CanBeBorrowed()
        => AvailableCopies > 0;

    public void BorrowCopy()
    {
        if (!CanBeBorrowed())
            throw new InvalidOperationException("No copies available for borrowing");

        AvailableCopies--;
        MarkAsUpdated();
    }

    public void ReturnCopy()
    {
        if (AvailableCopies >= TotalCopies)
            throw new InvalidOperationException("All copies are already returned");

        AvailableCopies++;
        MarkAsUpdated();
    }

    public void UpdateDetails(
        string title, string author, int publicationYear, string category)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        if (!string.IsNullOrWhiteSpace(author))
            Author = author; ;

        if (publicationYear >= 1000 && publicationYear <= DateTime.UtcNow.Year + 1)
            PublicationYear = publicationYear;

        if (!string.IsNullOrWhiteSpace(category))
            Category = category;

        MarkAsUpdated();
    }
}