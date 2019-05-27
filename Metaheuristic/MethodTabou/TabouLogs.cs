

namespace Metaheuristic.MethodTabou
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using Metaheuristic.QAP;

    public class TabouLogs
    {
        private List<LogTabou> logs = new List<LogTabou>();
        private LastLogTabou finalLog;

        public LogTabou Last
        {
            get => logs[logs.Count - 1];
        }

        public LogTabou this[int index]
        {
            get => logs[index];
        }

        public LastLogTabou FinalLog
        {
            get => finalLog;
        }

        public int Size
        {
            get => logs.Count;
        }

        public void AddStep(QuadraticAssignmentSolution current, QuadraticAssignmentSolution best, int currentTabouSize)
        {
            LogTabou step = new LogTabou();

            step.Step = logs.Count;

            step.Current = current;
            step.CurrentFitness = current.Fitness;

            step.Best = best;
            step.BestFitness = best.Fitness;

            step.CurrentTabouSize = currentTabouSize;

            //If it's not the first step, does uphill and improvements
            if (logs.Count > 0)
            {
                //Uphill depends on if the solution is improving or not. 0 = improving
                step.HasImproved = (current.Fitness < Last.CurrentFitness );

                step.BestImprovement = GetImprovement(Last.BestFitness, best.Fitness);
                step.CurrentImprovement = GetImprovement(Last.CurrentFitness, current.Fitness);
            }
            else
            {
                step.HasImproved = false;
                step.BestImprovement = 0d;
                step.CurrentImprovement = 0d;
            }

            //Add to logs
            logs.Add(step);
        }

        public void AddFinalLog()
        {
            LastLogTabou final = new LastLogTabou();

            //Best Individual
            final.Best = Last.Best;
            final.BestFitness = Last.BestFitness; 

            //Improvement between first and best
            final.BestImprovement = GetImprovement(logs[0].BestFitness, Last.BestFitness);

            final.Downhills = 0;
            final.Uphills = 0;
            for (int i = 1; i < logs.Count; i++)
            {
                //When there's an "hill" change
                if (logs[i].HasImproved != logs[i-1].HasImproved)
                {
                    //If it's starting to improve now, we just finished a downhill
                    if (logs[i].HasImproved)
                    {
                        final.Downhills++;
                    }
                    else
                    {
                        final.Uphills++;
                    }
                }
            }

            final.AverageTabouSize = (int)logs.Average(log => log.CurrentTabouSize);

            //save it
            finalLog = final;

        }

        public string ToStringImprovements()
        {
            string str = "";
            for (int i = 0; i < logs.Count; i++)
            {
                if (logs[i].HasImproved)
                {
                    str += "+";
                }
                else
                {

                    str += "-";
                }
            }

            return str;
        }


        private double GetImprovement(int lastFitness, int newFitness)
        {
            return Math.Round((1d - (double)newFitness / (double)lastFitness) * 100, 4);
        }

        public struct LogTabou
        {
            public int Step;

            public bool HasImproved;

            //current sol
            public QuadraticAssignmentSolution Current;
            public int CurrentFitness;
            public double CurrentImprovement;

            //best known sol
            public QuadraticAssignmentSolution Best;
            public int BestFitness;
            public double BestImprovement;

            //Forbidden list
            public int CurrentTabouSize;
        

            public override string ToString()
            {
                string str = "";
                str += "\nStep #" + Step;
                if (HasImproved)
                {
                    str += "\nCurrent has improved";
                }
                str += "\nCurrent Tabou Size : " + CurrentTabouSize;

                str += "\nCurrent : " + Current.ToString();
                str += "\nCurrent Fitness : " + CurrentFitness;
                str += "\nCurrent Improvement : " + CurrentImprovement + "%";

                str += "\nBest : " + Best.ToString();
                str += "\nBest Fitness : " + BestFitness;
                str += "\nBest Improvement : " + BestImprovement + "%";
                
                str += "\n";

                return str;
            }

        }

        public struct LastLogTabou
        {
            //Best found and improvements
            public QuadraticAssignmentSolution Best;
            public int BestFitness;
            public double BestImprovement;

            public int Uphills;
            public int Downhills;

            //tabou size
            public int AverageTabouSize;

            public string ToString()
            {
                string str = "";
                str += "\nAverage Tabou Size : " + AverageTabouSize;

                str += "\nBest Solution : " + Best.ToString();
                str += "\nBest Fitness : " + BestFitness;
                str += "\nTotal Improvement : " + BestImprovement + "%";


                str += "\nNb Uphills : " + Uphills;
                str += "\nNb Downhills : " + Uphills;

                str += "\n";

                return str;
            }

        }
    }
}
