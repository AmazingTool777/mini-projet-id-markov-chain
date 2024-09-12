using System.Text.Json;
using IDMarkovChain.Models.EmployeePerformance;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(JsonSerializer.Serialize(EmployeePerformanceDataset.Load()));
    }
}