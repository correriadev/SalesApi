using SalesApi.Domain.Common;
using SalesApi.Domain.ValueObjects;

namespace SalesApi.Domain.Entities;

public class Product : Entity
{
    public string Title { get; private set; }
    public Money Price { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }
    public string Image { get; private set; }

    private Product() 
    { 
        // Initialize non-nullable properties for EF Core
        Title = string.Empty;
        Price = Money.Zero;
        Description = string.Empty;
        Category = string.Empty;
        Image = string.Empty;
    } // For EF Core

    public Product(string title, Money price, string description, string category, string image)
    {
        ValidateString(title, nameof(Title));
        ValidateString(description, nameof(Description));
        ValidateString(category, nameof(Category));
        ValidateString(image, nameof(Image));

        Title = title;
        Price = price;
        Description = description;
        Category = category;
        Image = image;
    }

    public void UpdateTitle(string title)
    {
        ValidateString(title, nameof(Title));
        Title = title;
    }

    public void UpdatePrice(Money price)
    {
        Price = price;
    }

    public void UpdateDescription(string description)
    {
        ValidateString(description, nameof(Description));
        Description = description;
    }

    public void UpdateCategory(string category)
    {
        ValidateString(category, nameof(Category));
        Category = category;
    }

    public void UpdateImage(string image)
    {
        ValidateString(image, nameof(Image));
        Image = image;
    }
} 