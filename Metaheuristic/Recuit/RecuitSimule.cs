
namespace Metaheuristic.Recuit
{
    using Metaheuristic.Logs;
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;


    public class RecuitSimule : IQAPSolver
    {

        //General
        private bool verbose = true;
        private QuadraticAssignment qap;

        private Random rand = RandomSingleton.Instance.Rand;

        private RecuitLogs logs;

        #region Properties

        public ILogs Logs { get => logs; }

        public bool Verbose
        {
            get; set;
        }
        public int N
        {
            get => qap.N;
        }

        #endregion

        public RecuitSimule(QuadraticAssignment problem)
        {
            this.qap = problem;
        }


        public QuadraticAssignmentSolution Run(ISolverParameters param)
        {
            RecuitSimuleParameters paramRec;

            if (param is RecuitSimuleParameters)
            {
                paramRec = (RecuitSimuleParameters)param;
            }
            else
            {
                throw new BadTypeException();
            }

            return Run(paramRec.InitialSol,
                        paramRec.InitialTemp,
                        paramRec.TemperatureDecrease,
                        paramRec.MaxSteps);
        }

        public QuadraticAssignmentSolution Run(QuadraticAssignmentSolution initialSol, 
                                                    double initialTemp, 
                                                    double temperatureDecrease, 
                                                    int maxSteps)
        {
            rand = RandomSingleton.Instance.Rand;
            
            //setup
            QuadraticAssignmentSolution current = initialSol;

            //Bests
            QuadraticAssignmentSolution best = current;
            int minFitness = qap.Evaluate(current);

            //Parameters
            double temperature = initialTemp;
            int stagnantCount = 0;

            //Init logs
            logs = new RecuitLogs();
            logs.AddStep(current, best, temperature);

            if (verbose)
            {
                Console.WriteLine("\n---- RECUIT SIMULE :");
                Console.WriteLine("S0 : " + initialSol.ToString());
                Console.WriteLine("F0 : " + minFitness);
                Console.WriteLine("T0 : " + initialTemp);
                Console.WriteLine("---- Parameters :");
                Console.WriteLine("uTemp : " + temperatureDecrease);
                Console.WriteLine("maxSteps : " + maxSteps);
                //Console.WriteLine("nbNeighborPerStep : " + nbNeighborPerStep);
            }

            for (int i = 0; i < maxSteps; i++)
            {
                if (verbose)
                {
                    Console.WriteLine("Step #" + (i + 1));
                }

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
                    stagnantCount = 0;
                }
                else
                {
                    //There's a random chance to keep it anyway
                    double p = rand.NextDouble(); //rand in [0,1[

                    //I haven't chose that formula
                    if (p <= Math.Exp(-diffFitness / temperature))
                    {
                        current = neighbor;

                        stagnantCount = 0;
                    }
                    else
                    {
                        stagnantCount++;
                    }
                        
                }

                //Temperature decrease
                temperature = temperatureDecrease * temperature;
                
                logs.AddStep(current, best, temperature);

                //Additional terminal condition : If Temperature is extremely cold
                if (temperature < 1d || stagnantCount >= 40)
                {
                    if (verbose)
                        Console.WriteLine("Temperature/Changes is very low ! Algorithm has been stopped early. Step = " + i);
                    break;
                }
            }

            logs.AddFinalLog();
            if (verbose)
            {
                Console.WriteLine("---- Meilleur résultat :");
                Console.WriteLine(best.ToString());
                Console.WriteLine(minFitness);
                Console.WriteLine("---- FIN");
            }

            return best;
        }
    }

        
}
