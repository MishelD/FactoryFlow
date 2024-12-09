namespace FactoryFlow;

public interface ITruck
{
    string Model { get; }
    int Capacity { get; }
    void Load(IEnumerable<IProduct> products);
    IReadOnlyCollection<IProduct> GetLoad();
}

public class Truck : ITruck
{
    public string Model { get; }
    public int Capacity { get; }
    private readonly List<IProduct> _load = new List<IProduct>();

    public Truck(string model, int capacity)
    {
        Model = model;
        Capacity = capacity;
    }

    public void Load(IEnumerable<IProduct> products)
    {
        if (products.Count() > Capacity)
            throw new InvalidOperationException("Перегрузка грузовика!");
        
        _load.AddRange(products);
    }

    public IReadOnlyCollection<IProduct> GetLoad() => _load.AsReadOnly();
}