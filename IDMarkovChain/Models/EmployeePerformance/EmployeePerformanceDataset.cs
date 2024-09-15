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
        public static readonly int ACTIONS_COMPUTE_EXPERIMENTS_COUNT = 1000;

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
            new(ActionsNamesLabels[0].name, ActionsNamesLabels[0].label, new double[4, 4] {
                { 0.75, 0.20, 0.05, 0.00 },
                { 0.25, 0.60, 0.10, 0.05 },
                { 0.05, 0.15, 0.70, 0.10 },
                { 0.00, 0.05, 0.20, 0.75 }
            }),
            new(ActionsNamesLabels[1].name, ActionsNamesLabels[1].label, new double[4, 4] {
                { 0.40, 0.35, 0.20, 0.05 },
                { 0.11, 0.50, 0.30, 0.09 },
                { 0.05, 0.10, 0.55, 0.30 },
                { 0.00, 0.05, 0.20, 0.75 }
            }),
            new(ActionsNamesLabels[2].name, ActionsNamesLabels[2].label, new double[4, 4] {
                { 0.60, 0.30, 0.10, 0.00 },
                { 0.15, 0.55, 0.25, 0.05 },
                { 0.05, 0.10, 0.60, 0.25 },
                { 0.00, 0.05, 0.10, 0.85 }
            }),
            new(ActionsNamesLabels[3].name, ActionsNamesLabels[3].label, new double[4, 4] {
                { 0.85, 0.10, 0.05, 0.00 },
                { 0.50, 0.40, 0.05, 0.05 },
                { 0.25, 0.20, 0.40, 0.15 },
                { 0.10, 0.05, 0.20, 0.65 }
            })
        ];

        // Les valeurs des actions calculées à partir du dataset actuel.
        // Les valeurs des matrices de transitions seront calculées par la méthode `computeActions()`
        public static MarkovChainAction[] ComputedActions { get; set; } = new MarkovChainAction[ActionsNamesLabels.Length];

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
            // Chargument du dataset sur les performances des employés
            List<EmployeePerformance> performances = Load()!;
            // Diviser le dataset en 2 pour chaque t ([0]: t=0, [1]: t=1)
            List<EmployeePerformance>[] performancesPerT = [
                [.. performances.Where(d => d.Week == 0)],
                [.. performances.Where(d => d.Week == 1)],
            ];

            // Nombre de clusters
            int K = 4; // ou HypotheticalActions[0].TransitionMatrix.GetLength(0)
            // Clusterisation du dataset selon le niveau de performance
            KMeansCluster[] statesClusters = KMeansClustering.Clusterize(K, [.. performances!]);

            // Le nombre d'employés qui sont appliqués à une action
            // est obtenu par la division du nombre d'employés sur le nombre d'actions définies
            int actionPerformancesCount = performancesPerT[0].Count / ComputedActions.Length;

            // Calcul de la matrice de transition à chaque action
            for (int action = 0; action < ComputedActions.Length; action++)
            {
                // Nom + Libellé de l'action actuelle
                var actionData = ActionsNamesLabels[action];

                // Les matrices de transitions obtenues par toutes les experiences des moyennes experimentales
                List<double[,]> experimentsTransitionsMatrices = [];

                // Les données des employés qui ont subi l'action actuelle à partir de t=0 (à l'état initial)
                List<EmployeePerformance> actionInitialPerformances = performancesPerT[0].Slice(action * actionPerformancesCount, actionPerformancesCount);
                // Les données des employés résultant de l'action actuelle à t=1 (à l'état finale)
                // Dictionnaire: Clé: l'Id de l'employé - Valeur: les données sur l'employé comme valeur
                Dictionary<int, EmployeePerformance> actionFinalPerformancesByEmpId = [];
                foreach (EmployeePerformance performance in performancesPerT[1].Where(p => p.Action == actionData.name))
                {
                    actionFinalPerformancesByEmpId.Add(performance.EmpId, performance);
                }

                // Répartition des données performances à l'état initial par cluster
                // Dictionnaire: Clé: Id du cluster - Valeur: Liste des données de performance dans le cluster
                Dictionary<int, List<EmployeePerformance>> actionInitialPerformancesByCluster = [];
                foreach (EmployeePerformance performance in actionInitialPerformances)
                {
                    KMeansCluster parentCluster = KMeansClustering.FindParentCluster(performance, statesClusters);
                    if (!actionInitialPerformancesByCluster.TryGetValue(parentCluster.Id, out List<EmployeePerformance>? value))
                    {
                        value = ([]);
                        actionInitialPerformancesByCluster.Add(parentCluster.Id, value);
                    }

                    value.Add(performance);
                }

                // Calcul des matrices de transitions à chaque experience des moyennes expérimentales
                for (int experiment = 0; experiment < ACTIONS_COMPUTE_EXPERIMENTS_COUNT; experiment++)
                {
                    // Initialisation de matrice de transition pour l'experience actuelle
                    double[,] experimentTransitionMatrix = new double[statesClusters.Length, statesClusters.Length];

                    // Calcul ligne-par-ligne de la matrice de transition
                    // Chaque ligne correspond à chaque état (cluster) initial possible
                    foreach (KMeansCluster cluster in statesClusters)
                    {
                        // Initialisation d'un tableau contenant le nombre d'occurences de transitions
                        // à partir de l'état initial actuel vers tous les états finaux possibles
                        int[] finalStatesCounts = new int[statesClusters.Length];

                        // Générer un nombre défini `ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT` d'indices aléatoires
                        // parmis la liste des données de performance dans le cluster actuel
                        List<EmployeePerformance> clusterPerformances = actionInitialPerformancesByCluster[cluster.Id];
                        int[] clusterSamplesPerformancesIndices = ArrayUtils.GetRandomUniqueIndices(clusterPerformances.Count, ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT);
                        // Pour chaque données de performances séléctionnées aléatoirement,
                        // dénombrer le nombre d'occurences de transitions vers l'état final de l'employé correspondant
                        for (int i = 0; i < ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT; i++)
                        {
                            Random random = new();
                            EmployeePerformance initialPerformance = clusterPerformances[clusterSamplesPerformancesIndices[i]];
                            EmployeePerformance finalPerformance = actionFinalPerformancesByEmpId[initialPerformance.EmpId];
                            KMeansCluster finalPerformanceCluster = KMeansClustering.FindParentCluster(finalPerformance, statesClusters);
                            // Incrémenter le nombre d'occurence de transitions partant de l'état actuel vers l'état final de l'employé
                            finalStatesCounts[finalPerformanceCluster.Id]++;
                        }

                        // Remplir la ligne de la matrice de transition correspondant à l'état (cluster) actuel initial
                        for (int i = 0; i < finalStatesCounts.Length; i++)
                        {
                            // P[i,j] = nombre d'occurences de i vers j / nombre d'individus séléctionnés
                            experimentTransitionMatrix[cluster.Id, i] = (double)finalStatesCounts[i] / ACTIONS_COMPUTE_RANDOM_SAMPLES_COUNT;
                        }
                    }

                    // Ajouter la matrice de transition à la liste des matrices de transitions
                    // pour l'ensemble des toutes les expériences
                    experimentsTransitionsMatrices.Add(experimentTransitionMatrix);
                }

                // Matrice de transition moyenne des matrices de transition par toutes les expériences
                double[,] avgTransitionMatrix = MatrixUtils.AverageMatrices(experimentsTransitionsMatrices);
                // La matrice de transition moyenne sera utilisée par l'action calculée de l'action actuelle
                ComputedActions[action] = new(actionData.name, actionData.label, avgTransitionMatrix);
            }

            return ComputedActions;
        }
    }
}