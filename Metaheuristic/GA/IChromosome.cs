
namespace Metaheuristic.GA
{

    /// <summary>
    /// Interface implemented by classes that can be used for Genetic Algorithms.
    /// </summary>
    public interface IChromosome
    {

        /// <summary>
        /// Return the Fitness, need to be set first.
        /// </summary>
        int Fitness
        {
            get;
            set;
        }
        
    }
}
