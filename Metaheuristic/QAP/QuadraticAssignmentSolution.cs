using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Metaheuristic.QAP
{
    public class QuadraticAssignmentSolution : IChromosome
    {
        //Store the Identity Permutation fo size N (Dynamic Programming)
        private static Dictionary<int, QuadraticAssignmentSolution> identities = new Dictionary<int, QuadraticAssignmentSolution>();

        //Store all inversions possibles for a n-sized solutions.
        private static Dictionary<int, List<Tuple<int, int>>> inversions = new Dictionary<int, List<Tuple<int, int>>>();

        #region Members
        private readonly int n;

        /// <summary>
        /// The solution. solution[i] = location of the equipment #i
        /// 
        /// Because it's in [1, n], a minus one must be added when the value is used to look in the Distances array.
        /// </summary>
        private int[] solution;
        
        private int fitness;
        #endregion

        #region Properties
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
        
        public int Fitness { get => fitness; set => fitness = value; }
        
        #endregion

        #region Constructors

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
            this.fitness = nums[1];

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

        /// <summary>
        /// Clone the solution
        /// </summary>
        /// <param name="solution"></param>
        public QuadraticAssignmentSolution(QuadraticAssignmentSolution original) {

            this.n = original.N;

            //Clone the array too
            int[] sol = new int[this.n];
            original.solution.CopyTo(sol, 0);

            this.solution = sol;
        }

        #endregion

        #region Public Methods
        public bool IsValid()
        {
            return IsValid(solution);
        }

        public QuadraticAssignmentSolution Clone()
        {
            return new QuadraticAssignmentSolution(this);
        }

        #region Override
        public override string ToString()
        {
            return "{" + string.Join(",", solution) + "}";
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
            return Equals(other.Solution);

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
        #endregion



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

        public QuadraticAssignmentSolution GetNeighbor(Tuple<int, int> inversion)
        {
            if (!IsValidInversion(inversion) )
            {
                throw new InvalidQAPException("The inversion is not valid!");
            }

            //Copy current
            QuadraticAssignmentSolution neighbor = Clone();

            //Inverse two elements
            int temp = neighbor[inversion.Item1];
            neighbor.Solution[inversion.Item1] = neighbor[inversion.Item2];
            neighbor.Solution[inversion.Item2] = temp;

            return neighbor;
        }

        public QuadraticAssignmentSolution GetRandomNeighbor()
        {
            return GetNeighbor(GetRandomInversion());
        }

        public Tuple<int, int> GetRandomInversion()
        {

            Random rnd = new Random();
            Tuple<int, int>[] inversions = QuadraticAssignmentSolution.GetInversions(n);

            return inversions[rnd.Next(0, inversions.Length)];
        }
        #endregion

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

        private bool IsValidInversion(Tuple<int, int> inversion)
        {
            return (inversion.Item1 >= 0 && inversion.Item1 < n && inversion.Item2 >= 0 && inversion.Item2 < n);
        }

        #endregion

        #region Static Methods
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

                if (occurences[assignment - 1] > 1)
                    return false;
            }
            
            for (int i = 0; i < occurences.Length; i++)
            {
                if (occurences[i] != 1) return false;
            }

            return true;
        }

        public static bool IsValid(string solution)
        {
            return IsValid(Utils.ParseStringToIntArray(solution));
        }


        public static QuadraticAssignmentSolution GetIdentity(int n)
        {
            if (identities.ContainsKey(n))
            {
                return identities[n].Clone();
            }
            else
            {
                //Create Identity
                int[] solution = new int[n];

                //Fill the array with numbers from 1 to N.
                for (int i = 0; i < n; i++)
                {
                    solution[i] = i + 1;
                }

                identities.Add(n, new QuadraticAssignmentSolution(solution));

                return identities[n].Clone();
            }
        }

        public static Tuple<int,int>[] GetInversions(int n)
        {
            if (inversions.ContainsKey(n))
            {
                return inversions[n].ToArray();
            }
            else
            {
                List<Tuple<int, int>> allInversions = new List<Tuple<int, int>>();
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        allInversions.Add(new Tuple<int, int>(i, j));
                    }
                }

                //Save it
                inversions.Add(n, allInversions);

                return inversions[n].ToArray();
            }
        }

        public static int[] GetRandomInversion(int sizeN)
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
        
        #endregion
    }
}
