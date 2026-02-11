using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Product
{
    public Product()
    {

    }
    public Guid Id { get; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public bool Active { get; private set; }

    public ProductImage? Image { get; private set; }

    public ProductCategory Category { get; private set; }

    public Product(
        string name,
        string description,
        decimal price,
        ProductCategory category)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        Active = true;

        Validate();
    }

    public void ChangeImage(ProductImage image)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
    }

    public void RemoveImage()
    {
        Image = null;
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        Price = newPrice;
    }

    public void ChangeCategory(ProductCategory category)
    {
        if (category == ProductCategory.Undefined)
            throw new ArgumentException("Product category is required.");

        Category = category;
    }

    public void Deactivate()
    {
        Active = false;
    }

    // New: set product active
    public void Activate()
    {
        Active = true;
    }

    // New: change name
    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        Name = name;
    }

    // New: change description
    public void ChangeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description is required.");

        Description = description;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Product name is required.");

        if (Price <= 0)
            throw new ArgumentException("Product price must be greater than zero.");

        if (Category == ProductCategory.Undefined)
            throw new ArgumentException("Product category is required.");
    }
}
