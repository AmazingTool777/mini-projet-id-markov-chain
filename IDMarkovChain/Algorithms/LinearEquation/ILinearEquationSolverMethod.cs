namespace IDMarkovChain.Algorithms.LinearEquation
{
    public interface ILinearEquationSolverMethod
    {
        /// <summary>
        /// Résout un système d'équations linéaires en utilisant une stratégie de résolution.
        /// </summary>
        /// <param name="matrix">Matrice de coefficients du système d'équations</param>
        /// <param name="constants">Valeurs constantes du système d'équations</param>
        /// <returns>Les valeurs des inconnues</returns>
        public double[] Solve(double[,] matrix, double[] constants);
    }
}