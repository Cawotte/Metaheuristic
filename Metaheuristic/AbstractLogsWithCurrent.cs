using CsvHelper.Configuration.Attributes;
using Metaheuristic.QAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic
{
    public abstract class AbstractLogsWithCurrent : AbstractLogs
    {

        /// <summary>
        /// Complete the log with general info shared by all kind of logs, and add it to logs
        /// </summary>
        /// <param name=""></param>
        protected void AddStep(AbstractLogWithCurrent log, QuadraticAssignmentSolution current, QuadraticAssignmentSolution best)
        {
            log.Step = logs.Count;
            AbstractLogWithCurrent previous = (AbstractLogWithCurrent)Previous;

            //Current
            log.Current = current;
            log.CurrentFitness = current.Fitness;
            log.CurrentString = current.ToString();

            //Best
            log.Best = best;
            log.BestFitness = best.Fitness;
            log.BestString = best.ToString();


            //If it's not the first step, does uphill and improvements
            if (logs.Count > 0)
            {
                log.BestImprovement = GetImprovement(Previous.BestFitness, best.Fitness);
                log.HasImproved = GetImproved(current.Fitness, previous.CurrentFitness);
                log.CurrentImprovement = GetImprovement(previous.CurrentFitness, current.Fitness);
            }
            else
            {
                log.HasImproved = Improvement.Stagnated;
                log.BestImprovement = 0d;
            }

            //Add to logs
            logs.Add(log);

        }

        public abstract class AbstractLogWithCurrent : AbstractLog
        {
            //Current Solution
            public QuadraticAssignmentSolution Current;


            [Name("Current solution")]
            public string CurrentString { get; set; }

            [Name("Current fitness")]
            public int CurrentFitness { get; set; }

            [Name("Current improvement")]
            public double CurrentImprovement { get; set; }


            public override string ToString()
            {
                string str = "";
                str += "\nStep #" + Step;

                str += BestToString();
                str += CurrentToString();

                return str;
            }

            protected string CurrentToString()
            {
                string str = "";

                str += "\nCurrent has " + HasImproved.ToString();
                str += "\nCurrent : " + Current.ToString();
                str += "\nCurrent Fitness : " + CurrentFitness;
                str += "\nCurrent's Improvement : " + CurrentImprovement + "%";

                return str;
            }
        }
    }
}
