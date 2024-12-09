namespace FactoryFlow;

// Simulation
public class FactorySimulation
{
    private readonly IWarehouse _warehouse;
    private readonly List<IFactory> _factories = new List<IFactory>();
    private readonly List<IProduct> _products = new List<IProduct>();
    private readonly List<ITruck> _trucks = new List<ITruck>();
    private readonly List<List<IProduct>> _truckLogs = new List<List<IProduct>>();

    public FactorySimulation(IWarehouse warehouse)
    {
        _warehouse = warehouse;
        InitializeProducts();
        InitializeFactories();
        InitializeTrucks();
    }

    private void InitializeProducts()
    {
        _products.Add(new Product("Product A", 10, "Standart"));
        _products.Add(new Product("Product B", 15, "Wooden box"));
        _products.Add(new Product("Product C", 8, "Plastic box"));
    }
    
    private void InitializeFactories()
    {
        _factories.Add(new Factory("Factory 1", _products[0], 50));
        _factories.Add(new Factory("Factory 2", _products[1], 55));
        _factories.Add(new Factory("Factory 3", _products[2], 60));
    }

    private void InitializeTrucks()
    {
        _trucks.Add(new Truck("Small Truck", 100));
        _trucks.Add(new Truck("Medium Truck", 150));
        _trucks.Add(new Truck("Large Truck", 200));
    }

    /// <summary>
    /// Запуск симуляции
    /// </summary>
    /// <param name="simulationTime">зачение в секундах.
    /// 1 секунда = 1 виртуальному часу</param>
    public void Run(int simulationTime)
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Запуск фабрик
        var factoryTasks = _factories.Select(factory => Task.Run(() => FactoryWork(factory, token))).ToArray();

        // Мониторинг склада
        var monitorTask = Task.Run(() => MonitorWarehouse(token));

        // Завершение симуляции через x "часов"
        Task.Delay(TimeSpan.FromSeconds(simulationTime)).ContinueWith(_ => cts.Cancel());

        try
        {
            Task.WaitAll(factoryTasks);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Симуляция завершена.");
        }

        PrintStatistics();
    }

    private void FactoryWork(IFactory factory, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Thread.Sleep(1000); // 1 "час"
            var products = Enumerable.Repeat(factory.Produce(), (int)factory.ProductionRate).ToList();
            _warehouse.AddProducts(products);
            Console.WriteLine($"{factory.Name} произвёл {products.Count} единиц продукта {factory.ProductType.Name}");
        }
    }

    private void MonitorWarehouse(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_warehouse.IsAlmostFull(0.95))
            {
                Console.WriteLine("Склад заполнен на 95%. Начинается доставка...");
                LoadTrucks();
            }
        }
    }

    private void LoadTrucks()
    {
        foreach (var truck in _trucks)
        {
            var load = _warehouse.RetrieveProducts(truck.Capacity);
            if (load.Count > 0)
            {
                truck.Load(load);
                _truckLogs.Add(load);

                // Группируем продукцию по типу
                var grouped = load.GroupBy(product => product.Name)
                    .Select(g => new { Product = g.Key, Count = g.Count() });
            
                Console.WriteLine($"Грузовик {truck.Model} увёз {load.Count} единиц продукции:");
                foreach (var group in grouped)
                {
                    Console.WriteLine($"- {group.Product}: {group.Count}");
                }
            }
        }
    }

    private void PrintStatistics()
    {
        Console.WriteLine("Статистика доставки:");

        var totalProducts = _truckLogs.Sum(log => log.Count);
        var averageLoad = totalProducts / (double)_truckLogs.Count;

        // Группируем по типам продукции
        var productStats = _truckLogs.SelectMany(log => log)
            .GroupBy(product => product.Name)
            .Select(g => new
            {
                Product = g.Key,
                TotalCount = g.Count(),
                AverageCount = g.Count() / (double)_truckLogs.Count
            }).ToList();

        // Определяем самый частый товар
        var mostFrequentProduct = productStats.OrderByDescending(stat => stat.AverageCount).FirstOrDefault();

        Console.WriteLine($"Всего отправлено: {_truckLogs.Count} грузовиков.");
        Console.WriteLine($"Средняя загрузка: {averageLoad:F2} единиц.");

        foreach (var stat in productStats)
        {
            Console.WriteLine($"- {stat.Product}: в среднем {stat.AverageCount:F2} единиц за поездку.");
        }

        if (mostFrequentProduct != null)
        {
            Console.WriteLine($"Самый частый товар: {mostFrequentProduct.Product}, в среднем {mostFrequentProduct.AverageCount:F2} единиц за поездку.");
        }
    }
}