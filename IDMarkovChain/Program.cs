using System.Text.Json;
using IDMarkovChain.Context;
using IDMarkovChain.Models.EmployeePerformance;
using IDMarkovChain.Utils;

class Program
{
    static void Main(string[] args)
    {
        Step4();
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
        int policiesCount = ProblemContext.Policies.Length;

        for (int i = 0; i < policiesCount; i++)
        {
            ActionsPolicy policy = ProblemContext.Policies[i];
            int statesCount = policy.ActionsIndices.Length;

            Console.WriteLine($"Politique de décision {i}:");
            Console.WriteLine("---- Matrice de transitions");
            MatrixUtils.PrintMatrix(policy.TransitionMatrix, 14);
            Console.WriteLine("---- Probabilités des états à l'état stationnaire");
            for (int j = 0; j < statesCount; j++)
            {
                Console.WriteLine($"¶{j} = {policy.StationaryProbabilities[j]}");
            }
            Console.WriteLine("---- Coûts à chaque état");
            for (int j = 0; j < statesCount; j++)
            {
                Console.WriteLine($"Etat {i}: {policy.Costs[j]}");
            }
            Console.WriteLine($"Coût moyen E(C) = {policy.MeanCost}");
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
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine($"Etat {i} => {ProblemContext.Policies[minPolicyIndice].ActionsIndices[i]}");
        }
    }
}