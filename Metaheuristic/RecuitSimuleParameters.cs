

namespace Metaheuristic
{
    using Metaheuristic.GA;
    using Metaheuristic.QAP;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsuled parameters for the Recuit Simulé algorithm. Useful for Genetic Algorithm.
    /// </summary>
    class RecuitSimuleParameters : IChromosome
    {

        static Random rand = new Random();

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

        public static QuadraticAssignmentSolution GetRandomInitialSol(int n)
        {
            return new QuadraticAssignmentSolution(n);
        }

        public static double GetRandomInitialTemp()
        {
            return rand.NextDouble() * 100000d;
        }

        public static double GetRandomTemperatureDecrease()
        {
            return rand.NextDouble();
        }

        public static int GetRandomMaxStep()
        {
            return rand.Next(10, 10000);
        }

        public static int GetRandomNeighborStep()
        {
            return rand.Next(5, 25);
        }
        #endregion
    }
}
