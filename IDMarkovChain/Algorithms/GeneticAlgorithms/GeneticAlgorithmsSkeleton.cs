using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.GeneticAlgorithms
{
    /// <summary>
    /// Classe abstraite définissant 
    /// </summary>
    /// <typeparam name="TIndividualEncoding">Type d'encodage de l'individu</typeparam>
    abstract class GeneticAlgorithmsSkeleton<TIndividualEncoding>(
        ICrossOverStrategy<TIndividualEncoding> crossOverStrategy,
        IGenAlgoMutationStrategy<TIndividualEncoding> mutationStrategy,
        int populationSize = 100,
        bool fitnessAscending = false,
        double fitnessPlateauTolerance = 0.001,
        int fitnessPlateauIterationsSolutionWindow = 4,
        int maxIterations = 100,
        double mutationRate = 0.1
    )
    {
        /// <summary>
        /// Le nombre d'individus (population) utilisés lors de l'algorithme à chaque génération
        /// </summary>
        public readonly int POPULATION_SIZE = populationSize;

        /// <summary>
        /// Si le classement des individus se fait par ordre ascendent du score de fitness ou non.
        /// </summary>
        public readonly bool FITNESS_ASCENDING = fitnessAscending;

        /// <summary>
        /// Facteur de tri du score de fitness.
        /// Si ascendant alors 1, Sinon -1
        /// </summary>
        int sortFactor { get => FITNESS_ASCENDING ? 1 : -1; }

        /// <summary>
        /// Stratégie de séléction des individus les plus fit
        /// </summary>
        public IGenAlgoSelectionStrategy<TIndividualEncoding> SelectionStrategy = new TournamentSelection<TIndividualEncoding>();

        /// <summary>
        /// Stratégie de croisement de 2 individus
        /// </summary>
        public ICrossOverStrategy<TIndividualEncoding> CrossOverStrategy = crossOverStrategy;

        /// <summary>
        /// Stratégie de mutation d'un individu tout nouvellement créé
        /// </summary>
        public IGenAlgoMutationStrategy<TIndividualEncoding> MutationStrategy = mutationStrategy;

        /// <summary>
        /// Taux de mutation d'un individu tout nouvellement créé.
        /// Probabilité généralement très faible.
        /// </summary>
        public readonly double MUTATION_RATE = mutationRate;

        /// <summary>
        /// Tolérance en dessous de laquelle une variation de score de fitness est considérée comme plateau
        /// </summary>
        public readonly double FITNESS_PLATEAU_TOLERANCE = fitnessPlateauTolerance;

        /// <summary>
        /// Une nombre d'itérations en tant que fenêtre dans laquelle si les meilleurs score de fitness restent plateau,
        /// on arrête l'algorithme en prenant l'individu le plus fit à ce point comme solution
        /// </summary>
        public readonly int FITNESS_PLATEAU_ITERATIONS_SOLUTION_WINDOW = fitnessPlateauIterationsSolutionWindow;

        /// <summary>
        /// Le nombre maximum d'itérations jusqu'à l'arrêt de l'algorithme
        /// si la solution la plus optimale n'a pas été trouvée jusqu'à cela
        /// </summary>
        public readonly int MAX_ITERATIONS = maxIterations;

        /// <summary>
        /// Génère un individu aléatoire.
        /// </summary>
        /// <returns>L'individu aléatoire</returns>
        public abstract IGenAlgoIndividual<TIndividualEncoding> GenerateRandomIndividual();

        /// <summary>
        /// Génère la population initiale selon la taille définie par `POPULATION_SIZE`
        /// </summary>
        /// <returns>La population initiale</returns>
        IGenAlgoIndividual<TIndividualEncoding>[] GenerateInitialPopulation()
        {
            IGenAlgoIndividual<TIndividualEncoding>[] individuals = new IGenAlgoIndividual<TIndividualEncoding>[POPULATION_SIZE];
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                individuals[i] = GenerateRandomIndividual();
            }
            return individuals;
        }

        /// <summary>
        /// Reproduction entre les individus donnés
        /// </summary>
        /// <param name="individuals">Les individus à reproduire</param>
        /// <returns>Les nouveaux individus créés</returns>
        IGenAlgoIndividual<TIndividualEncoding>[] Repopulate(List<IGenAlgoIndividual<TIndividualEncoding>> individuals)
        {
            List<IGenAlgoIndividual<TIndividualEncoding>> nextIndividuals = [.. individuals]; // Nouvelle population
            while (individuals.Count > 0) // Tant qu'il y a des individus à reproduire
            {
                // Selection des 2 parents
                IGenAlgoIndividual<TIndividualEncoding>[] parents = new IGenAlgoIndividual<TIndividualEncoding>[2];
                Random rand = new();
                for (int i = 0; i < 2; i++)
                {
                    int parentIndice = rand.Next(individuals.Count);
                    parents[i] = individuals[parentIndice];
                    individuals.RemoveAt(parentIndice); // Enlever le parent parmis les non-accouplés
                    individuals.TrimExcess();
                }
                // Croisement
                IGenAlgoIndividual<TIndividualEncoding>[] children = CrossOverStrategy.Mate(
                    parents[0],
                    parents[1]
                );
                // Mutation & Ajout des descendants créés
                foreach (IGenAlgoIndividual<TIndividualEncoding> child in children)
                {
                    // Mutation
                    MutationStrategy.Mutate(child, MUTATION_RATE);
                    // Ajout des descendants créés
                    nextIndividuals.Add(child);
                }
            }
            return [.. nextIndividuals];
        }

        /// <summary>
        /// Calcule la variation du score de fitness entre 2 individus
        /// </summary>
        /// <param name="ind1">Le premier individu</param>
        /// <param name="ind2">Le deuxième individu</param>
        /// <returns>La variation</returns>
        static double GetFitnessScoreVariation(IGenAlgoIndividual<TIndividualEncoding> ind1, IGenAlgoIndividual<TIndividualEncoding> ind2)
        {
            return Math.Abs(ind1.GetFitnessScore() - ind2.GetFitnessScore());
        }

        /// <summary>
        /// Vérifie si un individu est plus fit qu'un autre individu.
        /// Elle dépend du facteur de tri.
        /// </summary>
        /// <param name="ind">L'individu à comparer</param>
        /// <param name="comparedTo">L'autre individu pris comme comparaison</param>
        /// <returns>Si l'individu est plus fit ou non</returns>
        bool IsFitterThan(IGenAlgoIndividual<TIndividualEncoding> ind, IGenAlgoIndividual<TIndividualEncoding> comparedTo)
        {
            return sortFactor * ind.GetFitnessScore() < sortFactor * comparedTo.GetFitnessScore();
        }

        /// <summary>
        /// Vérifie si un individu est la solution optimale de l'algorithme.
        /// A être redéfinie par les classes filles si nécéssaire.
        /// </summary>
        /// <param name="individual">L'individu à vérifier</param>
        /// <returns>Si l'individu est la solution optimale de l'algorithme ou non</returns>
        public virtual bool SolutionIsFound(IGenAlgoIndividual<TIndividualEncoding> individual)
        {
            return false;
        }

        /// <summary>
        /// Fonction principale qui éxécute l'algorithme génétique
        /// </summary>
        /// <returns>L'individu solution</returns>
        public IGenAlgoIndividual<TIndividualEncoding> Run()
        {
            // Initialisation des individus
            IGenAlgoIndividual<TIndividualEncoding>[] individuals = GenerateInitialPopulation();

            // Variable stockant l'individu le plus fit de la génération actuelle
            IGenAlgoIndividual<TIndividualEncoding> currentFittestIndividual = individuals
                .ToList()
                .OrderBy(i => sortFactor * i.GetFitnessScore())
                .First();
            // Variable stockant l'individu le plus fit depuis le début de l'algorithme
            IGenAlgoIndividual<TIndividualEncoding> fittestIndividual = currentFittestIndividual;
            // L'itération la plus récente à laquelle
            // une grande variation du score de fitness des individus les plus fit s'est produite
            int latestFitnessScoreBigVariationIteration = 0;

            // Répéter la reproduction jusqu'au max itérations ou solution trouvé
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                // Selection des individus à reproduire pour la génération suivante
                List<IGenAlgoIndividual<TIndividualEncoding>> selectedIndividuals = SelectionStrategy.Select(individuals);

                // Reproduction des individus séléctionnés pour obtenir les nouveaux individus de la génération suivante
                IGenAlgoIndividual<TIndividualEncoding>[] nextIndividuals = Repopulate(selectedIndividuals);

                // L'individu le plus fit de la génération actuelle
                currentFittestIndividual = nextIndividuals.ToList().OrderBy(i => sortFactor * i.GetFitnessScore()).First();

                // Arrêter si la solution est trouvée
                if (SolutionIsFound(currentFittestIndividual)) break;

                // Calcul de la variation du score de fitness
                // entre l'individu le plus fit de la génération actuelle et l'individu le plus fit depuis le début
                double fittestScoreVariation = GetFitnessScoreVariation(currentFittestIndividual, fittestIndividual);

                // Si la variation du score de fitness des individus les plus fit est plus grande que la tolérance,
                // on marque l'itération de cette grande variation
                if (fittestScoreVariation >= FITNESS_PLATEAU_TOLERANCE)
                {
                    latestFitnessScoreBigVariationIteration = i;
                }
                // Sinon Si la variation du score de fitness est inférieure à la tolérance,
                // et le nombre d'itérations succéssives écoulées depuis une grande variation du score de fitness des individus les plus fit
                // est inférieur à la fenêtre d'itérations de plateau du score de fitness, alors on arrête l'algorithme
                else if (i - latestFitnessScoreBigVariationIteration > FITNESS_PLATEAU_ITERATIONS_SOLUTION_WINDOW) break;

                // Mise à jour de l'individu le plus fit jamais trouvé à ce point
                if (IsFitterThan(currentFittestIndividual, fittestIndividual))
                {
                    fittestIndividual = currentFittestIndividual;
                }

                // Union des individus séléctionnés dans la génération précédente avec les nouveaux individus de la génération actuelle
                // pour former les individus finaux de la génération actuelle
                individuals = [.. selectedIndividuals.Concat(nextIndividuals)];
            }

            return fittestIndividual;
        }
    }
}