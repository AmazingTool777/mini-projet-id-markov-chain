namespace IDMarkovChain.Utils
{
    public static class ArrayUtils
    {
        // Génère N indices alétaoires d'un tableau à une taille donnée
        public static int[] GetRandomUniqueIndices(int arraySize, int n)
        {
            Random rand = new();

            // Création d'un tableau (trié) des indices
            int[] indexes = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                indexes[i] = i;
            }

            // Mélanger le tableau en utilisant le mélange de Fisher-Yates
            for (int i = arraySize - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                // Échanger les éléments i et j
                (indexes[j], indexes[i]) = (indexes[i], indexes[j]);
            }

            // Retourner les N premiers éléments du tableau mélangé
            int[] result = new int[n];
            Array.Copy(indexes, result, n);

            return result;
        }
    }
}