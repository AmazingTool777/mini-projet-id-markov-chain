using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.LinearEquation
{
    /// <summary>
    /// Classe permettant de resoudre l'équation lineaire utilisant la méthode d'elimination de Gauss.
    /// Implémente l'interface d'une stratégie de résolution d'un système d'équations linéaires
    /// </summary>
    public class GaussEliminationMethod : ILinearEquationMethodStrategy
    {
        /// <summary>
        /// Résout un problème d'équations linéaires
        /// </summary>
        /// <param name="matrix">La matrice des coefficients</param>
        /// <param name="constants">Le vecteur les constantes au 2nd membre</param>
        /// <returns>Le vecteur des solutions</returns>
        public double[] Solve(double[,] matrix, double[] constants)
        {
            int n = constants.Length;
            for (int i = 0; i < n - 1; i++)
            {
                double maxElement = Math.Abs(matrix[i, i]);
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(matrix[k, i]) > maxElement)
                    {
                        maxElement = Math.Abs(matrix[k, i]);
                        maxRow = k;
                    }
                }

                for (int k = i; k < n; k++)
                {
                    double temp = matrix[maxRow, k];
                    matrix[maxRow, k] = matrix[i, k];
                    matrix[i, k] = temp;
                }
                double tempConstant = constants[maxRow];
                constants[maxRow] = constants[i];
                constants[i] = tempConstant;

                for (int k = i + 1; k < n; k++)
                {
                    double factor = matrix[k, i] / matrix[i, i];
                    for (int j = i; j < n; j++)
                    {
                        matrix[k, j] -= factor * matrix[i, j];
                    }
                    constants[k] -= factor * constants[i];
                }
            }

            /// substitution backward
            double[] result = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                result[i] = constants[i] / matrix[i, i];
                for (int k = i - 1; k >= 0; k--)
                {
                    constants[k] -= matrix[k, i] * result[i];
                }
            }
            return result;
        }
    }
}