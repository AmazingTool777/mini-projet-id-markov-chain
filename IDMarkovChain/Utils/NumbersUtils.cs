namespace IDMarkovChain.Utils
{
    public static class NumbersUtils
    {
        public static float[] GenerateUniqueRandomNumbersInRange(int count, float min, float max)
        {
            // S'assurer que l'intervalle est suffisament grand pour supporter le nombre de valeurs à générer
            if (count > max - min + 1)
            {
                throw new ArgumentException("The range is too small to generate the required number of unique numbers.");
            }

            // HashSet pour stocker les nombres uniques
            HashSet<float> uniqueNumbers = [];
            Random random = new();

            // Boucler jusqu'a avoir le nombre de nombres uniques requis
            while (uniqueNumbers.Count < count)
            {
                // Générer un nombre unique entre l'intervalle [min, max]
                float newNumber = (max - min) * (float)random.NextDouble() + min;
                // HashSet garantit l'unicité donc plus la peine de vérifier les doublons
                uniqueNumbers.Add(newNumber);
            }

            return [.. uniqueNumbers];
        }
    }
}