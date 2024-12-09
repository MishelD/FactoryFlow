namespace FactoryFlow;

public interface IProduct
{
    string Name { get; }
    double Weight { get; }
    string Packaging { get; }
}

public class Product : IProduct
{
    public string Name { get; }
    public double Weight { get; }
    public string Packaging { get; }

    public Product(string name, double weight, string packaging)
    {
        Name = name;
        Weight = weight;
        Packaging = packaging;
    }

    public Product(IProduct product)
    {
        Name = product.Name;
        Weight = product.Weight;
        Packaging = product.Packaging;
    }
}
