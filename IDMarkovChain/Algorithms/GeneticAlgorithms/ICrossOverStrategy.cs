namespace IDMarkovChain.Algorithms.GeneticAlgorithms
{
    /// <summary>
    /// Interface pour une stratégie de croisement durant l'algorithme génétique
    /// </summary>
    /// <typeparam name="TIndividualEncoding">Le type d'encodage d'un individu</typeparam>
    interface ICrossOverStrategy<TIndividualEncoding>
    {
        /// <summary>
        /// Fonction principale éffectuant le croisement entre 2 individus.
        /// </summary>
        /// <param name="parent1">Le premier individu parent.</param>
        /// <param name="parent2">Le premier individu parent.</param>
        /// <returns>Les nouveaux individus enfants créés.</returns>
        public IGenAlgoIndividual<TIndividualEncoding>[] Mate(
            IGenAlgoIndividual<TIndividualEncoding> parent1,
            IGenAlgoIndividual<TIndividualEncoding> parent2
        );
    }
}