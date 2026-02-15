using LibraryManagement.Core.Entities;
using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Tests.Domain;

public class BookTests
{
    [Fact]
    public void Book_Creation_Should_Initialize_Correctly()
    {
        // Arrange & Act
        var isbn = new ISBN("978-0132350884");
        var book = new Book("Clean Code", "Robert Martin", isbn, 2008, "Programming", 5);

        // Assert
        Assert.Equal("Clean Code", book.Title);
        Assert.Equal("Robert Martin", book.Author);
        Assert.Equal(5, book.TotalCopies);
        Assert.Equal(5, book.AvailableCopies);
        Assert.True(book.CanBeBorrowed());
    }

    [Fact]
    public void Book_BorrowCopy_Should_Decrease_Available_Copies()
    {
        // Arrange
        var isbn = new ISBN("978-0132350884");
        var book = new Book("Clean Code", "Robert Martin", isbn, 2008, "Programming", 3);

        // Act
        book.BorrowCopy();

        // Assert
        Assert.Equal(3, book.TotalCopies);
        Assert.Equal(2, book.AvailableCopies);
    }

    [Fact]
    public void Book_BorrowCopy_Should_Throw_When_No_Copies_Available()
    {
        // Arrange
        var isbn = new ISBN("978-0132350884");
        var book = new Book("Clean Code", "Robert Martin", isbn, 2008, "Programming", 1);
        book.BorrowCopy();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => book.BorrowCopy());
    }

    [Fact]
    public void Book_ReturnCopy_Should_Increase_Available_Copies()
    {
        // Arrange
        var isbn = new ISBN("978-0132350884");
        var book = new Book("Clean Code", "Robert Martin", isbn, 2008, "Programming", 3);
        book.BorrowCopy();

        // Act
        book.ReturnCopy();

        // Assert
        Assert.Equal(3, book.AvailableCopies);
    }

    [Fact]
    public void Book_AddCopies_Should_Increase_Total_And_Available()
    {
        // Arrange
        var isbn = new ISBN("978-0132350884");
        var book = new Book("Clean Code", "Robert Martin", isbn, 2008, "Programming", 3);

        // Act
        book.AddCopies(2);

        // Assert
        Assert.Equal(5, book.TotalCopies);
        Assert.Equal(5, book.AvailableCopies);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Book_Creation_Should_Throw_When_Title_Is_Empty(string title)
    {
        // Arrange
        var isbn = new ISBN("978-0132350884");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Book(title, "Robert Martin", isbn, 2008, "Programming", 5));
    }

    [Theory]
    [InlineData(999)]
    [InlineData(2030)]
    public void Book_Creation_Should_Throw_When_Year_Is_Invalid(int year)
    {
        // Arrange
        var isbn = new ISBN("978-0132350884");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Book("Clean Code", "Robert Martin", isbn, year, "Programming", 5));
    }
}
