using SalesApi.Domain.Common;

namespace SalesApi.Domain.Entities;

public class Product : Entity
{
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _category = string.Empty;
    private string _image = string.Empty;
    private decimal _price;

    public Guid Id { get; set; }

    public string Title
    {
        get => _title;
        set
        {
            ValidateString(value, nameof(Title));
            _title = value;
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            ValidateMonetaryValue(value, nameof(Price));
            _price = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            ValidateString(value, nameof(Description));
            _description = value;
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            ValidateString(value, nameof(Category));
            _category = value;
        }
    }

    public string Image
    {
        get => _image;
        set
        {
            ValidateString(value, nameof(Image));
            _image = value;
        }
    }
} 