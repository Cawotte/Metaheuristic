using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.QAP
{
    interface IQuadratricAssignment
    {
        /**
         * 
         * w(a,b) = Poids entre les equipements a et b.
         * f(a) = Emplacement de l'équipement a.
         * d( f(a), f(b) ) = Distance entre les emplacements des equipements a et b.
         * */

        /// <summary>
        /// The N size of the problem : the number of facilities and locations.
        /// </summary>
        /// <returns></returns>
        int GetN();

        int GetDistance(int a, int b);

        int GetWeight(int a, int b);

        /// <summary>
        /// Compute the value of a solution. 
        /// </summary>
        /// <param name="solution">Must be a SET of size N with integers between 1 and N. </param>
        /// <returns></returns>
        int EvaluateSolution(int[] solution);

    }
}
