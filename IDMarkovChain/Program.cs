using System.Text.Json;
using IDMarkovChain.Algorithms.KMeans;
using IDMarkovChain.Models.EmployeePerformance;
using IDMarkovChain.Utils;

class Program
{
    static void Main(string[] args)
    {
        Step3();
    }

    static void Step3()
    {
        MarkovChainAction[] actions = EmployeePerformanceDataset.ComputeActions();
        foreach (MarkovChainAction action in actions)
        {
            action.Describe();
        }
    }
}