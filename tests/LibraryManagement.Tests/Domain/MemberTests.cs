using LibraryManagement.Core.Entities;
using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Tests.Domain;

public class MemberTests
{
    [Fact]
    public void Member_Creation_Should_Initialize_Correctly()
    {
        // Arrange & Act
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");

        // Assert
        Assert.Equal("John Doe", member.Name);
        Assert.Equal("john@example.com", member.Email.Value);
        Assert.Equal(MembershipStatus.Active, member.Status);
        Assert.Empty(member.BorrowedBookIds);
        Assert.True(member.CanBorrowBooks());
    }

    [Fact]
    public void Member_BorrowBook_Should_Add_To_Borrowed_List()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");
        var bookId = "book-123";

        // Act
        member.BorrowBook(bookId);

        // Assert
        Assert.Single(member.BorrowedBookIds);
        Assert.Contains(bookId, member.BorrowedBookIds);
    }

    [Fact]
    public void Member_BorrowBook_Should_Throw_When_Limit_Reached()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");

        // Act - Borrow 5 books (max limit)
        for (int i = 0; i < 5; i++)
        {
            member.BorrowBook($"book-{i}");
        }

        // Assert
        Assert.Throws<InvalidOperationException>(() => member.BorrowBook("book-6"));
    }

    [Fact]
    public void Member_ReturnBook_Should_Remove_From_Borrowed_List()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");
        var bookId = "book-123";
        member.BorrowBook(bookId);

        // Act
        member.ReturnBook(bookId);

        // Assert
        Assert.Empty(member.BorrowedBookIds);
    }

    [Fact]
    public void Member_Suspend_Should_Change_Status()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");

        // Act
        member.Suspend();

        // Assert
        Assert.Equal(MembershipStatus.Suspended, member.Status);
        Assert.False(member.CanBorrowBooks());
    }

    [Fact]
    public void Member_Reactivate_Should_Change_Status()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");
        member.Suspend();

        // Act
        member.Reactivate();

        // Assert
        Assert.Equal(MembershipStatus.Active, member.Status);
        Assert.True(member.CanBorrowBooks());
    }

    [Fact]
    public void Member_BorrowBook_Should_Throw_When_Already_Borrowed()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");
        var bookId = "book-123";
        member.BorrowBook(bookId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => member.BorrowBook(bookId));
    }

    [Fact]
    public void Member_ReturnBook_Should_Throw_When_Not_Borrowed()
    {
        // Arrange
        var email = new Email("john@example.com");
        var member = new Member("John Doe", email, "123-456-7890");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => member.ReturnBook("book-123"));
    }
}
