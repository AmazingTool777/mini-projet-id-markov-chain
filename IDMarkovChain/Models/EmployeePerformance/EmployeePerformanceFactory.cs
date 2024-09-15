using IDMarkovChain.Models.EmployeePerformance;
using IDMarkovChain.Utils;
using System.Text.Json;
using System.IO;

namespace IDMarkovChain.Models.EmployeePerformance
{
    static class EmployeePerformancefactory
    {
        // Crée une liste de données à-propos des performances des employés
        public static List<EmployeePerformance> CreateMany()
        {
            // Initialisation de la liste
            List<EmployeePerformance> performances = [];

            // Chargement des nom complets
            string plainFullnames = File.ReadAllText(EmployeePerformanceDataset.FullNamesPath);
            List<string> fullNames = JsonSerializer.Deserialize<List<string>>(plainFullnames)!;
            // Indice courant du nom complet
            int currFullNameIndice = 0;

            // Construction des sous-intervalles à partir du nombre minimum et nombre maximum d'unités de productions possibles
            // Le nombre de sous-intervalles est le nombre de clusters.
            int clustersCount = EmployeePerformanceDataset.HypotheticalActions[0].TransitionMatrix.GetLength(0);
            const int MIN_OUTPUT_COUNT = 1, MAX_OUTPUT_COUNT = 201;
            List<int[]> outputCountRanges = NumbersUtils.GetSubRanges(clustersCount, MIN_OUTPUT_COUNT, MAX_OUTPUT_COUNT);

            int actionsCount = EmployeePerformanceDataset.HypotheticalActions.Length;

            // L'Id courant de l'employé (Commence par 1)
            int empId = 1;
            // Nombre de lignes de données à générer sur les employés par cluster
            int EMPLOYEES_COUNT_PER_CLUSTER = 15;

            // Création des données pour chaque action à chaque cluster
            for (int i = 0; i < actionsCount; i++)
            {
                for (int j = 0; j < clustersCount; j++)
                {
                    // Intervalle de valeurs du nombre d'unités de productions pour le cluster actuel
                    int[] outputCountRange = outputCountRanges[j];

                    for (int k = 0; k < EMPLOYEES_COUNT_PER_CLUSTER; k++)
                    {
                        Random outputCountRandom = new();
                        // L'action par défaut appliqué à l'employé à t=0 est "Ne rien faire"
                        EmployeePerformance performance = new(empId, fullNames[currFullNameIndice], "ne-rien-faire", outputCountRandom.Next(outputCountRange[0], outputCountRange[1]), 0);
                        performances.Add(performance);
                        empId++;
                        currFullNameIndice++;
                    }
                }
            }

            /**
             * Création les données des employés pour t=1
             * Chaque employé dans t=0 génère une ligne de donnée dans t=1
             * et l'ordre des Id des employés dans t=1 reste le même que dans t=0
             */

            // Variable pour suivre le comptage des données générées à t=1
            int empCount = 0;
            // Génération des données pour chaque action à chaque cluster
            // Elle suit le même ordre que celui de t=0.
            // Cela garantit que l'employé à l'indice i, j et k sont est le même employé à ces indices dans t=0
            for (int i = 0; i < actionsCount; i++)
            {
                MarkovChainAction action = EmployeePerformanceDataset.HypotheticalActions[i];

                for (int j = 0; j < clustersCount; j++)
                {
                    for (int k = 0; k < EMPLOYEES_COUNT_PER_CLUSTER; k++)
                    {
                        Random outputCountRandom = new();
                        // Choix de la prochaine intervalle de valeurs d'unités de productions suite à l'action actuel
                        // selon le choix aléatoire du prochain cluster (état) sachant le cluster actuel
                        int[] outputCountRange = outputCountRanges[action.Apply(j)];
                        EmployeePerformance performance = new(empCount + 1, fullNames[empCount], action.Name, outputCountRandom.Next(outputCountRange[0], outputCountRange[1]), 1);
                        performances.Add(performance);
                        empCount++;
                    }
                }
            }

            return performances;
        }
    }
}