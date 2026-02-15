namespace LibraryManagement.Core.ValueObjects;

public class ISBN : ValueObject
{
    public string Value { get; private set; }

    private ISBN() { }

    public ISBN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ISBN cannot be empty", nameof(value));

        var cleanValue = value.Replace("-", "").Replace(" ", "");

        if (!IsValidISBN(cleanValue))
            throw new ArgumentException("Invalid ISBN format", nameof(value));

        Value = cleanValue;
    }

    private static bool IsValidISBN(string isbn)
    {
        // ISBN-10 ou ISBN-13
        if (isbn.Length == 10)
            return IsValidISBN10(isbn);

        if (isbn.Length == 13)
            return IsValidISBN13(isbn);

        return false;
    }

    private static bool IsValidISBN10(string isbn)
    {
        if (!isbn.All(c => char.IsDigit(c) || c == 'X'))
            return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (isbn[i] - '0') * (10 - i);
        }

        var lastChar = isbn[9];
        var checkDigit = lastChar == 'X' ? 10 : (lastChar - '0');
        sum += checkDigit;

        return sum % 11 == 0;
    }

    private static bool IsValidISBN13(string isbn)
    {
        if (!isbn.All(char.IsDigit))
            return false;

        var sum = 0;
        for (int i = 0; i < 12; i++)
        {
            var digit = isbn[i] - '0';
            sum += i % 2 == 0 ? digit : digit * 3;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == (isbn[12] - '0');
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}