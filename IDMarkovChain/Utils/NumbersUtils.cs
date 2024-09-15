namespace IDMarkovChain.Utils
{
    public static class NumbersUtils
    {
        // Génère N nombres uniques entre un intervalle [min, max[ donné.
        public static float[] GenerateUniqueRandomNumbersInRange(int n, float min, float max)
        {
            // S'assurer que l'intervalle est suffisament grand pour supporter le nombre de valeurs à générer
            if (n > max - min + 1)
            {
                throw new ArgumentException("The range is too small to generate the required number of unique numbers.");
            }

            // HashSet pour stocker les nombres uniques
            HashSet<float> uniqueNumbers = [];
            Random random = new();

            // Boucler jusqu'a avoir le nombre de nombres uniques requis
            while (uniqueNumbers.Count < n)
            {
                // Générer un nombre unique entre l'intervalle [min, max]
                float newNumber = (max - min) * (float)random.NextDouble() + min;
                // HashSet garantit l'unicité donc plus la peine de vérifier les doublons
                uniqueNumbers.Add(newNumber);
            }

            return [.. uniqueNumbers];
        }

        // Divise une intervalle [min, max[ donnée en N sous-intervalles
        public static List<int[]> GetSubRanges(int n, int min, int max)
        {
            int gap = (max - min) / n;
            List<int[]> ranges = [];
            for (int i = 0; i < n; i++)
            {
                int outputCountRangeMin = min + gap * i;
                int outputCountRangeMax = outputCountRangeMin + gap;
                ranges.Add([outputCountRangeMin, outputCountRangeMax]);
            }
            return ranges;
        }
    }
}