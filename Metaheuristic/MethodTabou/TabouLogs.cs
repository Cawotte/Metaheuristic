

namespace Metaheuristic.MethodTabou
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using Metaheuristic.QAP;
    using System.IO;
    using CsvHelper;
    using CsvHelper.Configuration.Attributes;
    using Metaheuristic.MethodeTabou;
    using Metaheuristic.Logs;

    public class TabouLogs : AbstractLogsWithCurrent
    {

        public void AddStep(QuadraticAssignmentSolution current, QuadraticAssignmentSolution best, int currentTabouSize)
        {
            LogTabou step = new LogTabou();

            //Tabou info
            step.CurrentTabouSize = currentTabouSize;

            //Add to logs
            AddStep(step, current, best);
        }

        public override void AddFinalLog()
        {
            FinalLogTabou final = new FinalLogTabou();
            
            //Tabou
            final.AverageTabouSize = (int)logs.Average(log => ((LogTabou)log).CurrentTabouSize);

            AddFinal(final);

        }
        
        public class LogTabou : AbstractLogWithCurrent
        {
            //Forbidden list
            [Name("Current Tabou Size")]
            public int CurrentTabouSize { get; set; }

            public override string ToString()
            {
                string str = base.ToString();

                str += "\nCurrent Tabou Size : " + CurrentTabouSize;
                str += "\n";

                return str;
            }

        }

        public class FinalLogTabou : AbstractFinalLog
        {
            //tabou size
            public int AverageTabouSize;

            public override string ToString()
            {
                string str = "";
                str += base.ToString();
                str += "\nAverage Tabou Size : " + AverageTabouSize;

                return str;
            }

        }
    }
}
