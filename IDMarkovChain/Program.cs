using System.Text.Json;
using IDMarkovChain.Algorithms.GeneticAlgorithms;
using IDMarkovChain.Context;
using IDMarkovChain.Models.EmployeePerformance;
using IDMarkovChain.Utils;

class Program
{
    static void Main(string[] args)
    {
        Step5();
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
        foreach (MarkovChainAction action in ProblemContext.HypotheticalActions)
        {
            action.Describe();
        }
        Console.WriteLine();

        Console.WriteLine("Les matrices de transitions calculées:");
        foreach (MarkovChainAction action in ProblemContext.ComputedActions)
        {
            action.Describe();
        }
    }

    static void Step4()
    {
        int statesCount = ProblemContext.HypotheticalActions[0].TransitionMatrix.GetLength(0);
        int policiesCount = ProblemContext.Policies.Length;

        Console.WriteLine("Le tableau des coûts:");
        MatrixUtils.PrintMatrix(ProblemContext.CostsTable);
        Console.WriteLine();

        for (int i = 0; i < policiesCount; i++)
        {
            ActionsPolicy policy = ProblemContext.Policies[i];

            Console.WriteLine($"Politique de décision {i}:");
            policy.Describe();
            Console.WriteLine();
        }

        double minMeanCost = double.MaxValue;
        int minPolicyIndice = -1;
        for (int i = 0; i < policiesCount; i++)
        {
            ActionsPolicy policy = ProblemContext.Policies[i];
            if (policy.MeanCost < minMeanCost)
            {
                minMeanCost = policy.MeanCost;
                minPolicyIndice = i;
            }
        }
        string output = "La politique de décision avec le moins de coûts est "
            + $"la politique décision {minPolicyIndice} avec un coût moyen de {minMeanCost}. "
            + $"La politique de décision {minPolicyIndice} est donc la plus optimale pour minimiser les coûts sur le long-terme.";
        Console.WriteLine(output);
        Console.WriteLine("Politique:");
        for (int i = 0; i < statesCount; i++)
        {
            Console.WriteLine($"Etat {i} => Décision {ProblemContext.Policies[minPolicyIndice].ActionsIndices[i]}");
        }
    }

    public static void Step5()
    {
        MinMeanCostGeneticAlgorithms minMeanCostGeneticAlgorithms = new();
        IGenAlgoIndividual<int[]> solution = minMeanCostGeneticAlgorithms.Run();
        solution.Describe();
    }
}