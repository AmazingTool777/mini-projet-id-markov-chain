using IDMarkovChain.Models.EmployeePerformance;
using IDMarkovChain.Utils;
using System.Text.Json;
using System.IO;

namespace IDMarkovChain.Models.EmployeePerformance
{
    static class EmployeePerformancefactory
    {
        public static List<EmployeePerformance> CreateMany()
        {
            List<EmployeePerformance> performances = [];

            string plainFullnames = File.ReadAllText(EmployeePerformanceDataset.FullNamesPath);
            List<string> fullNames = JsonSerializer.Deserialize<List<string>>(plainFullnames)!;
            int currFullNameIndice = 0;

            int clustersCount = EmployeePerformanceDataset.HyptheticalActions[0].TransitionMatrix.GetLength(0);
            const int MIN_OUTPUT_COUNT = 1, MAX_OUTPUT_COUNT = 201;
            int outputCountRangeGap = (MAX_OUTPUT_COUNT - MIN_OUTPUT_COUNT) / clustersCount;
            List<int[]> outputCountRanges = [];
            for (int i = 0; i < clustersCount; i++)
            {
                int outputCountRangeMin = MIN_OUTPUT_COUNT + outputCountRangeGap * i;
                int outputCountRangeMax = outputCountRangeMin + outputCountRangeGap;
                outputCountRanges.Add([outputCountRangeMin, outputCountRangeMax]);
            }

            int empId = 1;

            int employeesCountPerCluster = EmployeePerformanceDataset.SAMPLES_COUNT * EmployeePerformanceDataset.EXPERIMENTS_COUNT;
            int actionsCount = EmployeePerformanceDataset.HyptheticalActions.Length;
            for (int i = 0; i < actionsCount; i++)
            {
                for (int j = 0; j < clustersCount; j++)
                {
                    int[] outputCountRange = outputCountRanges[j];

                    for (int k = 0; k < employeesCountPerCluster; k++)
                    {
                        Random outputCountRandom = new();
                        EmployeePerformance performance = new(empId, fullNames[currFullNameIndice], "ne-rien-faire", outputCountRandom.Next(outputCountRange[0], outputCountRange[1]), 0);
                        performances.Add(performance);
                        empId++;
                        currFullNameIndice++;
                    }
                }
            }

            int empReCount = 0;
            for (int i = 0; i < actionsCount; i++)
            {
                MarkovChainAction action = EmployeePerformanceDataset.HyptheticalActions[i];

                for (int j = 0; j < clustersCount; j++)
                {
                    for (int k = 0; k < employeesCountPerCluster; k++)
                    {
                        Random outputCountRandom = new();
                        int[] outputCountRange = outputCountRanges[action.Apply(j)];
                        EmployeePerformance performance = new(empReCount + 1, fullNames[empReCount], action.Name, outputCountRandom.Next(outputCountRange[0], outputCountRange[1]), 1);
                        performances.Add(performance);
                        empReCount++;
                    }
                }
            }

            return performances;
        }
    }
}