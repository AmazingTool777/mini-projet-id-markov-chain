namespace IDMarkovChain.Utils
{
    public static class MatrixUtils
    {
        // Affiche une matrice (tableau multi-dimensionnel à 2 dimensions) d'une manière formattée
        public static void PrintMatrix(double[,] matrix, int columnWidth = 5)
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

        // Aggrégation d'une liste de matrices en une nouvelle matrice moyenne
        public static double[,] AverageMatrices(List<double[,]> matrices)
        {
            // Dimensions de la matrice
            int[] matrixSize = [matrices[0].GetLength(0), matrices[0].GetLength(1)];
            // Initialisation de la matrice moyenne
            double[,] avgMatrix = new double[matrixSize[0], matrixSize[1]];

            // Somme des valeurs dans chaque matrice
            for (int experiments = 0; experiments < matrices.Count; experiments++)
            {
                double[,] matrix = matrices[experiments];
                for (int i = 0; i < matrixSize[0]; i++)
                {
                    for (int j = 0; j < matrixSize[1]; j++)
                    {
                        avgMatrix[i, j] += matrix[i, j];
                    }
                }
            }
            // Rapport entre les sommes obtenues et le nombre de matrices dans la liste
            // afin de finaliser les valeurs moyennes
            for (int i = 0; i < matrixSize[0]; i++)
            {
                for (int j = 0; j < matrixSize[1]; j++)
                {
                    avgMatrix[i, j] /= matrices.Count;
                }
            }

            return avgMatrix;
        }

        /// <summary>
        /// Retourne la transposée d'une matrice
        /// </summary>
        /// <param name="matrix">La matrice donnée</param>
        /// <returns>La matrice transposée</returns>
        public static double[,] Transpose(double[,] matrix)
        {
            int[] matrixSize = [matrix.GetLength(0), matrix.GetLength(1)];
            double[,] transposeMatrix = new double[matrixSize[1], matrixSize[0]];

            for (int i = 0; i < matrixSize[1]; i++)
            {
                for (int j = 0; j < matrixSize[0]; j++)
                {
                    transposeMatrix[i, j] = matrix[j, i];
                }
            }

            return transposeMatrix;
        }
    }
}