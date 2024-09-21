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