using Metaheuristic.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.QAP
{
    public interface IQAPSolver
    {

        //Properties

        ILogs Logs
        {
            get;
        }

        bool Verbose
        {
            get; set;
        }

        QuadraticAssignmentSolution Run(ISolverParameters parameters);
        

    }
}
