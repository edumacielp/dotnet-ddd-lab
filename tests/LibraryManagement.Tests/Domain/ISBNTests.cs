using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Tests.Domain;

public class ISBNTests
{
    [Theory]
    [InlineData("978-0132350884")]
    [InlineData("9780132350884")]
    [InlineData("0-306-40615-2")]
    [InlineData("0306406152")]
    public void ISBN_Creation_Should_Accept_Valid_ISBNs(string isbnValue)
    {
        // Act
        var isbn = new ISBN(isbnValue);

        // Assert
        Assert.NotNull(isbn);
        Assert.NotEmpty(isbn.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ISBN_Creation_Should_Throw_When_Empty(string isbnValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ISBN(isbnValue));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcdefghij")]
    [InlineData("978-013235088X")]
    public void ISBN_Creation_Should_Throw_When_Invalid_Format(string isbnValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ISBN(isbnValue));
    }

    [Fact]
    public void ISBN_Equality_Should_Work_Correctly()
    {
        // Arrange
        var isbn1 = new ISBN("978-0132350884");
        var isbn2 = new ISBN("9780132350884");

        // Act & Assert
        Assert.Equal(isbn1, isbn2);
    }

    [Fact]
    public void ISBN_ToString_Should_Return_Value()
    {
        // Arrange
        var isbn = new ISBN("9780132350884");

        // Act
        var result = isbn.ToString();

        // Assert
        Assert.Equal("9780132350884", result);
    }
}
