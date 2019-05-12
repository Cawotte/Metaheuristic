using Metaheuristic.QAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic
{
    class RecuitSimule
    {

        public bool Verbose = true;
        private QuadratricAssignment qap;

        private float temperatureDecrease;
        private int maxSteps;
        private int nbNeighborPerStep;

        private Random rnd = new Random();

        public RecuitSimule(QuadratricAssignment problem, float temperatureDecrease, int maxSteps, int nbNeighborPerStep)
        {
            this.qap = problem;
            this.temperatureDecrease = temperatureDecrease;
            this.maxSteps = maxSteps;
            this.nbNeighborPerStep = nbNeighborPerStep;
        }
   
        public QuadraticAssignmentSolution Execute(QuadraticAssignmentSolution initialSol, float initialTemp)
        {
            //setup
            QuadraticAssignmentSolution current = initialSol;

            //Bests
            QuadraticAssignmentSolution best = current;
            int minFitness = qap.Evaluate(current);

            //Parameters
            float temperature = initialTemp;

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
            }

            if (Verbose)
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
