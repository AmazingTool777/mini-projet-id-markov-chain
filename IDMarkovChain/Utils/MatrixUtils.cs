namespace IDMarkovChain.Utils
{
    public static class MatrixUtils
    {
        // Affiche une matrice (tableau multi-dimensionnel à 2 dimensions) d'une manière formattée
        public static void PrintMatrix(float[,] matrix, int columnWidth = 5)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j].ToString().PadLeft(columnWidth));
                }
                Console.WriteLine();
            }
        }
    }
}