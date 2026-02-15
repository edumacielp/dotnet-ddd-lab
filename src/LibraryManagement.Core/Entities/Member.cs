using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Core.Entities;

public class Member : Entity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime MembershipDate { get; private set; }
    public MembershipStatus Status { get; private set; }
    private readonly List<string> _borrowedBookIds;
    public IReadOnlyCollection<string> BorrowedBookIds => _borrowedBookIds.AsReadOnly();

    private const int MaxBorrowedBooks = 5;

    private Member() : base()
    {
        _borrowedBookIds = [];
    }

    public Member(string name, Email email, string phoneNumber)
        : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        MembershipDate = DateTime.UtcNow;
        Status = MembershipStatus.Active;
        _borrowedBookIds = [];
    }

    public bool CanBorrowBooks()
        => Status == MembershipStatus.Active && _borrowedBookIds.Count < MaxBorrowedBooks;

    public void BorrowBook(string bookId)
    {
        if (!CanBorrowBooks())
            throw new InvalidOperationException("Member cannot borrow more books");

        if (_borrowedBookIds.Contains(bookId))
            throw new InvalidOperationException("Book already borrowed by this member");

        _borrowedBookIds.Add(bookId);
        MarkAsUpdated();
    }

    public void ReturnBook(string bookId)
    {
        if (!_borrowedBookIds.Contains(bookId))
            throw new InvalidOperationException("Book was not borrowed by this member");

        _borrowedBookIds.Remove(bookId);
        MarkAsUpdated();
    }

    public void Suspend()
    {
        Status = MembershipStatus.Suspended;
        MarkAsUpdated();
    }

    public void Reactivate()
    {
        Status = MembershipStatus.Active;
        MarkAsUpdated();
    }

    public void UpdateContactInfo(string phoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            PhoneNumber = phoneNumber;
            MarkAsUpdated();
        }
    }
}

public enum MembershipStatus
{
    Active,
    Suspended,
    Expired
}