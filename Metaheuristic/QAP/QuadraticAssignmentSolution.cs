﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Metaheuristic.QAP
{
    public class QuadraticAssignmentSolution
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

        #region Override
        public override string ToString()
        {
            return "{" + string.Join(",", solution) + "}";
            /*
            string str = "";
            str += n + "\n";
            for (int i = 0; i < solution.Length; i++)
            {
                str += solution[i] + " ";
            }
            str += "\n"; 

            return str; */
        }

        public override bool Equals(object obj)
        {
            //If null or types doesn't match
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            QuadraticAssignmentSolution other = (QuadraticAssignmentSolution)obj;

            //Compare the array solutions
            return ArrayAreEquals(this.solution, other.Solution);

        }

        public bool Equals(int[] array)
        {
            return ArrayAreEquals(this.solution, array);
        }

        public override int GetHashCode()
        {
            ///TODO : To Change ?
            return solution.GetHashCode();
        }

        /// <summary>
        /// Multiply the solutions permutations using Permutation multiplication rules, and return the new solution. Non-Commutative!
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static QuadraticAssignmentSolution operator *(QuadraticAssignmentSolution p1, QuadraticAssignmentSolution p2)
        {
            int[] product = new int[p1.N];

            for (int i = 0; i < product.Length; i++)
            {
                product[i] = p2[p1[i] - 1];
            }

            return new QuadraticAssignmentSolution(product);
        }
        #endregion

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

        public QuadraticAssignmentSolution ApplyInversion(int[] inversion)
        {
            if (inversion.Length != 2)
            {
                throw new InvalidQAPException("Inversion needs to be an array of 2 int !");
            }
            if (inversion[0] >= n || inversion[1] >= n)
            {
                throw new InvalidQAPException("Outs of bounds inversion values !");
            }

            //Swap the values in the inversion indexes.

            int temp = solution[inversion[0]];
            solution[inversion[0]] = solution[inversion[1]];
            solution[inversion[1]] = temp;

            //To chain call
            return this;
            

        }

        public static bool IsValid(string solution)
        {
            return IsValid(Utils.ParseStringToIntArray(solution));
        }

        public static int[] GetRandomPermutation(int sizeN)
        {
            //Values goes from 0 to n-1 !!
            //Certified no self-permutations
            //To optimize?

            List<int> marbleBag = new List<int>();
            for (int i = 0; i < sizeN; i++)
            {
                marbleBag.Add(i);
            }

            //Pick two random
            Random rnd = new Random();
            int[] inversion = new int[2];


            //Pick 1
            inversion[0] = marbleBag[rnd.Next(0, sizeN)];

            //Remove it from list
            marbleBag.Remove(inversion[0]);

            //Pick 2
            inversion[1] = marbleBag[rnd.Next(0, sizeN - 1)];

            return inversion;
        }

        #region Private Methods
        private static bool ArrayAreEquals(int[] a, int[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

    }
}
