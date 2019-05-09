

namespace Metaheuristic
{
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public class GeneticAlgorithm
    {
        private const bool VERBOSE = true;

        #region Members

        //Parameters
        private int n;
        private QuadratricAssignment problem;
        private int populationSize;

        private double mutationChance = 0.05d;
        private int maxSteps;


        //TODO : TO SEED
        private Random rand = new Random();

        //Calculations
        private QuadraticAssignmentSolution[] population;
        private int sumFitnesses;

        //Other
        private List<QuadraticAssignmentSolution> trackBests = new List<QuadraticAssignmentSolution>();

        #endregion

        #region Properties
        private QuadraticAssignmentSolution BestIndividual
        {
            get => population[0];
        }
        #endregion

        public GeneticAlgorithm(QuadratricAssignment problem, int populationSize)
        {
            this.problem = problem;
            this.n = problem.N;
            this.populationSize = populationSize;
        }

        private int GetFitness(QuadraticAssignmentSolution solution)
        {
            return problem.Evaluate(solution);
        }

        public QuadraticAssignmentSolution Run(int steps)
        {
            this.maxSteps = steps;
            trackBests = new List<QuadraticAssignmentSolution>();

            InitializePopulation();
            ComputePopulationFitnesses();

            int step = 0;
            //While no termintation criteria
            while (step < maxSteps)
            {
                if (VERBOSE)
                {
                    Console.WriteLine("Step #" + (step+1));
                }
                //Construct a new population (to enhance)

                QuadraticAssignmentSolution[] newPopulation = new QuadraticAssignmentSolution[populationSize];

                //Make a child for all the population.
                for (int i = 0; i < populationSize; i += 2)
                {
                    QuadraticAssignmentSolution[] childs = Crossover(SelectParents());
                    newPopulation[i] = TryMutate(childs[0]);

                    //Test in case the population size is odd
                    if (i != populationSize)
                    {
                        newPopulation[i + 1] = TryMutate(childs[1]);
                    }
                }

                population = newPopulation;

                //New Fitness
                ComputePopulationFitnesses();

                step++;

            }

            if (VERBOSE)
            {
                PrintBestHistory();
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

            trackBests.Add(BestIndividual);

            if (VERBOSE)
            {
                PrintTopFitnesses(5);
            }
        }

        private QuadraticAssignmentSolution[] Crossover(QuadraticAssignmentSolution[] parents)
        {
            return Crossover(parents[0], parents[1]);
        }

        private QuadraticAssignmentSolution[] Crossover(QuadraticAssignmentSolution parent1, QuadraticAssignmentSolution parent2)
        {

            QuadraticAssignmentSolution[] childs = new QuadraticAssignmentSolution[2];
            childs[0] = parent1 * parent2;
            childs[1] = parent2 * parent1;

            return childs;
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
            
            for (int i = 0; i < 2; i++)
            {
                int partialSumFitnesses = 0;
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

        private QuadraticAssignmentSolution TryMutate(QuadraticAssignmentSolution child)
        {
            double roll = rand.NextDouble();

            if (roll < mutationChance)
            {
                child.ApplyInversion(QuadraticAssignmentSolution.GetRandomPermutation(n));
            }

            return child;
        }

        private void PrintTopFitnesses(int top)
        {
            Console.WriteLine("--- FITNESSES");
            int lenght = Math.Min(top, populationSize);
            for (int i = 0; i < lenght; i++)
            {
                Console.WriteLine("#" + (i + 1) + " : " + population[i].Score);
            }
            Console.WriteLine("----");
        }
        private void PrintFitnesses()
        {
            Console.WriteLine("--- FITNESSES");
            for (int i = 0; i < populationSize; i++)
            {
                Console.WriteLine("#" + (i + 1) + " : " + population[i].Score);
            }
            Console.WriteLine("----");
        }

        private void PrintBestHistory()
        {
            Console.WriteLine("--- BEST");
            foreach (QuadraticAssignmentSolution best in trackBests)
            {
                Console.WriteLine(best.ToString() + " : " + best.Score);
            }
            Console.WriteLine("----");
        }
    }
}
