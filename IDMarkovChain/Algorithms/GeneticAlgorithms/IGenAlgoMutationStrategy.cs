namespace IDMarkovChain.Algorithms.GeneticAlgorithms
{
    /// <summary>
    /// Interface pour une stratégie de mutation d'un individu durant l'algorithme génétique
    /// </summary>
    /// <typeparam name="TIndividualEncoding">Le type d'encodage</typeparam>
    interface IGenAlgoMutationStrategy<TIndividualEncoding>
    {
        /// <summary>
        /// La fonction principale effectuant mutation
        /// </summary>
        /// <param name="individual">L'individu sur laquelle éffectuer la mutation</param>
        /// <param name="mutationRate">Le taux de mutation</param>
        /// <returns></returns>
        public IGenAlgoIndividual<TIndividualEncoding> Mutate(
            IGenAlgoIndividual<TIndividualEncoding> individual,
            double mutationRate
        );
    }
}