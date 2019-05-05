

namespace Metaheuristic
{
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    class GeneticAlgorithm
    {

        private int n;
        private QuadratricAssignment problem;

        private int populationSize;
        private QuadraticAssignmentSolution[] population;
        private int[] fitnesses;
        private int sumFitnesses;

        private float mutationChance;

        private int maxSteps;

        //TODO : TO SEED
        private Random rand = new Random();

        private QuadraticAssignmentSolution BestIndividual {
            get => population[0];
        }

        private int GetFitness(QuadraticAssignmentSolution solution)
        {
            return problem.Evaluate(solution);
        }

        private QuadraticAssignmentSolution Run()
        {
            InitializePopulation();
            ComputePopulationFitnesses();

            int step = 0;
            while (step < maxSteps)
            {

            }

            return BestIndividual;
            /*
             * initialize population
                find fitness of population
   
                while (termination criteria is reached) do
                    parent selection
                    crossover with probability pc
                    mutation with probability pm
                    decode and fitness calculation
                    survivor selection
                    find best
                return best
             * 
             * */
        }

        private void InitializePopulation()
        {
            population = new QuadraticAssignmentSolution[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                //New random population
                population[i] = new QuadraticAssignmentSolution(n);

            }
        }

        private void ComputePopulationFitnesses()
        {
            for (int i = 0; i < populationSize; i++)
            {
                problem.Evaluate(population[i]); //(Scores are assigned in the Evaluate method)
            
            }

            //Sort by Score (Thanks LINQ)
            population.OrderByDescending(individual => individual.Score);

            sumFitnesses = population.Sum(individual => individual.Score);
            //Make it return best score?
        }

        /// <summary>
        /// Select two parents from the population, given different selection methods (to implement)
        /// </summary>
        /// <returns></returns>
        private QuadraticAssignmentSolution[] SelectParents()
        {

            /// TODO : Have different population selection algorithms possible.
            /// 

            QuadraticAssignmentSolution[] parents = new QuadraticAssignmentSolution[2];

            //Roulette Wheel Selection
            
            int partialSumFitnesses = 0;
            for (int i = 0; i < 2; i++)
            {
                //Spin the wheel
                int randValue = rand.Next(0, sumFitnesses);

                //Search the parent on the "wheel"
                int index = 0;
                while (partialSumFitnesses < sumFitnesses) {
                    partialSumFitnesses += population[index++].Score;
                }

                parents[i] = population[index-1]; // -1 to cancel the previous last increment from the loop
            }

            return parents;
        }
    }
}
