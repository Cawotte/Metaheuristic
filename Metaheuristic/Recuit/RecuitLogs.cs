﻿
namespace Metaheuristic.Recuit
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Metaheuristic.QAP;

    public class RecuitLogs
    {
        private List<LogRecuit> logs = new List<LogRecuit>();
        private LastLogRecuit finalLog;

        public LogRecuit Last
        {
            get => logs[logs.Count - 1];
        }

        public LogRecuit this[int index]
        {
            get => logs[index];
        }

        public LastLogRecuit FinalLog
        {
            get => finalLog;
        }

        public int Size
        {
            get => logs.Count;
        }

        public void AddStep(QuadraticAssignmentSolution current, QuadraticAssignmentSolution best, double currentTemp)
        {
            LogRecuit step = new LogRecuit();

            step.Step = logs.Count;

            step.Current = current;
            step.CurrentFitness = current.Fitness;

            step.Best = best;
            step.BestFitness = best.Fitness;

            step.CurrentTemp = currentTemp;

            //If it's not the first step, does uphill and improvements
            if (logs.Count > 0)
            {
                //Uphill depends on if the solution is improving or not. 0 = improving
                if (current.Fitness < Last.CurrentFitness)
                {
                    step.HasImproved = -1;
                }
                else if (current.Fitness > Last.CurrentFitness)
                {
                    step.HasImproved = 1;
                }
                else
                {
                    step.HasImproved = 0;
                }

                step.BestImprovement = GetImprovement(Last.BestFitness, best.Fitness);
                step.CurrentImprovement = GetImprovement(Last.CurrentFitness, current.Fitness);
            }
            else
            {
                step.HasImproved = 0;
                step.BestImprovement = 0d;
                step.CurrentImprovement = 0d;
            }

            //Add to logs
            logs.Add(step);
        }

        public void AddFinalLog()
        {
            LastLogRecuit final = new LastLogRecuit();

            //Best Individual
            final.Best = Last.Best;
            final.BestFitness = Last.BestFitness;

            //Improvement between first and best
            final.BestImprovement = GetImprovement(logs[0].BestFitness, Last.BestFitness);

            final.FinalTemperature = Last.CurrentTemp;

            final.NbWorsePicked = 0;
            for (int i = 1; i < logs.Count; i++)
            {
                if (logs[i].HasImproved == 1)
                {
                    final.NbWorsePicked++;
                }
            }


            //save it
            finalLog = final;

        }

        public string ToStringImprovements()
        {
            string str = "";
            for (int i = 0; i < logs.Count; i++)
            {
                switch (logs[i].HasImproved)
                {
                    case 0:
                        str += "o";
                        break;
                    case 1:
                        str += "+";
                        break;
                    case -1:
                        str += "-";
                        break;
                }
            }

            return str;
        }


        private double GetImprovement(int lastFitness, int newFitness)
        {
            return Math.Round((1d - (double)newFitness / (double)lastFitness) * 100, 4);
        }

        public struct LogRecuit
        {
            public int Step;

            public int HasImproved;

            //current sol
            public QuadraticAssignmentSolution Current;
            public int CurrentFitness;
            public double CurrentImprovement;

            //best known sol
            public QuadraticAssignmentSolution Best;
            public int BestFitness;
            public double BestImprovement;

            //Forbidden list
            public double CurrentTemp;


            public override string ToString()
            {
                string str = "";
                str += "\nStep #" + Step;
                str += "\nCurrent Temperature : " + CurrentTemp;

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

        public struct LastLogRecuit
        {
            //Best found and improvements
            public QuadraticAssignmentSolution Best;
            public int BestFitness;
            public double BestImprovement;

            public double FinalTemperature;
            public int NbWorsePicked;

            public override string ToString()
            {
                string str = "";
                str += "\nFinal Temperature: " + FinalTemperature;
                str += "\nNb Worse Picks : " + NbWorsePicked;

                str += "\nBest Solution : " + Best.ToString();
                str += "\nBest Fitness : " + BestFitness;
                str += "\nTotal Improvement : " + BestImprovement + "%";


                str += "\n";

                return str;
            }

        }
    }
}