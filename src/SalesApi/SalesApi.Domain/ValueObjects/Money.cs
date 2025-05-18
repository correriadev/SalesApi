using System.Globalization;

namespace SalesApi.Domain.ValueObjects;

public sealed class Money
{
    private readonly decimal _amount;
    private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    private Money(decimal amount)
    {
        _amount = amount;
    }

    public static Money FromDecimal(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Money amount cannot be negative", nameof(amount));

        return new Money(Math.Round(amount, 2));
    }

    public static Money Zero => new(0);

    public decimal ToDecimal() => _amount;

    public override string ToString() => _amount.ToString("F2", _culture);

    public static Money operator +(Money left, Money right) => 
        FromDecimal(left._amount + right._amount);

    public static Money operator -(Money left, Money right) => 
        FromDecimal(left._amount - right._amount);

    public static Money operator *(Money money, decimal multiplier) => 
        FromDecimal(money._amount * multiplier);

    public static bool operator ==(Money? left, Money? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left._amount == right._amount;
    }

    public static bool operator !=(Money? left, Money? right) => 
        !(left == right);

    public override bool Equals(object? obj)
    {
        if (obj is Money other)
            return _amount == other._amount;
        return false;
    }

    public override int GetHashCode() => 
        _amount.GetHashCode();
} 