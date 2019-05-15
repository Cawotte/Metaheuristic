

namespace Metaheuristic
{
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public class GeneticAlgorithm : AbstractGeneticAlgorithm<QuadraticAssignmentSolution>
    {
        #region Members

        //Parameters
        private int n;
        private QuadratricAssignment problem;

        #endregion

        public GeneticAlgorithm(QuadratricAssignment problem, int populationSize)
        {
            this.problem = problem;
            this.n = problem.N;
            this.populationSize = populationSize;
        }

        protected override int GetFitness(QuadraticAssignmentSolution solution)
        {
            return problem.Evaluate(solution);
        }
        
        protected override QuadraticAssignmentSolution[] Crossover(QuadraticAssignmentSolution parent1, QuadraticAssignmentSolution parent2)
        {

            QuadraticAssignmentSolution[] childs = new QuadraticAssignmentSolution[2];
            childs[0] = parent1 * parent2;
            childs[1] = parent2 * parent1;

            return childs;
        }

        protected override QuadraticAssignmentSolution GenerateIndividual()
        {
            return new QuadraticAssignmentSolution(n);
        }

        protected override QuadraticAssignmentSolution Mutate(QuadraticAssignmentSolution child)
        {
            return child.ApplyInversion(QuadraticAssignmentSolution.GetRandomInversion(n));
        }
    }
}
