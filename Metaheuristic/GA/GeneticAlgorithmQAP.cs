

namespace Metaheuristic.GA
{
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public class GeneticAlgorithmQAP : AbstractGeneticAlgorithm<QuadraticAssignmentSolution>
    {
        #region Members

        //Parameters
        private int n;
        private QuadraticAssignment problem;

        #endregion

        public GeneticAlgorithmQAP(QuadraticAssignment problem,
                                    int populationSize,
                                    int maxSteps,
                                    double mutationChance,
                                    int elitism = 0,
                                    int newBlood = 0) :
            base(populationSize, maxSteps, mutationChance, elitism, newBlood) //Super constructor
        {
            this.problem = problem;
            this.n = problem.N;
        }

        public GeneticAlgorithmQAP(QuadraticAssignment problem)
        {
            this.problem = problem;
            this.n = problem.N;
        }

        protected override int GetFitness(QuadraticAssignmentSolution solution)
        {
            return problem.Evaluate(solution);
        }
        
        protected override QuadraticAssignmentSolution[] Crossover(QuadraticAssignmentSolution parent1, QuadraticAssignmentSolution parent2)
        {

            QuadraticAssignmentSolution[] childs = new QuadraticAssignmentSolution[2];

            //Crossover, 
            //we randomly shuffle both parents, then we get take care of contradictions

            //They are each a copy of a parent
            childs[0] = parent1.Clone();
            childs[1] = parent2.Clone();

            for (int i = 0; i < n; i++)
            {
                int flip = rand.Next(2);

                //We randomly swap the gene of each child
                if (flip == 1)
                {
                    childs[0].Solution[i] = parent2.Solution[i];
                    childs[1].Solution[i] = parent1.Solution[i];
                }
            }

            //For each child, we fix the contradictions that makes it not a permutation.
            for (int i = 0; i < childs.Length; i++)
            {
                int[] permutation = childs[i].Solution;

                //List to keep track of the unpicked values.
                HashSet<int> possibleValues = problem.Identity.Solution.ToHashSet();
                //List to keep track of the blocking indexes.
                HashSet<int> blockingIndexes = new HashSet<int>();

                //Find mistakes
                for (int j = 0; j < n; j++)
                {
                    bool isRemoved = possibleValues.Remove(permutation[j]);

                    //If we weren't able to remove it, it means it was at least a second occurence,
                    //so a problematic index
                    if (!isRemoved)
                    {
                        blockingIndexes.Add(j);
                    }

                }

                //Fix them, for each problematic indexes, pick a random unpicked value.
                foreach (int index in blockingIndexes) {

                    //Random index to pick in possible values
                    int indexRand = rand.Next(possibleValues.Count);

                    //Put that value in a problematic index in the permutation
                    permutation[index] = possibleValues.ElementAt(indexRand);

                    //Remove that value from the possible values
                    possibleValues.Remove(permutation[index]);

                }

            }
            


            return childs;
        }

        protected override QuadraticAssignmentSolution GenerateIndividual()
        {
            return new QuadraticAssignmentSolution(n);
        }

        protected override QuadraticAssignmentSolution Mutate(QuadraticAssignmentSolution child)
        {
            return child.ApplyInversion(problem.GetRandomInversion());
        }
    }
}
