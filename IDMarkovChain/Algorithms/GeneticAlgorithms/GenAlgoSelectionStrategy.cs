using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.GeneticAlgorithms
{
    /// <summary>
    /// Interface pour une stratégie de séléction d'individus à reproduire durant l'algorithme génétique
    /// </summary>
    /// <typeparam name="TIndividualEncoding">Le type d'encodage d'un individu</typeparam>
    interface IGenAlgoSelectionStrategy<TIndividualEncoding>
    {
        /// <summary>
        /// La fonction principale effectuant la séléction.
        /// </summary>
        /// <param name="individuals">La piscine d'individus sur laquelle éffectuer la séléction.</param>
        /// <param name="selectionRate">Le pourcentage d'individus à séléctionner</param>
        /// <returns>Les individus séléctionnés</returns>
        public List<IGenAlgoIndividual<TIndividualEncoding>> Select(
            IGenAlgoIndividual<TIndividualEncoding>[] individuals,
            double selectionRate = 0.5
        );
    }

    /// <summary>
    /// Stratégie de séléction par élitisme
    /// </summary>
    /// <typeparam name="TIndividualEncoding">Le type d'encodage d'un individu</typeparam>
    /// <param name="fitnessAscending">Si trier les individus par ordre croissant du fitness ou non.</param>
    class ElitismSelection<TIndividualEncoding>(bool fitnessAscending = false) : IGenAlgoSelectionStrategy<TIndividualEncoding>
    {
        // Vrai si trier les individus par ordre croissant du fitness
        public readonly bool FITNESS_ASCENDING = fitnessAscending;

        public List<IGenAlgoIndividual<TIndividualEncoding>> Select(
            IGenAlgoIndividual<TIndividualEncoding>[] individuals,
            double selectionRate = 0.5
        )
        {
            int selectedCount = (int)Math.Floor(individuals.Length * selectionRate);
            int sortFactor = FITNESS_ASCENDING ? 1 : -1;
            // Selection des `selectedCount` premiers individus triés en fonction du fitness
            return [.. individuals.ToList().OrderBy(i => sortFactor * i.GetFitnessScore()).Take(selectedCount)];
        }
    }

    class TournamentSelection<TIndividualEncoding>(int tournamentSize = 8) : IGenAlgoSelectionStrategy<TIndividualEncoding>
    {
        public int TournamentSize = tournamentSize;

        public List<IGenAlgoIndividual<TIndividualEncoding>> Select(
            IGenAlgoIndividual<TIndividualEncoding>[] individuals,
            double selectionRate
        )
        {
            int individualsCount = individuals.Length;
            int selectedCount = (int)Math.Floor(individualsCount * selectionRate);
            List<IGenAlgoIndividual<TIndividualEncoding>> selectedIndividuals = [];
            List<IGenAlgoIndividual<TIndividualEncoding>> remainingIndividuals = [.. individuals];

            for (int i = 0; i < selectedCount; i++)
            {
                int[] opponentsIndices = ArrayUtils.GetRandomUniqueIndices(individualsCount - i, TournamentSize);
                List<IGenAlgoIndividual<TIndividualEncoding>> opponents = ArrayUtils.GetElementsAtIndices(
                    remainingIndividuals.ToArray(),
                    opponentsIndices
                );
                double maxFitnessScore = double.MinValue;
                int selectedIndice = -1;
                for (int j = 0; j < TournamentSize; j++)
                {
                    double opponentFitnessScore = opponents[j].GetFitnessScore();
                    if (opponentFitnessScore > maxFitnessScore)
                    {
                        maxFitnessScore = opponentFitnessScore;
                        selectedIndice = opponentsIndices[j];
                    }
                }
                selectedIndividuals.Add(remainingIndividuals[selectedIndice]);
                remainingIndividuals.RemoveAt(selectedIndice);
            }

            return selectedIndividuals;
        }
    }
}