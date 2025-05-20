namespace SalesApi.Domain.Common;

public static class BusinessRules
{
    public static class SaleItem
    {
        public const int MIN_QUANTITY_FOR_DISCOUNT = 4;
        public const int MIN_QUANTITY_FOR_HIGHER_DISCOUNT = 10;
        public const int MAX_QUANTITY = 20;
        public const decimal STANDARD_DISCOUNT_PERCENTAGE = 0.10m;
        public const decimal HIGHER_DISCOUNT_PERCENTAGE = 0.20m;

        public static string GetMaxQuantityExceededMessage(int quantity) =>
            $"You can buy only {MAX_QUANTITY} pieces of a item";

        public static string GetDiscountNotAllowedMessage() =>
            $"Discount is not allowed for quantities below {MIN_QUANTITY_FOR_DISCOUNT} items";
    }
} 