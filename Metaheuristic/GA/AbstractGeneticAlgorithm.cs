

namespace Metaheuristic.GA
{
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public abstract class AbstractGeneticAlgorithm<T> where T : IChromosome
    {
        public bool Verbose = true;

        #region Members

        //Parameters
        protected int populationSize;
        protected double mutationChance = 0.05d;
        protected int maxSteps;

        protected int elitism = 0; //TODO
        protected int newBlood = 0; //TODO


        //TODO : TO SEED
        protected Random rand = new Random();

        //Calculations
        protected T[] population;
        protected int sumFitnesses;

        //Other
        protected List<T> trackBests = new List<T>();

        #endregion

        #region Properties
        protected T BestIndividual
        {
            get => population[0];
        }
        #endregion

        protected AbstractGeneticAlgorithm(
                                    int populationSize,
                                    int maxSteps,
                                    double mutationChance,
                                    int elitism = 0,
                                    int newBlood = 0)
        {
            this.populationSize = populationSize;
            this.maxSteps = maxSteps;
            this.mutationChance = mutationChance;
            this.elitism = elitism;
            this.newBlood = newBlood;
        }

        public T Run()
        {
            trackBests = new List<T>();

            {
                Console.WriteLine("Generation #0...");
            }

            InitializePopulation();
            ComputePopulationFitnesses();

            int step = 0;
            //While no termintation criteria
            while (step < maxSteps)
            {
                if (Verbose)
                {
                    Console.WriteLine("Generation #" + (step+1));
                }

                //Construct a new population (to enhance)
                T[] newPopulation = new T[populationSize];
                
                //Make a child for all the population.
                for (int i = 0; i < populationSize; i += 2)
                {
                    T[] childs = Crossover(SelectParents());
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

            if (Verbose)
            {
                PrintBestHistory();
            }
            return BestIndividual;
            /***
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
             * ***/
        }

        #region TO IMPLEMENT IN CHILDS

        protected abstract int GetFitness(T solution);
        protected abstract T GenerateIndividual();

        protected abstract T Mutate(T child);

        protected abstract T[] Crossover(T parent1, T parent2);

        #endregion

        protected void InitializePopulation()
        {
            population = new T[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                //New random population
                population[i] = GenerateIndividual();
            }
        }


        protected void ComputePopulationFitnesses()
        {
            for (int i = 0; i < populationSize; i++)
            {
                population[i].Fitness = GetFitness(population[i]);
            }

            //Sort by Score (Thanks LINQ)
            population.OrderByDescending(individual => individual.Fitness);

            sumFitnesses = population.Sum(individual => individual.Fitness);
            //Make it return best score?

            trackBests.Add(BestIndividual);

            if (Verbose)
            {
                PrintTopFitnesses(5);
            }
        }

        protected T[] Crossover(T[] parents)
        {
            return Crossover(parents[0], parents[1]);
        }


        /// <summary>
        /// Select two parents from the population, given different selection methods (to implement)
        /// </summary>
        /// <returns></returns>
        protected T[] SelectParents()
        {

            /// TODO : Have different population selection algorithms possible.
            /// 

            T[] parents = new T[2];

            //Roulette Wheel Selection
            
            for (int i = 0; i < 2; i++)
            {
                int partialSumFitnesses = 0;
                //Spin the wheel
                int randValue = rand.Next(0, sumFitnesses);

                //Search the parent on the "wheel"
                int index = 0;
                while (partialSumFitnesses < sumFitnesses) {
                    partialSumFitnesses += population[index++].Fitness;
                }

                parents[i] = population[index-1]; // -1 to cancel the previous last increment from the loop
            }

            return parents;
        }

        protected T TryMutate(T child)
        {
            double roll = rand.NextDouble();

            if (roll < mutationChance)
            {
                child = Mutate(child);
            }

            return child;
        }


        #region Print Info
        protected void PrintTopFitnesses(int top)
        {
            Console.WriteLine("--- FITNESSES");
            int lenght = Math.Min(top, populationSize);
            for (int i = 0; i < lenght; i++)
            {
                Console.WriteLine("#" + (i + 1) + " : " + population[i].Fitness);
            }
            Console.WriteLine("----");
        }
        protected void PrintFitnesses()
        {
            Console.WriteLine("--- FITNESSES");
            for (int i = 0; i < populationSize; i++)
            {
                Console.WriteLine("#" + (i + 1) + " : " + population[i].Fitness);
            }
            Console.WriteLine("----");
        }

        protected void PrintBestHistory()
        {
            Console.WriteLine("--- BEST");
            foreach (T best in trackBests)
            {
                Console.WriteLine(best.ToString() + " : " + best.Fitness);
            }
            Console.WriteLine("----");
        }
        #endregion
    }
}
