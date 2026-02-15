using System.Text.RegularExpressions;

namespace LibraryManagement.Core.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    private Email() { }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = value.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}