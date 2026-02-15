namespace LibraryManagement.Core.Entities;

public class Loan : Entity
{
    public string BookId { get; private set; }
    public string MemberId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }
    public decimal? LateFee { get; private set; }

    private const int DefaultLoanDurationDays = 14;
    private const decimal LateFeePerDay = 2.00m;

    private Loan() : base() { }

    public Loan(string bookId, string memberId)
        : base()
    {
        if (string.IsNullOrWhiteSpace(bookId))
            throw new ArgumentException("Book ID cannot be empty", nameof(bookId));

        if (string.IsNullOrWhiteSpace(memberId))
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));

        BookId = bookId;
        MemberId = memberId;
        LoanDate = DateTime.UtcNow;
        DueDate = LoanDate.AddDays(DefaultLoanDurationDays);
        Status = LoanStatus.Active;
    }

    public bool IsOverdue()
        => Status == LoanStatus.Active && DateTime.UtcNow > DueDate;

    public int GetDaysOverdue()
    {
        if (!IsOverdue())
            return 0;

        return (DateTime.UtcNow - DueDate).Days;
    }

    public void ReturnBook()
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("Loan is not active");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;

        if (IsOverdue())
        {
            var daysOverdue = (ReturnDate.Value - DueDate).Days;
            LateFee = daysOverdue * LateFeePerDay;
        }

        MarkAsUpdated();
    }

    public void RenewLoan(int additionalDays = DefaultLoanDurationDays)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("Only active loans can be renewed");

        if (IsOverdue())
            throw new InvalidOperationException("Overdue loans cannot be renewed");

        DueDate = DueDate.AddDays(additionalDays);
        MarkAsUpdated();
    }

    public void MarkAsLost()
    {
        if (Status == LoanStatus.Returned)
            throw new InvalidOperationException("Returned books cannot be marked as lost");

        Status = LoanStatus.Lost;
        MarkAsUpdated();
    }

    public decimal CalculateCurrentLateFee()
    {
        if (!IsOverdue() || Status != LoanStatus.Active)
            return 0;

        var daysOverdue = GetDaysOverdue();
        return daysOverdue * LateFeePerDay;
    }
}

public enum LoanStatus
{
    Active,
    Returned,
    Lost
}