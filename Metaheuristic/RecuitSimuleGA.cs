
namespace Metaheuristic
{

    using Metaheuristic.GA;
    using Metaheuristic.QAP;
    using System;

    class GeneticAlgorithmRecuit : AbstractGeneticAlgorithm<RecuitSimuleParameters>
    {

        public RecuitSimule problem;

        private Random rand = new Random();

        public GeneticAlgorithmRecuit(RecuitSimule problem, 
                                    int populationSize,
                                    int maxSteps,
                                    double mutationChance)
        {
            this.problem = problem;
            this.problem.Verbose = false;
            this.populationSize = populationSize;
            this.maxSteps = maxSteps;
            this.mutationChance = mutationChance;
        }

        protected override RecuitSimuleParameters[] Crossover(RecuitSimuleParameters parent1, RecuitSimuleParameters parent2)
        {
            RecuitSimuleParameters[] childs = new RecuitSimuleParameters[2];
            childs[0] = new RecuitSimuleParameters(parent1);
            childs[1] = new RecuitSimuleParameters(parent1);

            //For each parameters, we flip a coin to know from which parent each child heritate their.
            //If a child doesn't have a parent parameter, the second child will.

            //In this case, each child is a copy of parent1, and we pick randomly which one will get a parameter
            //replace by parent2's.

            int flip = rand.Next(0, 2);
            childs[flip].InitialSol = parent2.InitialSol;

            flip = rand.Next(0, 2);
            childs[flip].InitialTemp = parent2.InitialTemp;

            flip = rand.Next(0, 2);
            childs[flip].TemperatureDecrease = parent2.TemperatureDecrease;

            flip = rand.Next(0, 2);
            childs[flip].MaxSteps = parent2.MaxSteps;

            flip = rand.Next(0, 2);
            childs[flip].NbNeighborPerStep = parent2.NbNeighborPerStep;

            return childs;

        }

        protected override RecuitSimuleParameters GenerateIndividual()
        {
            //Get a random individual
            return new RecuitSimuleParameters(problem.N);
        }

        protected override int GetFitness(RecuitSimuleParameters solution)
        {
            //Return the fitness of the best solution from the algorithm executon
            return problem.Execute(solution).Fitness;
        }

        protected override RecuitSimuleParameters Mutate(RecuitSimuleParameters child)
        {

            //We choose a random parameter to reset mutate
            int choice = rand.Next(0, 5);
            switch (choice) {
                case 0:
                    child.InitialSol = RecuitSimuleParameters.GetRandomInitialSol(problem.N);
                    break;
                case 1:
                    child.InitialTemp = RecuitSimuleParameters.GetRandomInitialTemp();
                    break;
                case 2:
                    child.TemperatureDecrease = RecuitSimuleParameters.GetRandomTemperatureDecrease();
                    break;
                case 3:
                    child.MaxSteps = RecuitSimuleParameters.GetRandomMaxStep();
                    break;
                case 4:
                    child.NbNeighborPerStep = RecuitSimuleParameters.GetRandomNeighborStep();
                    break;
            }

            return child;
        }
    }
}
