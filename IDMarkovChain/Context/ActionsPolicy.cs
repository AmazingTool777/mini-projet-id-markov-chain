using IDMarkovChain.Algorithms.LinearEquation;
using IDMarkovChain.Utils;

namespace IDMarkovChain.Context
{
    class ActionsPolicy(int[] actionsIndices, MarkovChainAction[] actions, int[,] costsTable)
    {
        // Les indices des actions qui consituent la politique par état
        public int[] ActionsIndices = actionsIndices;

        // La matrice de transition de la politique de décision
        public double[,] TransitionMatrix = InferTransitionMatrix(actionsIndices, actions);

        // Le tableau des coûts par état de la politique de décision
        public int[] Costs = InferCosts(actionsIndices, costsTable);

        // Les probabilités de chaque état à l'état stationnaire pour la politique de décision
        double[]? stationaryProbabilities;
        // Getter pour l'attribut `stationaryProbabilities` en tant que "Singleton"
        public double[] StationaryProbabilities
        {
            get
            {
                stationaryProbabilities ??= CalculateStationaryStateProbabilities();
                return stationaryProbabilities;
            }
        }

        // Le coût moyen de la politique de décision
        double? meanCost;
        // Getter pour l'attribut `meanCost` en tant que "Singleton"
        public double MeanCost
        {
            get
            {
                meanCost ??= CalculateMeanCost();
                return (double)meanCost;
            }
        }

        /// <summary>
        /// Déduit la matrice de transition d'une politique de décision 
        /// à partir des indices des actions de la politique et des données des actions éxistantes.
        /// </summary>
        /// <param name="actionsIndices">Les indices des actions qui constituent la politique par état</param>
        /// <param name="actions">Les actions du modèle</param>
        /// <returns>La matrice de transition de la politique de décision</returns>
        static double[,] InferTransitionMatrix(int[] actionsIndices, MarkovChainAction[] actions)
        {
            int statesCount = actionsIndices.Length;
            double[,] matrix = new double[statesCount, statesCount];

            for (int i = 0; i < statesCount; i++)
            {
                MarkovChainAction stateAction = actions[actionsIndices[i]];

                for (int j = 0; j < statesCount; j++)
                {
                    matrix[i, j] = stateAction.TransitionMatrix[i, j];
                }
            }

            return matrix;
        }

        /// <summary>
        /// Déduit le tableau des couts d'une politique de décision
        /// </summary>
        /// <param name="actionsIndices">Les indices des actions qui constituent la politique par état</param>
        /// <param name="costsTable">Le tableau global des coûts</param>
        /// <returns>Le tableau des couts d'une politique de décision déduit</returns>
        static int[] InferCosts(int[] actionsIndices, int[,] costsTable)
        {
            int statesCount = actionsIndices.Length;
            int[] costs = new int[actionsIndices.Length];

            for (int i = 0; i < statesCount; i++)
            {
                costs[i] = costsTable[i, actionsIndices[i]];
            }

            return costs;
        }

        /// <summary>
        /// Calcule les probabilités des états à l'état stationnaire de la politique de décision
        /// </summary>
        /// <returns></returns>
        double[] CalculateStationaryStateProbabilities()
        {
            int statesCount = ActionsIndices.Length;

            // Initialisation de la matrice des coefficients
            double[,] coeffs = new double[statesCount + 1, statesCount];
            // Copie de la transposée de la matrice de transition vers la matrice des coefficients
            double[,] tTransitionMatrix = MatrixUtils.Transpose(TransitionMatrix);
            for (int i = 0; i < statesCount; i++)
            {
                for (int j = 0; j < statesCount; j++)
                {
                    coeffs[i, j] = tTransitionMatrix[i, j];
                }
            }
            // Soustraction par -1 des valeurs en diagonales afin de sortir
            // l'expression du système d'équations sans 2nd membre pour pouvoir
            // résoudre le système avec la méthode d'élimination de Gauss
            for (int i = 0; i < statesCount; i++)
            {
                coeffs[i, i] -= 1;
            }
            // Les valeurs de la dernière ligne de la matrice de coéfficient est égale à 1
            for (int i = 0; i < statesCount; i++)
            {
                coeffs[statesCount, i] = 1;
            }

            // Initialisation du tableau des coefficients en 2nd membre (0 par défaut)
            double[] consts = new double[statesCount + 1];
            // sauf la dernière ligne qui est égale à 1
            consts[statesCount] = 1;

            // Résolution du système avec la méthode de Gauss
            return LinearEquationSolver.Solve(coeffs, consts);
        }

        /// <summary>
        /// Vérifie si les probabilités stationnaires sont normalisées.
        /// </summary>
        /// <returns>Vrai si les probabilités sont normalisées, faux sinon</returns>
        public bool HasNormalizedStationaryStateProbabilities()
        {
            return StationaryProbabilities.Sum() == 1;
        }

        /// <summary>
        /// Calcule le coût moyen E(C) de la politique de décision
        /// </summary>
        /// <returns>Le coût moyen E(C)</returns>
        double CalculateMeanCost()
        {
            int statesCount = ActionsIndices.Length;
            double meanCost = 0;

            for (int i = 0; i < statesCount; i++)
            {
                meanCost += Costs[i] * StationaryProbabilities[i];
            }

            return meanCost;
        }
    }
}