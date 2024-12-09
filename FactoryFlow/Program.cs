namespace FactoryFlow;

class Program
{
    static void Main(string[] args)
    {
        IWarehouse warehouse = new Warehouse(100); // 100 — множитель вместимости склада
        var simulation = new FactorySimulation(warehouse);
        
        Console.Write("Введите длительность симуляции в секундах (виртуальных часах):");
        int simulationTime = Convert.ToInt16(Console.ReadLine());
        
        simulation.Run(simulationTime);
    }
}






