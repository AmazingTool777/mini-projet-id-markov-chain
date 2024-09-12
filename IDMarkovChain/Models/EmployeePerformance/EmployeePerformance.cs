namespace IDMarkovChain.Models.EmployeePerformance
{
    public class EmployeePerformance(int empId, string fullName, string action, int outputCount, int week)
    {
        public int EmpId { get; set; } = empId;

        public string FullName { get; set; } = fullName;

        public int OutputCount { get; set; } = outputCount;

        public string Action { get; set; } = action;

        public int Week { get; set; } = week;
    }
}