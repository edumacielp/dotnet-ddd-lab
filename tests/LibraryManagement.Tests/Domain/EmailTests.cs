using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Tests.Domain;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("john+tag@gmail.com")]
    public void Email_Creation_Should_Accept_Valid_Emails(string emailValue)
    {
        // Act
        var email = new Email(emailValue);

        // Assert
        Assert.NotNull(email);
        Assert.Equal(emailValue.ToLowerInvariant(), email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Email_Creation_Should_Throw_When_Empty(string emailValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(emailValue));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user name@example.com")]
    public void Email_Creation_Should_Throw_When_Invalid_Format(string emailValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(emailValue));
    }

    [Fact]
    public void Email_Should_Convert_To_Lowercase()
    {
        // Arrange & Act
        var email = new Email("Test@EXAMPLE.COM");

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Email_Equality_Should_Work_Correctly()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("TEST@EXAMPLE.COM");

        // Act & Assert
        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Email_ToString_Should_Return_Value()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal("test@example.com", result);
    }
}
