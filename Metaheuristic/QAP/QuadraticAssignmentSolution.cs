using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Metaheuristic.QAP
{
    class QuadraticAssignmentSolution
    {
        // Only the array solution is necessary. The array locations is deduced from it and makes up for faster calculations.

        private int n;

        /// <summary>
        /// The solution. equipments[i] = equipment positioned at location #i.
        /// </summary>
        private int[] solution;

        /// <summary>
        /// locations[i] = location of the equipment #i.
        /// </summary>
        private int[] locations;

        public QuadraticAssignmentSolution(int[] solution)
        {
            this.n = solution.Length;
            this.solution = solution;

            if (!IsValid(solution))
            {
                throw new InvalidQAPException("This is not a valid solution!");
            }

            int[] locations = new int[n + 1];
            for (int i = 0; i < n; i++)
            {
                locations[solution[i]] = i;
            }

            this.locations = locations;
        }

        public QuadraticAssignmentSolution(string strSolution)
        {
            //Convert string to int array
            this.solution = ParseStringToIntArray(strSolution);

            if (!IsValid(solution))
            {
                throw new InvalidQAPException("This is not a valid solution!");
            }


            /*
            List<int> arrangement = new List<int>();
            string[] numbers = Regex.Split(strSolution, @"\D+");
            foreach (string number in numbers)
            {
                //There might be empty strings, so we ignore them.
                if (!string.IsNullOrEmpty(number))
                {
                    arrangement.Add(int.Parse(number));
                }
            }

            this.solution = arrangement.ToArray();*/
            this.n = this.solution.Length;

            int[] locations = new int[n + 1];
            for (int i = 0; i < n; i++)
            {
                locations[solution[i]] = i;
            }

            this.locations = locations;


        }

        public int N { get => n; }
        public int[] Solution { get => solution; }
        public int[] Locations { get => locations; }

        public bool IsValid()
        {
            return IsValid(solution);
        }

        public override string ToString()
        {
            string str = "";
            str += n + "\n";
            for (int i = 0; i < solution.Length; i++)
            {
                str += solution[i] + " ";
            }
            str += "\n";

            return str;
        }

        public static bool IsValid(int[] solution)
        {
            int n = solution.Length;
            int[] occurences = new int[n];

            for (int i = 0; i < n; i++)
            {
                int assignment = solution[i];
                //If value out of bounds
                if (assignment <= 0 || assignment > n)
                {
                    return false;
                }

                occurences[assignment - 1]++;
            }

            //There must exactly one occurences of each number between 1 and n.  If we factor all the occurences the result will be 1 only if it's the case.
            float product = 1f;
            for (int i = 0; i < occurences.Length; i++)
            {
                product *= occurences[i];
            }

            return product == 1f;
        }

        private static int[] ParseStringToIntArray(string str)
        {
            List<int> arrangement = new List<int>();
            string[] numbers = Regex.Split(str, @"\D+");
            foreach (string number in numbers)
            {
                //There might be empty strings, so we ignore them.
                if (!string.IsNullOrEmpty(number))
                {
                    arrangement.Add(int.Parse(number));
                }
            }

            return arrangement.ToArray();
        }
    }
}
