using Metaheuristic.QAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.MethodTabou
{
    public class TabouParameters : SolverParameters, ISolverParameters
    {

        public QuadraticAssignmentSolution InitialSol;
        public int SizeTabou = 3;
        public int NbSteps = 20;

        public TabouParameters(int n) {
            InitialSol = new QuadraticAssignmentSolution(n);
        }

        public TabouParameters(QuadraticAssignmentSolution initialSol,
                        int sizeTabou,
                        int nbSteps)
        {
            this.InitialSol = initialSol;
            this.SizeTabou = sizeTabou;
            this.NbSteps = nbSteps;
        }

        public override string ToString()
        {

            string str = "";
            str += "\nN : " + InitialSol.N;
            str += "\nS0 : " + InitialSol.ToString();
            str += "\nSize Tabou : " + SizeTabou;
            str += "\nNbsteps : " + NbSteps;
            str += "\n";

            return str;
        }
    }
}
