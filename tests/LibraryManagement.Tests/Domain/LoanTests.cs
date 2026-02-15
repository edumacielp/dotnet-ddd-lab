using LibraryManagement.Core.Entities;

namespace LibraryManagement.Tests.Domain;

public class LoanTests
{
    [Fact]
    public void Loan_Creation_Should_Initialize_Correctly()
    {
        // Arrange & Act
        var loan = new Loan("book-123", "member-456");

        // Assert
        Assert.Equal("book-123", loan.BookId);
        Assert.Equal("member-456", loan.MemberId);
        Assert.Equal(LoanStatus.Active, loan.Status);
        Assert.Null(loan.ReturnDate);
        Assert.Null(loan.LateFee);
        Assert.Equal(14, (loan.DueDate - loan.LoanDate).Days);
    }

    [Fact]
    public void Loan_IsOverdue_Should_Return_False_When_Not_Overdue()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");

        // Act & Assert
        Assert.False(loan.IsOverdue());
    }

    [Fact]
    public void Loan_ReturnBook_Should_Mark_As_Returned()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");

        // Act
        loan.ReturnBook();

        // Assert
        Assert.Equal(LoanStatus.Returned, loan.Status);
        Assert.NotNull(loan.ReturnDate);
        Assert.Null(loan.LateFee); // Not overdue, so no late fee
    }

    [Fact]
    public void Loan_RenewLoan_Should_Extend_Due_Date()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");
        var originalDueDate = loan.DueDate;

        // Act
        loan.RenewLoan(7);

        // Assert
        Assert.Equal(originalDueDate.AddDays(7), loan.DueDate);
        Assert.Equal(LoanStatus.Active, loan.Status);
    }

    [Fact]
    public void Loan_RenewLoan_Should_Throw_When_Not_Active()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");
        loan.ReturnBook();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => loan.RenewLoan());
    }

    [Fact]
    public void Loan_MarkAsLost_Should_Change_Status()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");

        // Act
        loan.MarkAsLost();

        // Assert
        Assert.Equal(LoanStatus.Lost, loan.Status);
    }

    [Fact]
    public void Loan_MarkAsLost_Should_Throw_When_Already_Returned()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");
        loan.ReturnBook();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => loan.MarkAsLost());
    }

    [Fact]
    public void Loan_ReturnBook_Should_Throw_When_Not_Active()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");
        loan.ReturnBook();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => loan.ReturnBook());
    }

    [Fact]
    public void Loan_GetDaysOverdue_Should_Return_Zero_When_Not_Overdue()
    {
        // Arrange
        var loan = new Loan("book-123", "member-456");

        // Act
        var daysOverdue = loan.GetDaysOverdue();

        // Assert
        Assert.Equal(0, daysOverdue);
    }
}
