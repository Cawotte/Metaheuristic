using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic
{
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
