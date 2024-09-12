namespace IDMarkovChain.Utils
{
    public class MarkovChainAction(string name, string label, float[,] transitionMatrix)
    {
        public string Name { get; } = name;

        public string Label { get; } = label;

        public float[,] TransitionMatrix { get; } = transitionMatrix;

        public int Apply(int initialState)
        {
            Dictionary<float, int> probsStatesMap = new();
            int matrixLength = TransitionMatrix.GetLength(0);
            for (int i = 0; i < matrixLength; i++)
            {
                probsStatesMap.TryAdd(TransitionMatrix[initialState, i], i);
            }

            float[] sortedProbabilities = new float[matrixLength];
            for (int i = 0; i < matrixLength; i++)
            {
                sortedProbabilities[i] = TransitionMatrix[initialState, i];
            }
            Array.Sort(sortedProbabilities);

            Random nextStateProbRandom = new();
            float nextStateProbability = (float)nextStateProbRandom.NextDouble();
            float cumulProbabilities = 0;
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
    }
}