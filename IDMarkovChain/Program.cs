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
        string companyActivityDescription = "L'entreprise dans notre cas a pour activité la production de produits agro-alimentaires " +
        "emballés. La performance des employés est mesurée par le nombre de produits emballés par semaine. " +
        "Dans le dataset suivant de cette entreprise, l'attribut du nombre de produits emballés est le champs `OutputCount`.";
        Console.WriteLine(companyActivityDescription + "\n");

        Console.WriteLine("Voici un aperçu du dataset:");
        List<EmployeePerformance> performances = EmployeePerformanceDataset.Load()!;
        int GLIMPSE_TRUNCATE = 5;
        for (int i = 0; i < GLIMPSE_TRUNCATE; i++)
        {
            Console.WriteLine(JsonSerializer.Serialize(performances[i]));
        }
        Console.WriteLine(".\n.\n.");
        int lastIndex = performances.Count - 1;
        for (int i = 0; i < GLIMPSE_TRUNCATE; i++)
        {
            Console.WriteLine(JsonSerializer.Serialize(performances[lastIndex - i]));
        }
        Console.WriteLine();

        Console.WriteLine("Les matrices de transitions hypothétiques:");
        foreach (MarkovChainAction action in EmployeePerformanceDataset.HypotheticalActions)
        {
            action.Describe();
        }
        Console.WriteLine();

        Console.WriteLine("Les matrices de transitions calculées:");
        MarkovChainAction[] actions = EmployeePerformanceDataset.ComputeActions();
        foreach (MarkovChainAction action in actions)
        {
            action.Describe();
        }
    }
}