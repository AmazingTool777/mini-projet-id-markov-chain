using IDMarkovChain.Algorithms.GeneticAlgorithms;
using IDMarkovChain.Algorithms.LinearEquation;
using IDMarkovChain.Utils;

namespace IDMarkovChain.Context
{
    class ActionsPolicy(int[] actionsIndices) : IGenAlgoIndividual<int[]>
    {
        // Les indices des actions qui consituent la politique par état
        public int[] ActionsIndices = actionsIndices;

        // La matrice de transition de la politique de décision
        public double[,] TransitionMatrix = InferTransitionMatrix(actionsIndices);

        // Le tableau des coûts par état de la politique de décision
        public int[] Costs = InferCosts(actionsIndices);

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
        static double[,] InferTransitionMatrix(int[] actionsIndices)
        {
            int statesCount = actionsIndices.Length;
            double[,] matrix = new double[statesCount, statesCount];

            for (int i = 0; i < statesCount; i++)
            {
                MarkovChainAction stateAction = ProblemContext.Actions[actionsIndices[i]];

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
        static int[] InferCosts(int[] actionsIndices)
        {
            int statesCount = actionsIndices.Length;
            int[] costs = new int[actionsIndices.Length];

            for (int i = 0; i < statesCount; i++)
            {
                costs[i] = ProblemContext.CostsTable[i, actionsIndices[i]];
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

        /// <summary>
        /// Décrit la politique de décision sur console.
        /// </summary>
        public void Describe()
        {
            int statesCount = ProblemContext.HypotheticalActions[0].TransitionMatrix.GetLength(0);
            Console.WriteLine("---- Matrice de transitions");
            MatrixUtils.PrintMatrix(TransitionMatrix, 14);
            Console.WriteLine("---- Probabilités des états à l'état stationnaire");
            for (int j = 0; j < statesCount; j++)
            {
                Console.WriteLine($"¶{j} = {StationaryProbabilities[j]}");
            }
            string isNormalizedText = HasNormalizedStationaryStateProbabilities() ? "OUI" : "NON";
            Console.WriteLine($"Contrainte de normalisation vérifiée: {isNormalizedText}");
            Console.WriteLine("---- Actions à chaque état");
            for (int i = 0; i < statesCount; i++)
            {
                Console.WriteLine($"Etat {i} => Action {ActionsIndices[i]}");
            }
            Console.WriteLine("---- Coûts à chaque état");
            for (int i = 0; i < statesCount; i++)
            {
                Console.WriteLine($"Etat {i}: {Costs[i]}");
            }
            Console.WriteLine($"Coût moyen E(C) = {MeanCost}");
        }

        /// <summary>
        /// Implémenation du Getter de l'encodage de la politque de décision
        /// en tant qu'individu dans l'algorithme génétique
        /// </summary>
        /// <returns>L'encodage</returns>
        public int[] GetEncoding()
        {
            return ActionsIndices;
        }

        /// <summary>
        /// Implémenation du Setter de l'encodage de la politque de décision
        /// en tant qu'individu dans l'algorithme génétique
        /// </summary>
        /// <param name="encoding">Le nouveau encodage</param>
        /// <returns></returns>
        public IGenAlgoIndividual<int[]> SetEncoding(int[] encoding)
        {
            // Mise à jour de l'encodage
            ActionsIndices = encoding;
            // Réinitialisation des autres attributs dépendants sur l'encodage
            TransitionMatrix = InferTransitionMatrix(ActionsIndices);
            Costs = InferCosts(ActionsIndices);
            stationaryProbabilities = null;
            meanCost = null;
            // Retourne la référence sur la politique de décision
            return this;
        }

        /// <summary>
        /// Implémenation du Getter du score de fitness en tant qu'individu dans l'algorithme génétique.
        /// Retourne le coût moyen dans notre cas.
        /// </summary>
        /// <returns>Le score de fitness qui est le coût moyen.</returns>
        public double GetFitnessScore()
        {
            return MeanCost;
        }
    }
}