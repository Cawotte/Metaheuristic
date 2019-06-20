using Metaheuristic.QAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.GA
{
    public class GeneticAlgorithmParameters : ISolverParameters
    {
        public int PopulationSize;
        public int MaxSteps;
        public double MutationChance;
        public int Elitism = 0;
        public int NewBlood = 0;

        public GeneticAlgorithmParameters(int populationSize, int maxSteps, double mutationChance, int elitism, int newBlood)
        {
            this.PopulationSize = populationSize;
            this.MaxSteps = maxSteps;
            this.MutationChance = mutationChance;
            this.Elitism = elitism;
            this.NewBlood = newBlood;
        }

        public override string ToString()
        {

            string str = "";
            str += "\n Population Size : " + PopulationSize;
            str += "\n Number of Generations : " + MaxSteps;
            str += "\n Mutation Chance : " + MutationChance;
            str += "\n Elitism : " + Elitism;
            str += "\n New Blood : " + NewBlood;
            str += "\n";

            return str;
        }
    }
}
