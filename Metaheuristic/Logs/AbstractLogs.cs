using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Metaheuristic.QAP;
using System.Linq;

namespace Metaheuristic.Logs
{
    public abstract class AbstractLogs : ILogs
    {
        protected List<AbstractLog> logs = new List<AbstractLog>();
        protected AbstractFinalLog finalLog;

        protected Stopwatch stopWatch = Stopwatch.StartNew();

        #region Properties

        #region Interface ILogs

        public List<ILog> Logs
        {
            get => logs.Cast<ILog>().ToList(); //ugly?
        }

        public ILog FinalLog
        {
            get => finalLog;
        }
        public int Size
        {
            get => logs.Count;
        }
        #endregion

        public AbstractLog this[int index]
        {
            get => logs[index];
        }
        
        public AbstractLog Previous
        {
            get => logs[logs.Count - 1];
        }
        #endregion


        protected void AddFinal(AbstractFinalLog final)
        {

            //Best
            final.Best = Previous.Best;
            final.BestFitness = Previous.BestFitness;

            //Improvements
            final.BestImprovement = GetImprovement(logs[0].BestFitness, Previous.BestFitness);
            final.ImprovementChart = GetImprovementChart();

            //Time
            final.timeEllapsed = stopWatch.ElapsedMilliseconds;
            stopWatch.Stop();


            //save it
            finalLog = final;
        }


        public abstract void AddFinalLog();

        public void SaveLogsTo(string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(logs);
            }
        }
        
        public static double GetImprovement(int lastFitness, int newFitness)
        {
            return Math.Round((1d - (double)newFitness / (double)lastFitness) * 100, 4);
        }

        public static double GetRelativeError(int bestFoundFitness, int bestKnownFitness)
        {
            double absoluteError = Math.Abs(bestFoundFitness - bestKnownFitness);
            return Math.Round( (absoluteError / bestKnownFitness) * 100, 4);
        }

        protected Improvement GetImproved(int lastFitness, int newFitness) 
        {
            if (lastFitness == newFitness)
                return Improvement.Stagnated;
            if (lastFitness > newFitness)
                return Improvement.Worsened;
            else
                return Improvement.Improved;
        }

        protected string GetImprovementChart()
        {
            string str = "";
            for (int i = 1; i < logs.Count; i++)
            {
                switch (logs[i].HasImproved)
                {
                    case Improvement.Stagnated:
                        str += "o";
                        break;
                    case Improvement.Worsened:
                        str += "-"; //worse
                        break;
                    case Improvement.Improved:
                        str += "+"; //better
                        break;
                }
            }

            return str;
        }

        public abstract class AbstractLog : ILog
        {
            [Name("Step")]
            public int Step { get; set; }

            [Name("Has Improved")]
            public Improvement HasImproved { get; set; }
            

            //Best
            public QuadraticAssignmentSolution Best;

            [Name("Best Solution")]
            public string BestString { get; set; }

            [Name("Best Fitness")]
            public int BestFitness { get; set; }

            [Name("Best HasImproved")]
            public double BestImprovement { get; set; }

            [Name("Time Ellapsed")]
            public double TimeEllapsed { get; set; }

            public override string ToString()
            {
                string str = "";
                str += "\nStep #" + Step;
                str += "\nExecution Time : " + TimeEllapsed + "ms";
                str += BestToString();
                return str;
            }

            protected string BestToString()
            {
                string str = "";
                str += "\nBest : " + BestString;
                str += "\nBest Fitness : " + BestFitness;
                str += "\nBest's Improvement : " + BestImprovement + "%";
                return str;
            }
        }
        
        

        public abstract class AbstractFinalLog : ILog
        {
            //Best found and improvements
            public QuadraticAssignmentSolution Best;
            public int BestFitness;
            public double BestImprovement;

            public string ImprovementChart;

            public double timeEllapsed;

            public override string ToString()
            {
                string str = "";
                str += ImprovementToString();
                str += BestToString();
                str += "\nExecution Time : " + timeEllapsed + "ms";
                

                return str;
            }

            protected string ImprovementToString()
            {
                string str = "";
                str += "\nImprovement chart : " + ImprovementChart;
                str += "\nImprovement from initial : " + BestImprovement + "%";
                return str;
            }

            protected string BestToString()
            {
                string str = "";
                str += "\nBest Solution : " + Best.ToString();
                str += "\nBest Fitness : " + BestFitness;
                return str;
            }
        }


        public enum Improvement
        {
            Improved, Stagnated, Worsened
        }
    }
}
