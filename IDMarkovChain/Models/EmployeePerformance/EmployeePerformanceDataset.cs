using System.Diagnostics;
using System.Text.Json;
using IDMarkovChain.Algorithms.KMeans;
using IDMarkovChain.Utils;

namespace IDMarkovChain.Models.EmployeePerformance
{
    public static class EmployeePerformanceDataset
    {
        // Chemin vers le fichier contenant le dataset en JSON des performances des employés au cours de 2 semaines consécutives
        private static readonly string EMPLOYEES_PERFORMANCES_DATASET_PATH = "Models\\EmployeePerformance\\dataset.json";
        // Chemin vers le précédent dataset mais durant l'éxécution du code
        public static string EmployeesPerformancesDatasetPath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, EMPLOYEES_PERFORMANCES_DATASET_PATH);
            }
        }

        // Chemin vers le fichier contenant une liste de 240 noms complets
        private static readonly string FULL_NAMES_PATH = "Models\\EmployeePerformance\\full-names.json";
        // Chemin vers le précédent dataset mais durant l'éxécution du code
        public static string FullNamesPath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, FULL_NAMES_PATH);
            }
        }

        // Données en mémoire des performances des employés
        static List<EmployeePerformance>? employeesPerformances = null;

        // Le nombre d'échantillons d'employés pris aléatoirement pour calculer la probabilité d'une transition d'un état vers un autre
        public static readonly int ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT = 8;

        // Le nombre d'expériences sur la détermination de la matrice de transition
        public static readonly int ACTIONS_COMPUTE_EXPERIMENTS_COUNT = 100;

        // Les noms et libellés des actions possibles
        public static readonly (string name, string label)[] ActionsNamesLabels = [
            ("ne-rien-faire", "Ne rien faire"),
            ("augmenter-salaire-5-pourcent", "Augmenter le salaire de 5%"),
            ("concurrence-entre-employes", "Concurrence entre employés"),
            ("licenciement", "Licencier l'employé concerné"),
        ];

        // Les données des actions possibles que l'on a supposé au départ (hypothétique).
        // Les données en matrices de transitions doivent faire l'objet de vérifications
        // contre celles calculées à partir du dataset actuel.
        public static readonly MarkovChainAction[] HypotheticalActions = [
            new(ActionsNamesLabels[0].name, ActionsNamesLabels[0].label, new float[4, 4] {
                { 0.75f, 0.20f, 0.05f, 0.00f },
                { 0.25f, 0.60f, 0.10f, 0.05f },
                { 0.05f, 0.15f, 0.70f, 0.10f },
                { 0.00f, 0.05f, 0.20f, 0.75f }
            }),
            new(ActionsNamesLabels[1].name, ActionsNamesLabels[1].label, new float[4, 4] {
                { 0.40f, 0.35f, 0.20f, 0.05f },
                { 0.11f, 0.50f, 0.30f, 0.09f },
                { 0.05f, 0.10f, 0.55f, 0.30f },
                { 0.00f, 0.05f, 0.20f, 0.75f }
            }),
            new(ActionsNamesLabels[2].name, ActionsNamesLabels[2].label, new float[4, 4] {
                { 0.60f, 0.30f, 0.10f, 0.00f },
                { 0.15f, 0.55f, 0.25f, 0.05f },
                { 0.05f, 0.10f, 0.60f, 0.25f },
                { 0.00f, 0.05f, 0.10f, 0.85f }
            }),
            new(ActionsNamesLabels[3].name, ActionsNamesLabels[3].label, new float[4, 4] {
                { 0.85f, 0.10f, 0.05f, 0.00f },
                { 0.50f, 0.40f, 0.05f, 0.05f },
                { 0.25f, 0.20f, 0.40f, 0.15f },
                { 0.10f, 0.05f, 0.20f, 0.65f }
            })
        ];

        // Les valeurs des actions calculées à partir du dataset actuel.
        // Les valeurs des matrices de transitions seront calculées par la méthode `computeActions()`
        public static MarkovChainAction[] ComputedActions { get; } = [
            new(ActionsNamesLabels[0].name, ActionsNamesLabels[0].label, new float[4, 4]),
            new(ActionsNamesLabels[1].name, ActionsNamesLabels[1].label, new float[4, 4]),
            new(ActionsNamesLabels[2].name, ActionsNamesLabels[2].label, new float[4, 4]),
            new(ActionsNamesLabels[3].name, ActionsNamesLabels[3].label, new float[4, 4])
        ];

        // Charge les données sur les performances des employés en utilisant le design pattern "Singleton"
        public static List<EmployeePerformance>? Load(bool create = false)
        {
            if (create)
            {
                employeesPerformances = EmployeePerformancefactory.CreateMany();
                File.WriteAllText(EmployeesPerformancesDatasetPath, JsonSerializer.Serialize(employeesPerformances));
            }
            else if (employeesPerformances == null)
            {
                if (File.Exists(FullNamesPath))
                {
                    employeesPerformances = JsonSerializer.Deserialize<List<EmployeePerformance>>(File.ReadAllText(EmployeesPerformancesDatasetPath));
                }
                else
                {
                    employeesPerformances = EmployeePerformancefactory.CreateMany();
                    File.WriteAllText(EmployeesPerformancesDatasetPath, JsonSerializer.Serialize(employeesPerformances));
                }
            }

            return employeesPerformances;
        }

        // Calcule les valeurs des matrices de transitions correspondantes au dataset actuel
        // suite au résultat de l'algorithme des K-Moyennes sur le dataset
        public static MarkovChainAction[] ComputeActions()
        {
            List<EmployeePerformance> performances = Load()!;
            List<EmployeePerformance>[] performancesPerT = [
                [.. performances.Where(d => d.Week == 0)],
                [.. performances.Where(d => d.Week == 1)],
            ];

            int K = 4;
            KMeansCluster[] statesClusters = KMeansClustering.Clusterize(K, [.. performances!]);

            int actionPerformancesCount = performancesPerT[0].Count / ComputedActions.Length;

            for (int action = 0; action < ComputedActions.Length; action++)
            {
                var (name, label) = ActionsNamesLabels[action];

                List<float[,]> experimentsTransitionsMatrices = [];

                List<EmployeePerformance> actionInitialPerformances = performancesPerT[0].Slice(action * actionPerformancesCount, actionPerformancesCount);

                Dictionary<int, EmployeePerformance> actionFinalPerformancesByEmpId = [];
                foreach (EmployeePerformance performance in performancesPerT[1].Where(p => p.Action == name))
                {
                    actionFinalPerformancesByEmpId.Add(performance.EmpId, performance);
                }

                Dictionary<int, List<EmployeePerformance>> actionInitialPerformancesByCluster = [];
                foreach (EmployeePerformance performance in actionInitialPerformances)
                {
                    KMeansCluster parentCluster = KMeansClustering.FindParentCluster(performance, statesClusters);
                    if (!actionInitialPerformancesByCluster.ContainsKey(parentCluster.Id))
                    {
                        actionInitialPerformancesByCluster.Add(parentCluster.Id, []);
                    }
                    actionInitialPerformancesByCluster[parentCluster.Id].Add(performance);
                }

                for (int experiment = 0; experiment < ACTIONS_COMPUTE_EXPERIMENTS_COUNT; experiment++)
                {
                    float[,] experimentTransitionMatrix = new float[statesClusters.Length, statesClusters.Length];

                    foreach (KMeansCluster cluster in statesClusters)
                    {
                        int[] finalStatesCounts = new int[statesClusters.Length];

                        List<EmployeePerformance> clusterPerformances = actionInitialPerformancesByCluster[cluster.Id];

                        int[] clusterSamplesPerformancesIndices = ArrayUtils.GetRandomUniqueIndexes(clusterPerformances.Count, ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT);
                        for (int i = 0; i < ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT; i++)
                        {
                            Random random = new();
                            EmployeePerformance initialPerformance = clusterPerformances[clusterSamplesPerformancesIndices[i]];
                            EmployeePerformance finalPerformance = actionFinalPerformancesByEmpId[initialPerformance.EmpId];
                            KMeansCluster finalPerformanceCluster = KMeansClustering.FindParentCluster(finalPerformance, statesClusters);
                            finalStatesCounts[finalPerformanceCluster.Id] = finalStatesCounts[finalPerformanceCluster.Id] + 1;
                        }

                        for (int i = 0; i < finalStatesCounts.Length; i++)
                        {
                            experimentTransitionMatrix[cluster.Id, i] = (float)finalStatesCounts[i] / ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT;
                        }
                    }

                    experimentsTransitionsMatrices.Add(experimentTransitionMatrix);
                }

                for (int experiments = 0; experiments < ACTIONS_COMPUTE_EXPERIMENTS_COUNT; experiments++)
                {
                    float[,] experimentTransitionMatrix = experimentsTransitionsMatrices[experiments];
                    for (int i = 0; i < statesClusters.Length; i++)
                    {
                        for (int j = 0; j < statesClusters.Length; j++)
                        {
                            ComputedActions[action].TransitionMatrix[i, j] += experimentTransitionMatrix[i, j];
                        }
                    }
                }
                for (int i = 0; i < statesClusters.Length; i++)
                {
                    for (int j = 0; j < statesClusters.Length; j++)
                    {
                        ComputedActions[action].TransitionMatrix[i, j] /= ACTIONS_COMPUTE_EXPERIMENTS_COUNT;
                    }
                }
            }

            return ComputedActions;
        }
    }
}