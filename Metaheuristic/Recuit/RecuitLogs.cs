
namespace Metaheuristic.Recuit
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Metaheuristic.QAP;
    using CsvHelper;
    using CsvHelper.Configuration.Attributes;
    using System.IO;

    public class RecuitLogs : AbstractLogsWithCurrent
    {
        public void AddStep(QuadraticAssignmentSolution current, QuadraticAssignmentSolution best, double currentTemp)
        {
            LogRecuit step = new LogRecuit();

            //Recuit
            step.CurrentTemp = currentTemp;

            AddStep(step, current, best);
        }

        public override void AddFinalLog()
        {
            FinalLogRecuit final = new FinalLogRecuit();
            LogRecuit previous = (LogRecuit)Previous;

            //Recuit
            final.FinalTemperature = previous.CurrentTemp;

            final.NbWorsePicked = 0;
            for (int i = 1; i < logs.Count; i++)
            {
                if (logs[i].HasImproved == Improvement.Worsened)
                {
                    final.NbWorsePicked++;
                }
            }

            AddFinal(final);

        }
        
        public class LogRecuit : AbstractLogWithCurrent
        {

            [Name("Current Temperature")]
            public double CurrentTemp { get; set; }


            public override string ToString()
            {
                string str = base.ToString();
                str += "\nCurrent Temperature : " + CurrentTemp;

                str += "\n";

                return str;
            }

        }

        public class FinalLogRecuit : AbstractFinalLog
        {

            public double FinalTemperature;
            public int NbWorsePicked;

            public override string ToString()
            {
                string str = base.ToString();
                str += "\nFinal Temperature: " + FinalTemperature;
                str += "\nNumber of Worse Solution Picked : " + NbWorsePicked;
                
                str += "\n";

                return str;
            }

        }
    }
}
