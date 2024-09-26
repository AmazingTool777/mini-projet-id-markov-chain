namespace IDMarkovChain.Algorithms.GeneticAlgorithms
{
    /// <summary>
    /// Interface pour un individu dans l'algorithme génétique
    /// </summary>
    /// <typeparam name="TIndividualEncoding">Le type d'encodage de l'individu</typeparam>
    interface IGenAlgoIndividual<TIndividualEncoding>
    {
        /// <summary>
        /// Retourne l'encodage de l'individu
        /// </summary>
        /// <returns>L'encodage de l'individu</returns>
        public TIndividualEncoding GetEncoding();

        /// <summary>
        /// Met à jour l'encodage de l'individu
        /// </summary>
        /// <returns>L'encodage de l'individu</returns>
        public IGenAlgoIndividual<TIndividualEncoding> SetEncoding(TIndividualEncoding encoding);

        /// <summary>
        /// Retourne le score de fitness de l'individu
        /// </summary>
        /// <returns>Le score de fitness</returns>
        public abstract double GetFitnessScore();

        /// <summary>
        /// Décrit l'individu sur le console
        /// </summary>
        public abstract void Describe();
    }
}