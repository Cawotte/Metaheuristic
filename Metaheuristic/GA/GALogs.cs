﻿

namespace Metaheuristic.GA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public class GALogs<T> where T : IChromosome
    {
        private List<LogStep<T>> logs = new List<LogStep<T>>();
        private LogStep<T> finalLog;

        private static bool ExtendedLog = false; //Only set true for debugging

        public LogStep<T> Last
        {
            get => logs[logs.Count - 1];
        }

        public LogStep<T> this[int index] {
            get => logs[index];
        }

        public LogStep<T> FinalLog
        {
            get => finalLog;
        }

        public int Size
        {
            get => logs.Count;
        }

        public void AddStep(T[] population)
        {
            LogStep<T> step = new LogStep<T>();

            step.Step = logs.Count;

            //Diversity and AverageFitness
            int average = 0;
            HashSet<int> uniqueFitnesses = new HashSet<int>(); //Two different invidivuals are two indivivuals with different fitnesses (not really true)

            for (int i = 0; i < population.Length; i++)
            {
                average += population[i].Fitness;
                uniqueFitnesses.Add(population[i].Fitness);
            }

            average = (int)((double)average / (double)population.Length);
            step.AverageFitness = average;

            step.Diversity = Math.Round(((double)uniqueFitnesses.Count / (double)population.Length) * 100d, 2);

            step.Best = population[0];
            step.BestFitness = population[0].Fitness;

            //Improvement
            //If it's the first step, we init it.
            if (logs.Count == 0)
            {
                step.Improvement = 0d;
                step.BestImprovement = 0d;
            }
            else
            {
                step.Improvement = GetImprovement(Last.AverageFitness, step.AverageFitness);
                step.BestImprovement = GetImprovement(Last.BestFitness, step.BestFitness);
            }

            if (ExtendedLog)
            {
                step.Population = population;
            }
            //Add to logs
            logs.Add(step);
        }

        public void AddFinalLog()
        {
            LogStep<T> final = new LogStep<T>();

            //Best Individual
            final.BestFitness = Last.BestFitness; //logs.Max(step => step.BestFitness);
            final.Best = Last.Best;

            //Improvement between first and best
            final.BestImprovement = GetImprovement(logs[0].BestFitness, Last.BestFitness);

            //Average improvement
            final.Improvement = Math.Round(logs.Average(step => step.Improvement), 4);

            //Average Diversity
            final.Diversity = Math.Round(logs.Average(step => step.Diversity), 2);

            //Average Fitness
            final.AverageFitness = (int)logs.Average(step => step.AverageFitness);

            final.Step = Size + 1; //Not really useful

            //save it
            finalLog = final;

        }
        

        private double GetImprovement(int lastFitness, int newFitness) {
            return Math.Round( (1d - (double)newFitness / (double)lastFitness) * 100, 4);
        }

        public struct LogStep<T> where T : IChromosome
        {
            public int Step;
            public int AverageFitness;
            public double Diversity;
            public double Improvement;

            public T Best;
            public int BestFitness;
            public double BestImprovement;

            //Used for debugging
            public T[] Population;

            public override string ToString() {
                string str = "";
                str += "\nStep #" + Step;
                str += "\nAverage Fitness : " + AverageFitness;
                str += "\nDiversity : " + Diversity +"%";
                str += "\nImprovement : " + Improvement + "%";
                str += "\nBest Fitness : " + BestFitness;
                str += "\nBest Improvement : " + BestImprovement + "%";
                str += "\n";

                return str;
            }

            public string ToStringFinalLog()
            {
                string str = "";
                str += "\nBest Fitness : " + BestFitness;
                str += "\nAverage GA Fitness : " + AverageFitness;
                str += "\nAverage Diversity : " + Diversity + "%";
                str += "\nAverage Improvement : " + Improvement + "%";
                str += "\nImprovement between initial and best : " + BestImprovement + "%";
                str += "\n";

                return str;
            }
        }
    }

}