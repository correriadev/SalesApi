namespace SalesApi.Domain.Common;

public abstract class Entity
{
    protected static void ValidateString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{propertyName} cannot be null or empty", propertyName);
    }

    protected static void ValidateMonetaryValue(decimal value, string propertyName)
    {
        if (value < 0)
            throw new ArgumentException($"{propertyName} cannot be negative", propertyName);
    }

    protected static void ValidateQuantity(decimal value, string propertyName)
    {
        if (value <= 0)
            throw new ArgumentException($"{propertyName} must be greater than zero", propertyName);
    }
} 