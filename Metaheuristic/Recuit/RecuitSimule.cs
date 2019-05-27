
namespace Metaheuristic.Recuit
{
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;


    class RecuitSimule
    {

        public bool Verbose = true;
        private QuadratricAssignment qap;
        
        private Random rnd = RandomSingleton.Instance.GetNewSeededRandom();

        private RecuitLogs logs;

        public int N
        {
            get => qap.N;
        }
        public RecuitLogs Logs { get => logs; }

        public RecuitSimule(QuadratricAssignment problem)
        {
            this.qap = problem;
        }
   
        public QuadraticAssignmentSolution Execute(QuadraticAssignmentSolution initialSol, 
                                                    double initialTemp, 
                                                    double temperatureDecrease, 
                                                    int maxSteps, 
                                                    int nbNeighborPerStep)
        {
            //setup
            QuadraticAssignmentSolution current = initialSol;

            //Bests
            QuadraticAssignmentSolution best = current;
            int minFitness = qap.Evaluate(current);

            //Parameters
            double temperature = initialTemp;

            //Init logs
            logs = new RecuitLogs();
            logs.AddStep(current, best, temperature);

            if (Verbose)
            {
                Console.WriteLine("\n---- RECUIT SIMULE :");
                Console.WriteLine("S0 : " + initialSol.ToString());
                Console.WriteLine("F0 : " + minFitness);
                Console.WriteLine("T0 : " + initialTemp);
                Console.WriteLine("---- Parameters :");
                Console.WriteLine("uTemp : " + temperatureDecrease);
                Console.WriteLine("maxSteps : " + maxSteps);
                Console.WriteLine("nbNeighborPerStep : " + nbNeighborPerStep);
            }

            for (int i = 0; i < maxSteps; i++)
            {
                if (Verbose)
                {
                    Console.WriteLine("Step #" + (i + 1));
                }
                    //Try X neighbors at that temperature
                    for (int j = 0; j < nbNeighborPerStep; j++)
                {

                    //Randomly get a neighbor
                    QuadraticAssignmentSolution neighbor = current.GetRandomNeighbor();

                    //Check if better than CURRENT solution
                    int neighborFitness = qap.Evaluate(neighbor);
                    int diffFitness = neighborFitness - minFitness;

                    //If it's better, we keep it
                    if (diffFitness <= 0)
                    {
                        current = neighbor;

                        //If it's better than ALL solution
                        if (neighborFitness < minFitness)
                        {
                            best = neighbor;
                            minFitness = neighborFitness;
                        }
                    }
                    else
                    {
                        //There's a random chance to keep it anyway
                        double p = rnd.NextDouble(); //rand in [0,1[

                        //I haven't chose that formula
                        if (p <= Math.Exp(-diffFitness / temperature))
                        {
                            current = neighbor;
                        }

                    }
                }

                //Temperature decrease
                temperature = temperatureDecrease * temperature;


                logs.AddStep(current, best, temperature);
            }

            logs.AddFinalLog();

            if (Verbose)
            {
                Console.WriteLine("---- Meilleur résultat :");
                Console.WriteLine(best.ToString());
                Console.WriteLine(minFitness);
                Console.WriteLine("---- FIN");
            }

            return best;
        }

        public QuadraticAssignmentSolution Execute(RecuitSimuleParameters parameters)
        {
            return Execute(parameters.InitialSol,
                        parameters.InitialTemp,
                        parameters.TemperatureDecrease,
                        parameters.MaxSteps,
                        parameters.NbNeighborPerStep);
        }
    }

        
}
