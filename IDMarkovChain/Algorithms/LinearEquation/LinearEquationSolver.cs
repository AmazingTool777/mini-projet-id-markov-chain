using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.LinearEquation
{
    public static class LinearEquationSolver
    {
        /// <summary>
        /// Résout un système d'équations linéaires en utilisant une stratégie de résolution.
        /// </summary>
        /// <param name="matrix">Matrice de coefficients du système d'équations</param>
        /// <param name="constants">Valeurs constantes du système d'équations</param>
        /// <param name="method">Stratégie de résolution du système d'équations</param>
        /// <returns>Les valeurs des inconnues</returns>
        public static double[] Solve(double[,] matrix, double[] constants, ILinearEquationMethodStrategy? method = null)
        {
            method ??= new GaussEliminationMethod();
            return method.Solve(matrix, constants);
        }
    }
}