using System.Collections.Concurrent;

namespace FactoryFlow;

public interface IWarehouse
{
    void AddProducts(IEnumerable<IProduct> products);
    List<IProduct> RetrieveProducts(int count);
    bool IsFull { get; }
    bool IsAlmostFull(double threshold);
    int CurrentLoad { get; }
    int Capacity { get; }
}

public class Warehouse : IWarehouse
{
    private readonly ConcurrentQueue<IProduct> _products = new ConcurrentQueue<IProduct>();
    public int Capacity { get; }
    public int CurrentLoad => _products.Count;

    public Warehouse(int multiplier)
    {
        Capacity = multiplier * 50; // Например, с учётом базовой производительности
    }

    public void AddProducts(IEnumerable<IProduct> products)
    {
        foreach (var product in products)
        {
            if (CurrentLoad >= Capacity)
                throw new InvalidOperationException("Склад переполнен!");
            _products.Enqueue(product);
        }
    }

    public List<IProduct> RetrieveProducts(int count)
    {
        var retrieved = new List<IProduct>();
        for (int i = 0; i < count; i++)
        {
            if (_products.TryDequeue(out var product))
                retrieved.Add(product);
        }
        return retrieved;
    }

    public bool IsFull => CurrentLoad >= Capacity;
    public bool IsAlmostFull(double threshold) => CurrentLoad >= Capacity * threshold;
}