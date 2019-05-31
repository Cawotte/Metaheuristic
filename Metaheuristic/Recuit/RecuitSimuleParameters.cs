

namespace Metaheuristic.Recuit
{
    using Metaheuristic.GA;
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsuled parameters for the Recuit Simulé algorithm. Useful for Genetic Algorithm.
    /// </summary>
    public class RecuitSimuleParameters : IChromosome
    {

        private Random rand = RandomSingleton.Instance.GetNewSeededRandom();

        //Parameters
        public QuadraticAssignmentSolution InitialSol;
        public double InitialTemp;
        public double TemperatureDecrease;
        public int MaxSteps;
        public int NbNeighborPerStep;

        //Others

        private int fitness = -1;
        public int Fitness { get => fitness; set => fitness = value; }

        #region Constructors


        public RecuitSimuleParameters(QuadraticAssignmentSolution initialSol,
                                                    double initialTemp,
                                                    double temperatureDecrease,
                                                    int maxSteps,
                                                    int nbNeighborPerStep)
        {
            this.InitialSol = initialSol;
            this.InitialTemp = initialTemp;
            this.TemperatureDecrease = temperatureDecrease;
            this.MaxSteps = maxSteps;
            this.NbNeighborPerStep = nbNeighborPerStep;
            
        }

        /// <summary>
        /// Copy the given parameters values
        /// </summary>
        /// <param name="param"></param>
        public RecuitSimuleParameters(RecuitSimuleParameters param)
        {
            this.InitialSol = param.InitialSol;
            this.InitialTemp = param.InitialTemp;
            this.TemperatureDecrease = param.TemperatureDecrease;
            this.MaxSteps = param.MaxSteps;
            this.NbNeighborPerStep = param.NbNeighborPerStep;
            
        }

        /// <summary>
        /// Get random parameters
        /// </summary>
        /// <param name="n"></param>
        public RecuitSimuleParameters(int n)
        {

            this.InitialSol = GetRandomInitialSol(n);
            this.InitialTemp = GetRandomInitialTemp();
            this.TemperatureDecrease = GetRandomTemperatureDecrease();
            this.MaxSteps = GetRandomMaxStep();
            this.NbNeighborPerStep = GetRandomNeighborStep();
        }


        #endregion
        public QuadraticAssignmentSolution GetRandomInitialSol(int n)
        {
            return new QuadraticAssignmentSolution(n);
        }

        public double GetRandomInitialTemp()
        {
            return rand.NextDouble() * 100000d;
        }

        public double GetRandomTemperatureDecrease()
        {
            return rand.NextDouble();
        }

        public int GetRandomMaxStep()
        {
            return rand.Next(10, 2000);
        }

        public int GetRandomNeighborStep()
        {
            return rand.Next(5, 25);
        }




        public override string ToString()
        {
            string str = "";
            str += "\nN : " + InitialSol.N;
            str += "\nS0 : " + InitialSol.ToString();
            str += "\nT0 : " + String.Format("{0:0.00}", InitialTemp);
            str += "\nTemp_Decrease : " + String.Format("{0:0.00}", TemperatureDecrease);
            str += "\nMaxSteps : " + MaxSteps;
            str += "\nNeighbors per Step : " + NbNeighborPerStep;
            str += "\n";

            return str;
        }
    }
}
