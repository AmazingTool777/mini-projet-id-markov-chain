using IDMarkovChain.Algorithms.GeneticAlgorithms;

namespace IDMarkovChain.Context
{
    /// <summary>
    /// Classe implémentant l'algorithme génétique pour la recherche de la politique de décisions
    /// minimisant les coût moyens dans notre problème de management.
    /// </summary>
    class MinMeanCostGeneticAlgorithms : GeneticAlgorithmsSkeleton<int[]>
    {
        public MinMeanCostGeneticAlgorithms() : base(
            crossOverStrategy: new MinMeanCostGenAlgoNaturalCrossOver(),
            mutationStrategy: new MinMeanCostGenAlgoMutation(),
            populationSize: 100,
            fitnessAscending: true,
            fitnessPlateauIterationsSolutionWindow: 4
        )
        {
            SelectionStrategy = new ElitismSelection<int[]>(fitnessAscending: true);
        }

        /// <summary>
        /// Implémentation de la fonction générant un individu (politique de décision) aléatoire
        /// </summary>
        /// <returns>La politique de décision créée</returns>
        public override IGenAlgoIndividual<int[]> GenerateRandomIndividual()
        {
            int statesCount = ProblemContext.HypotheticalActions[0].TransitionMatrix.GetLength(0);
            int actionsCount = ProblemContext.ActionsNamesLabels.Length;
            int[] encoding = new int[statesCount];

            Random rand = new();
            for (int i = 0; i < statesCount; i++)
            {
                encoding[i] = rand.Next(actionsCount);
            }
            ActionsPolicy policy = new(encoding);

            return policy;
        }
    }

    /// <summary>
    /// Stratégie de croisement utilisée par l'algorithme génétique pour notre problème de minimisation des coûts moyens
    /// </summary>
    class MinMeanCostGenAlgoNaturalCrossOver : ICrossOverStrategy<int[]>
    {
        public IGenAlgoIndividual<int[]>[] Mate(IGenAlgoIndividual<int[]> parent1, IGenAlgoIndividual<int[]> parent2)
        {
            int length = parent1.GetEncoding().Length;
            Random rand = new();
            int crossIndice = rand.Next(1, length - 1); // Indice aléatoire de croisement
            IGenAlgoIndividual<int[]>[] children = new IGenAlgoIndividual<int[]>[2];

            // Croisement parent 1 -> parent 2
            int[] child1Encoding = (int[])parent1.GetEncoding().Clone();
            Array.Copy(parent2.GetEncoding(), crossIndice, child1Encoding, crossIndice, length - crossIndice);
            children[0] = new ActionsPolicy(child1Encoding);
            // Croisement parent 2 -> parent 1
            int[] child2Encoding = (int[])parent2.GetEncoding().Clone();
            Array.Copy(parent1.GetEncoding(), crossIndice, child2Encoding, crossIndice, length - crossIndice);
            children[1] = new ActionsPolicy(child2Encoding);

            return children;
        }
    }

    /// <summary>
    /// Stratégie de mutation utilisée par l'algorithme génétique pour notre problème de minimisation des coûts moyens
    /// </summary>
    class MinMeanCostGenAlgoMutation : IGenAlgoMutationStrategy<int[]>
    {
        public IGenAlgoIndividual<int[]> Mutate(IGenAlgoIndividual<int[]> policy, double mutationRate)
        {
            int[] encoding = policy.GetEncoding(); // Encodage de la politique
            int encodingLength = encoding.Length; // Nombre d'états formant la politiques
            int actionsCount = ProblemContext.ActionsNamesLabels.Length; // Le nombre d'actions possibles

            int[] mutated = new int[encodingLength]; // Initialisation de l'encodage après mutation
            Array.Copy(policy.GetEncoding(), mutated, encodingLength);

            Random random = new();
            // Pour tous les états formant la politique
            for (int i = 0; i < encodingLength; i++)
            {
                if (random.NextDouble() < mutationRate) // Si nombre aléatoire < taux de mutation
                {
                    // Remplacer l'action à cet état par un autre état aléatoire
                    int newValue;
                    do
                    {
                        newValue = random.Next(0, actionsCount);
                    } while (newValue == mutated[i]);

                    mutated[i] = newValue;
                }
            }
            // Mettre à jour l'encodage (éventuellement) modifiée par la mutation
            policy.SetEncoding(mutated);

            return policy;
        }
    }
}