using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Metaheuristic.QAP
{
    class QuadraticAssignmentSolution
    {
        // Only the array solution is necessary. The array locations is deduced from it and makes up for faster calculations.

        private readonly int n;

        /// <summary>
        /// The solution. solution[i] = location of the equipment #i
        /// 
        /// Because it's in [1, n], a minus one must be added when the value is used to look in the Distances array.
        /// </summary>
        private int[] solution;
        
        private int score;

        public int N { get => n; }
        public int[] Solution
        {
            get
            {
                return solution;
            }
            
        }

        public int this[int i]
        {
            get => solution[i];
        }
        
        public int Score { get => score; set => score = value; }

        public QuadraticAssignmentSolution(int[] solution)
        {
            this.n = solution.Length;
            this.solution = solution;

            if (!IsValid(solution))
            {
                throw new InvalidQAPException("This is not a valid solution!");
            }
        }

        public QuadraticAssignmentSolution(string strSolution)
        {
            //Convert string to int array
            int[] solution = Utils.ParseStringToIntArray(strSolution);

            this.n = solution.Length;
            this.solution = solution;

            if (!IsValid(solution))
            {
                throw new InvalidQAPException("This is not a valid solution!");
            }

        }

        public QuadraticAssignmentSolution(string filepath, bool dummy)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            StreamReader file = new StreamReader(filepath);

            int[] nums = Utils.ParseStringToIntArray(file.ReadLine());

            //Read N
            this.n = nums[0];

            //Read Best Value
            this.score = nums[1];

            //Read Solution
            this.solution = Utils.ReadNextIntegers(file, n);

            file.Close();

            if (!IsValid(solution))
            {
                throw new InvalidQAPException("This is not a valid solution!");
            }
        }

        /// <summary>
        /// Create a random assignement solution of size N.
        /// </summary>
        /// <param name="n"></param>
        public QuadraticAssignmentSolution(int n)
        {
            //RANDOM SOLUTION OF S

            this.n = n;
            solution = new int[n];

            //Fill the array with numbers from 1 to N.
            for (int i = 0; i < n; i++)
            {
                solution[i] = i + 1;
            }

            //Shuffle it.
            Utils.Shuffle(solution);


        }



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

        /// <summary>
        /// Return true if the solution is <b>coherent</b> : 1 occurence of each number from 1 to N, N is the number of numbers.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
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

        public static bool IsValid(string solution)
        {
            return IsValid(Utils.ParseStringToIntArray(solution));
        }

        
    }
}
