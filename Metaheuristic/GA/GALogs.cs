

namespace Metaheuristic.GA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    using CsvHelper;
    using CsvHelper.Configuration.Attributes;
    using System.IO;

    public class GALogs<T> where T : IChromosome
    {
        private List<LogStep<T>> logs = new List<LogStep<T>>();
        private FinalLogStep<T> finalLog;

        private static bool ExtendedLog = false; //Only set true for debugging

        public LogStep<T> Last
        {
            get => logs[logs.Count - 1];
        }

        public LogStep<T> this[int index] {
            get => logs[index];
        }

        public FinalLogStep<T> FinalLog
        {
            get => finalLog;
        }

        public int Size
        {
            get => logs.Count;
        }

        public void AddStep(T[] population, T best)
        {
            LogStep<T> step = new LogStep<T>();

            step.Step = logs.Count;

            //Diversity and AverageFitness
            long average = 0;
            HashSet<int> uniqueFitnesses = new HashSet<int>(); //Two different invidivuals are two indivivuals with different fitnesses (not really true)

            for (int i = 0; i < population.Length; i++)
            {
                average += population[i].Fitness;
                uniqueFitnesses.Add(population[i].Fitness);
            }

            average = (long)((double)average / (double)population.Length);
            step.AverageFitness = (int)average;

            step.Diversity = Math.Round(((double)uniqueFitnesses.Count / (double)population.Length) * 100d, 2);

            step.Best = best;
            step.BestFitness = best.Fitness;
            step.BestString = best.ToString();

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

        public void AddFinalLog(double timeEllapsed)
        {
            FinalLogStep<T> final = new FinalLogStep<T>();

            //Best Individual
            final.BestFitness = Last.BestFitness; //logs.Max(step => step.BestFitness);
            final.Best = Last.Best;

            //Improvement between first and best
            final.BestImprovement = GetImprovement(logs[0].BestFitness, Last.BestFitness);

            //Average improvement
            final.AverageImprovement = Math.Round(logs.Average(step => step.Improvement), 4);

            //Average Diversity
            final.AverageDiversity = Math.Round(logs.Average(step => step.Diversity), 2);

            //Average Fitness
            final.AverageFitness = (int)logs.Average(step => step.AverageFitness);


            final.timeEllapsed = timeEllapsed;

            //save it
            finalLog = final;

        }

        public void SaveLogsTo(string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(logs);
            }
        }

        private double GetImprovement(int lastFitness, int newFitness) {
            return Math.Round( (1d - (double)newFitness / (double)lastFitness) * 100, 4);
        }

        public struct LogStep<T> where T : IChromosome
        {
            [Name("Step")]
            public int Step { get; set; }

            [Name("Average Fitness")]
            public int AverageFitness { get; set; }
            [Name("Diversity")]
            public double Diversity { get; set; }
            [Name("Improvement")]
            public double Improvement { get; set; }
            
            public T Best;

            [Name("Best")]
            public String BestString { get; set; }

            [Name("Best Fitness")]
            public int BestFitness { get; set; }
            [Name("Best Improvement")]
            public double BestImprovement { get; set; }

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
            
        }

        public struct FinalLogStep<T> where T : IChromosome
        {
            public int AverageFitness;
            public double AverageDiversity;
            public double AverageImprovement;

            public T Best;
            public int BestFitness;
            public double BestImprovement;

            public double timeEllapsed;


            public override string ToString()
            {
                string str = "";
                str += "\nBest Solution : " + Best.ToString();
                str += "\nBest Fitness : " + BestFitness;
                str += "\nAverage GA Fitness : " + AverageFitness;
                str += "\nAverage Diversity : " + AverageDiversity + "%";
                str += "\nAverage Improvement : " + AverageImprovement + "%";
                str += "\nImprovement between initial and best : " + BestImprovement + "%";
                str += "\nExecution Time : " + timeEllapsed + "ms";
                str += "\n";

                return str;
            }
        }
    }

}
