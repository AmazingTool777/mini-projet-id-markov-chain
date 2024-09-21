using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.LinearEquation
{
    /// <summary>
    /// Classe permettant de resoudre l'équation lineaire utilisant la méthode d'elimination de Gauss.
    /// Implémente l'interface d'une stratégie de résolution d'un système d'équations linéaires
    /// </summary>
    public class GaussEliminationMethod : ILinearEquationSolverMethod
    {
        /// <summary>
        /// Résout un problème d'équations linéaires
        /// </summary>
        /// <param name="coeffs">La matrice des coefficients</param>
        /// <param name="constants">Le vecteur les constantes au 2nd membre</param>
        /// <returns>Le vecteur des solutions</returns>
        public double[] Solve(double[,] coeffs, double[] constants)
        {
            int n = constants.Length;
            int m = coeffs.GetLength(1);

            // Elimination successives des coefficients suivant la diagonale
            for (int i = 0; i < m; i++)
            {
                // Recherche de la ligne avec le plus grand coefficient sur la colonne actuelle
                double maxElement = Math.Abs(coeffs[i, i]);
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(coeffs[k, i]) > maxElement)
                    {
                        maxElement = Math.Abs(coeffs[k, i]);
                        maxRow = k;
                    }
                }
                // Permutation de la ligne actuelle avec la ligne ayant le coefficient le plus grand
                // afin de prendre cette ligne en tant que ligne pivot
                for (int k = i; k < m; k++)
                {
                    (coeffs[i, k], coeffs[maxRow, k]) = (coeffs[maxRow, k], coeffs[i, k]);
                }
                (constants[i], constants[maxRow]) = (constants[maxRow], constants[i]);

                // Elimination des coefficients
                for (int k = i + 1; k < n; k++)
                {
                    double factor = coeffs[k, i] / coeffs[i, i]; // Facteur d'elimination par la ligne pivot
                    for (int j = i; j < m; j++)
                    {
                        coeffs[k, j] -= factor * coeffs[i, j];
                    }
                    constants[k] -= factor * constants[i];
                }
            }

            // Retrosubstitution
            double[] result = new double[m];
            for (int i = m - 1; i >= 0; i--)
            {
                result[i] = constants[i] / coeffs[i, i];
                for (int k = i - 1; k >= 0; k--)
                {
                    constants[k] -= coeffs[k, i] * result[i];
                }
            }

            return result;
        }
    }
}