namespace IDMarkovChain.Utils
{
    public static class NumbersUtils
    {
        // Génère N nombres uniques entre un intervalle [min, max[ donné.
        public static double[] GenerateUniqueRandomNumbersInRange(int n, double min, double max)
        {
            // S'assurer que l'intervalle est suffisament grand pour supporter le nombre de valeurs à générer
            if (n > max - min + 1)
            {
                throw new ArgumentException("The range is too small to generate the required number of unique numbers.");
            }

            // HashSet pour stocker les nombres uniques
            HashSet<double> uniqueNumbers = [];
            Random random = new();

            // Boucler jusqu'a avoir le nombre de nombres uniques requis
            while (uniqueNumbers.Count < n)
            {
                // Générer un nombre unique entre l'intervalle [min, max]
                double newNumber = (max - min) * (double)random.NextDouble() + min;
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

        /// <summary>
        /// Génère un nombre aléatoire entre un intervalle [min, max[ donné.
        /// </summary>
        /// <param name="min">La borne inférieure de l'intervalle</param>
        /// <param name="max">La borne supérieure de l'intervalle</param>
        /// <returns>Un nombre aléatoire entre l'intervalle [min, max[</returns>
        public static double GetRandomNumberInRange(double min, double max)
        {
            Random random = new();
            return (max - min) * (double)random.NextDouble() + min;
        }

        /// <summary>
        /// Génère une distribution de probabilités aléatoires
        /// </summary>
        /// <param name="n">Le nombre de classes de la distribution</param>
        /// <param name="stepMargin">la coefficient à multiplier avec la prochaine probabilité pour servir de marge</param>
        /// <returns>Les probabilités de chaque classe</returns>
        public static double[] GenerateRandomProbabilityDistribution(int n, double stepMargin = 1.0)
        {
            double[] probabilities = new double[n];
            double cumul = 0, step, nextCumul;

            for (int i = 0; i < n; i++)
            {
                step = (double)(1 - cumul) / (n - i);
                if (i == (n - 1))
                {
                    probabilities[i] = 1 - cumul;
                    break;
                }
                nextCumul = GetRandomNumberInRange(cumul, cumul + step * (1 + stepMargin));
                probabilities[i] = nextCumul - cumul;
                cumul = nextCumul;
            }

            return probabilities;
        }
    }
}