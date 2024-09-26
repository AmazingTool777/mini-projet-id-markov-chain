using System;
using System.Collections.Generic;
using System.Linq;

namespace IDMarkovChain.Algorithms.GeneticAlgorithms
{
    /* public class Example
    {
        // Number of individuals in each generation 
        private const int POPULATION_SIZE = 100;

        // Valid Genes 
        private const string GENES = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOP" +
                                    "QRSTUVWXYZ 1234567890, .-;:_!\"#%&/()=?@${[]}";

        // Target string to be generated 
        private const string TARGET = "I love GeeksforGeeks";

        private static readonly Random random = new Random();

        // Function to generate random numbers in given range 
        private static int RandomNum(int start, int end)
        {
            return random.Next(start, end + 1);
        }

        // Create random genes for mutation 
        private static char MutatedGenes()
        {
            int len = GENES.Length;
            int r = RandomNum(0, len - 1);
            return GENES[r];
        }

        // Create chromosome or string of genes 
        private static string CreateGnome()
        {
            int len = TARGET.Length;
            char[] gnome = new char[len];
            for (int i = 0; i < len; i++)
            {
                gnome[i] = MutatedGenes();
            }
            return new string(gnome);
        }

        // Class representing individual in population 
        private class Individual
        {
            public string Chromosome { get; }
            public int Fitness { get; }

            public Individual(string chromosome)
            {
                Chromosome = chromosome;
                Fitness = CalculateFitness();
            }

            // Calculate fitness score, it is the number of 
            // characters in string which differ from target string. 
            private int CalculateFitness()
            {
                return Chromosome.Zip(TARGET, (a, b) => a == b ? 0 : 1).Sum();
            }

            // Perform mating and produce new offspring 
            public Individual Mate(Individual parent2)
            {
                char[] childChromosome = new char[Chromosome.Length];
                for (int i = 0; i < Chromosome.Length; i++)
                {
                    double p = random.NextDouble();
                    if (p < 0.45)
                        childChromosome[i] = Chromosome[i];
                    else if (p < 0.90)
                        childChromosome[i] = parent2.Chromosome[i];
                    else
                        childChromosome[i] = MutatedGenes();
                }
                return new Individual(new string(childChromosome));
            }
        }

        // Overloading < operator 
        private class FitnessComparer : IComparer<Individual>
        {
            public int Compare(Individual ind1, Individual ind2)
            {
                return ind1.Fitness.CompareTo(ind2.Fitness);
            }
        }

        // Driver code 
        public static void Main()
        {
            // current generation 
            int generation = 0;

            List<Individual> population = new List<Individual>();
            bool found = false;

            // create initial population 
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                string gnome = CreateGnome();
                population.Add(new Individual(gnome));
            }

            while (!found)
            {
                // sort the population in increasing order of fitness score 
                population.Sort(new FitnessComparer());

                // if the individual having lowest fitness score ie. 
                // 0 then we know that we have reached the target 
                // and break the loop 
                if (population[0].Fitness == 0)
                {
                    found = true;
                    break;
                }

                // Otherwise generate new offsprings for new generation 
                List<Individual> newGeneration = new List<Individual>();

                // Perform Elitism, that means 10% of fittest population 
                // goes to the next generation 
                int s = (10 * POPULATION_SIZE) / 100;
                for (int i = 0; i < s; i++)
                    newGeneration.Add(population[i]);

                // From 50% of fittest population, Individuals 
                // will mate to produce offspring 
                s = (90 * POPULATION_SIZE) / 100;
                for (int i = 0; i < s; i++)
                {
                    int len = population.Count;
                    int r = RandomNum(0, 50);
                    Individual parent1 = population[r];
                    r = RandomNum(0, 50);
                    Individual parent2 = population[r];
                    Individual offspring = parent1.Mate(parent2);
                    newGeneration.Add(offspring);
                }
                population = newGeneration;
                Console.WriteLine("Generation: " + generation + "\t" +
                                "String: " + population[0].Chromosome + "\t" +
                                "Fitness: " + population[0].Fitness);

                generation++;
            }

            Console.WriteLine("Generation: " + generation + "\t" +
                            "String: " + population[0].Chromosome + "\t" +
                            "Fitness: " + population[0].Fitness);
        }
    } */

}