namespace Domain.ValueObjects;

public sealed class ProductImage
{
    public string Path { get; }

    private ProductImage(string path)
    {
        Path = path;
    }

    public static ProductImage Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Image path cannot be empty.");

        if (!Uri.TryCreate(path, UriKind.Absolute, out _))
            throw new ArgumentException("Image path must be a valid absolute URI.");

        return new ProductImage(path);
    }
    public override bool Equals(object? obj)
        => obj is ProductImage other && Path == other.Path;

    public override int GetHashCode()
        => Path.GetHashCode();
}
