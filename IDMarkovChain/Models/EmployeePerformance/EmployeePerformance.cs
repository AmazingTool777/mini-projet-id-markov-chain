using System.Text.Json.Serialization;
using IDMarkovChain.Algorithms.KMeans;

namespace IDMarkovChain.Models.EmployeePerformance
{
    public class EmployeePerformance(int empId, string fullName, string action, int outputCount, int week) : IClusterPoint
    {
        public int EmpId { get; set; } = empId;

        public string FullName { get; set; } = fullName;

        public int OutputCount { get; set; } = outputCount;

        public string Action { get; set; } = action;

        public int Week { get; set; } = week;

        [JsonIgnore]
        public float Coordinate { get { return OutputCount; } }

        // Redéfinition de la méthode `Equals()` afin de permettre la comparaison des objets dans la collection `HashSet`
        public override bool Equals(object? obj)
        {
            if (obj is EmployeePerformance other)
            {
                return other.EmpId == EmpId && other.Week == Week;
            }
            return false;
        }

        // Redéfinition de la méthode `GetHashCode()` afin de faliciter le stockage des objets dans la collection `HashSet`
        public override int GetHashCode()
        {
            return HashCode.Combine(EmpId, Week);
        }
    }
}