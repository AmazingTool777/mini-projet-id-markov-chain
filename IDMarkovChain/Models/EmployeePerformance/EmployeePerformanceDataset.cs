using System.Text.Json;
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

        // Le nombre d'employés pris aléatoirement pour déterminer la matrice de transition pour chaque décision à l'aide de l'algorithme de K-Moyenne
        public static readonly int SAMPLES_COUNT = 5;

        // Le nombre d'expériences sur la détermination de la matrice de transition pour chaque décision à l'aide de l'algorithme de K-Moyenne
        // sur les employés pris aléatoirement
        public static readonly int EXPERIMENTS_COUNT = 3;

        // Liste des données concernant chaque action possible du manager
        // Les matrices de transitions sont purement hypothétiques;
        // elles doivent faire l'objet de vérification par l'application de l'algorithme k-moyennes.
        public static readonly MarkovChainAction[] HyptheticalActions = [
            new("ne-rien-faire", "Ne rien faire", new float[4, 4] {
                { 0.75f, 0.20f, 0.05f, 0.00f },
                { 0.25f, 0.60f, 0.10f, 0.05f },
                { 0.05f, 0.15f, 0.70f, 0.10f },
                { 0.00f, 0.05f, 0.20f, 0.75f }
            }),
            new("augmenter-salaire-5-pourcent", "Augmenter le salaire de 5%", new float[4, 4] {
                { 0.40f, 0.35f, 0.20f, 0.05f },
                { 0.11f, 0.50f, 0.30f, 0.09f },
                { 0.05f, 0.10f, 0.55f, 0.30f },
                { 0.00f, 0.05f, 0.20f, 0.75f }
            }),
            new("concurrence-entre-employes", "Concurrence entre employés", new float[4, 4] {
                { 0.60f, 0.30f, 0.10f, 0.00f },
                { 0.15f, 0.55f, 0.25f, 0.05f },
                { 0.05f, 0.10f, 0.60f, 0.25f },
                { 0.00f, 0.05f, 0.10f, 0.85f }
            }),
            new("licenciement", "Licencier l'employé concerné", new float[4, 4] {
                { 0.85f, 0.10f, 0.05f, 0.00f },
                { 0.50f, 0.40f, 0.05f, 0.05f },
                { 0.25f, 0.20f, 0.40f, 0.15f },
                { 0.10f, 0.05f, 0.20f, 0.65f }
            })
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
    }
}