using IDMarkovChain.Utils;

namespace IDMarkovChain.Context
{
    public class MarkovChainAction(string name, string label, double[,] transitionMatrix)
    {
        public string Name { get; } = name;

        public string Label { get; } = label;

        public double[,] TransitionMatrix { get; } = transitionMatrix;

        // Génère un état final de transition aléatoire à partir d'un état initial donné
        // mais qui sera choisie selon les probabilités de chaque transition
        public int Apply(int initialState)
        {
            // Correspondance entre les probabilités et les états finaux correspondants
            Dictionary<double, int> probsStatesMap = new();
            int matrixLength = TransitionMatrix.GetLength(0);
            for (int i = 0; i < matrixLength; i++)
            {
                probsStatesMap.TryAdd(TransitionMatrix[initialState, i], i);
            }

            // Tri et copie des probabilités des transitions à partir de l'état initial donné
            double[] sortedProbabilities = new double[matrixLength];
            for (int i = 0; i < matrixLength; i++)
            {
                sortedProbabilities[i] = TransitionMatrix[initialState, i];
            }
            Array.Sort(sortedProbabilities);

            // Génération d'une nombre aléatoire
            Random nextStateProbRandom = new();
            double nextStateProbability = (double)nextStateProbRandom.NextDouble();
            // Détermination de l'état final de transition correspondant au nombre aléatoire généré
            // en utilisant un tableau cumulé des probabilités à partir de l'état initial donné
            // et de la correspondance entre les probabilités de transitions et les états finaux de transitions
            double cumulProbabilities = 0;
            for (int i = 0; i < matrixLength; i++)
            {
                cumulProbabilities += sortedProbabilities[i];
                if (nextStateProbability <= cumulProbabilities)
                {
                    return probsStatesMap.GetValueOrDefault(sortedProbabilities[i]);
                }
            }

            return 0;
        }

        public void Describe()
        {
            Console.WriteLine("- ACTION: " + Label + " ----------------------------------------------");
            MatrixUtils.PrintMatrix(TransitionMatrix, 12);
        }
    }
}