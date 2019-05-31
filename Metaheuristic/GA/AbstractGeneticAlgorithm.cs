

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
        public bool WithLogs = true;



        #region Members

        //Parameters
        protected int populationSize;
        protected double mutationChance = 0.05d;
        protected int maxSteps;

        protected int elitism = 0;
        protected int newBlood = 0;

        
        protected Random rand = RandomSingleton.Instance.GetNewSeededRandom();


        //Calculations
        protected T[] population;
        protected int sumFitnesses;

        //Other
        protected List<T> trackBests = new List<T>();
        protected GALogs<T> logs;

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

            if (elitism + newBlood >= populationSize)
            {
                throw new InvalidGAException("There can't be more elitism and new blood than population size!");
            }
            
        }

        public T Run()
        {
            trackBests = new List<T>();

            if (Verbose)
            {
                Console.WriteLine("Generation #0...");
            }

            InitializePopulation();
            ComputePopulationFitnesses(true);

            if (WithLogs)
            {
                logs = new GALogs<T>();
                logs.AddStep(population);
                if ( Verbose )
                    Console.WriteLine(logs.Last.ToString());
            }

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

                int newChildEndIndex = populationSize - newBlood;

                //Elitism : We keep the first #elitism best ones.
                for (int i = 0; i < elitism; i++)
                {
                    newPopulation[i] = population[i];
                }

                //Make a child for all the population.
                for (int i = elitism; i < newChildEndIndex; i += 2)
                {
                    T[] childs = Crossover(SelectParents());

                    newPopulation[i] = TryMutate(childs[0]);

                    //Test in case the population size is odd
                    if (i + 1 == newChildEndIndex)
                    {
                        break;
                    }

                    newPopulation[i + 1] = TryMutate(childs[1]);
                }

                //We generate #newBlood new individuals that are not made from parents. (New Blood)
                for (int i = newChildEndIndex; i < populationSize; i++)
                {
                    newPopulation[i] = GenerateIndividual();
                }

                population = newPopulation;

                //New Fitness
                ComputePopulationFitnesses();

                if (WithLogs)
                {
                    logs.AddStep(population);
                    if (Verbose)
                        Console.WriteLine(logs.Last.ToString());
                }

                step++;

            }
            if (WithLogs)
            {
                logs.AddFinalLog();
                if (Verbose)
                    Console.WriteLine("#Final Log :" + logs.FinalLog.ToStringFinalLog());
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


        protected void ComputePopulationFitnesses(bool isFirstGen = false)
        {
            //If it's the first gen, we compute all Fitness. Else we skip the elites ones we already know.
            int startIndex = isFirstGen ? 0 : elitism;

            int sumFitnesses = 0;
            for (int i = startIndex; i < populationSize; i++)
            {
                population[i].Fitness = GetFitness(population[i]);
                sumFitnesses += population[i].Fitness;
            }

            //Sort by Score (Thanks LINQ)
            population = population.OrderBy(individual => individual.Fitness).ToArray();

            //sumFitnesses = population.Sum(individual => individual.Fitness);
            //Make it return best score?

            trackBests.Add(BestIndividual);

            if (Verbose)
            {
                //PrintTopFitnesses(5);
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
            
            T[] parents = KTournamentSelection(4);
            

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

        protected T GetRandomInvidivualFromPopulation() {
            return population[rand.Next(0, population.Length)];
        }

        private T[] KTournamentSelection(int K)
        {

            T[] parents = new T[2];

            for (int i = 0; i < parents.Length; i++)
            {

                //We pick K individuals at random
                List<T> challengers = new List<T>();
                for (int j = 0; j < K; j++)
                {
                    challengers.Add(GetRandomInvidivualFromPopulation());
                }

                //find minFitness
                int minFitness = challengers.Min(individual => individual.Fitness);

                //We keep the one with the best fitness.
                parents[i] = challengers.First(individual => individual.Fitness == minFitness);
            }

            return parents;
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
