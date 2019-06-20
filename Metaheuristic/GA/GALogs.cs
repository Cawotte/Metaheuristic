

namespace Metaheuristic.GA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    using CsvHelper;
    using CsvHelper.Configuration.Attributes;
    using System.IO;
    using Metaheuristic.Logs;

    public class GALogs<T> : AbstractLogs where T : IChromosome
    {

        private static bool ExtendedLog = false; //Only set true for debugging

        public void AddStep(T[] population, T best)
        {
            LogGA<T> step = new LogGA<T>();
            LogGA<T> previous = (LogGA<T>)Previous;

            //General
            step.Step = logs.Count;
            step.TimeEllapsed = stopWatch.ElapsedMilliseconds;

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

            //Best
            step.Best = best;
            step.BestFitness = best.Fitness;
            step.BestString = best.ToString();

            //HasImproved
            //If it's the first step, we init it.
            if (logs.Count == 0)
            {
                step.Improvement = 0d;
                step.BestImprovement = 0d;
            }
            else
            {
                step.Improvement = GetImprovement(previous.AverageFitness, step.AverageFitness);
                step.BestImprovement = GetImprovement(previous.BestFitness, step.BestFitness);
            }

            //Debug
            if (ExtendedLog)
            {
                step.Population = population;
            }
            

            //Add to logs
            logs.Add(step);
        }

        public override void AddFinalLog()
        {
            FinalLogGA<T> final = new FinalLogGA<T>();
            LogGA<T> last = (LogGA<T>)Previous;

            //Best Individual
            final.BestFitness = last.BestFitness; //logs.Max(step => step.BestFitness);
            final.Best = last.Best;

            //HasImproved between first and best
            final.BestImprovement = GetImprovement(logs[0].BestFitness, last.BestFitness);

            //Average improvement
            final.AverageImprovement = Math.Round(logs.Average(step => ((LogGA<T>)step).Improvement), 4);

            //Average Diversity
            final.AverageDiversity = Math.Round(logs.Average(step => ((LogGA<T>)step).Diversity), 2);

            //Average Fitness
            final.AverageFitness = (int)logs.Average(step => ((LogGA<T>)step).AverageFitness);


            final.timeEllapsed = stopWatch.ElapsedMilliseconds;
            stopWatch.Stop();

            //save it
            finalLog = final;

        }
        
        

        public class LogGA<T> : AbstractLog where T : IChromosome
        {

            [Name("Improvement")]
            public double Improvement { get; set; }

            [Name("Average Fitness")]
            public int AverageFitness { get; set; }
            [Name("Diversity")]
            public double Diversity { get; set; }
            
            public new T Best;
            

            //Used for debugging
            public T[] Population;

            public override string ToString() {
                string str = "";
                str += "\nStep #" + Step;
                str += "\nAverage Fitness : " + AverageFitness;
                str += "\nDiversity : " + Diversity +"%";
                str += "\nImprovement : " + Improvement + "%";
                str += "\nBest Fitness : " + BestFitness;
                str += "\nBest HasImproved : " + BestImprovement + "%";
                str += "\n";

                return str;
            }
            
        }

        public class FinalLogGA<T> : AbstractFinalLog where T : IChromosome
        {
            public int AverageFitness;
            public double AverageDiversity;
            public double AverageImprovement;

            public new  T Best;
            public override string ToString()
            {
                string str = "";

                str += BestToString();

                //GA Infos
                str += "\nAverage GA Fitness : " + AverageFitness;
                str += "\nAverage Diversity : " + AverageDiversity + "%";
                str += "\nAverage HasImproved : " + AverageImprovement + "%";

                str += ImprovementToString();
                str += "\nExecution Time : " + timeEllapsed + "ms";
                str += "\n";

                return str;
            }
            /*
            public override string ToString()
            {
                string str = "";
                str += "\nBest Solution : " + Best.ToString();
                str += "\nBest Fitness : " + BestFitness;
                str += "\nAverage GA Fitness : " + AverageFitness;
                str += "\nAverage Diversity : " + AverageDiversity + "%";
                str += "\nAverage HasImproved : " + AverageImprovement + "%";
                str += "\nImprovement between initial and best : " + BestImprovement + "%";
                str += "\nExecution Time : " + timeEllapsed + "ms";
                str += "\n";

                return str;
            }*/
        }
    }

}
