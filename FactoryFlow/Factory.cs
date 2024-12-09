namespace FactoryFlow;

public interface IFactory
{
    string Name { get; }
    public IProduct ProductType { get; }
    IProduct Produce();
    double ProductionRate { get; }
}

public class Factory : IFactory
{
    public string Name { get; }
    public IProduct ProductType { get; }
    public double ProductionRate { get; }

    public Factory(string name, IProduct productType, double productionRate)
    {
        Name = name;
        ProductType = productType;
        ProductionRate = productionRate;
    }

    public IProduct Produce()
    {
        return new Product(ProductType.Name, ProductType.Weight, ProductType.Packaging);
    }
}