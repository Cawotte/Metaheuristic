

namespace Metaheuristic.QAP
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Text.RegularExpressions;

    public class QuadraticAssignment
    {
        //Statics
        //Store the Identity Permutation fo size N (Dynamic Programming)
        private static Dictionary<int, QuadraticAssignmentSolution> identities = new Dictionary<int, QuadraticAssignmentSolution>();

        //Store all inversions possibles for a n-sized solutions.
        private static Dictionary<int, List<Tuple<int, int>>> inversions = new Dictionary<int, List<Tuple<int, int>>>();

        /**
         * 
         * w(a,b) = Poids entre les equipements a et b.
         * f(a) = Emplacement de l'équipement a.
         * d( f(a), f(b) ) = Distance entre les emplacements des equipements a et b.
         * 
         * Score = Somme des w(a,b) * d( f(a), f(b) ) pour toutes les combinaisons (a,b) possibles.
         * */

        private readonly int n;
        private readonly int[,] weights;
        private readonly int[,] distances;

        public int N { get => n; }

        public int[,] Weights { get => weights; }
        public int[,] Distances { get => distances; }

        /// <summary>
        /// The Identity permutation of size N.
        /// </summary>
        public QuadraticAssignmentSolution Identity
        {
            get => GetIdentity(n);
        }

        /// <summary>
        /// All the possible inversions for a permutation of size N.
        /// </summary>
        public Tuple<int, int>[] Inversions
        {
            get => GetInversions(n);
        }

        /// <summary>
        /// Create a QAP from given parameters
        /// </summary>
        /// <param name="n"></param>
        /// <param name="weights"></param>
        /// <param name="distances"></param>
        public QuadraticAssignment(int n, int[,] weights, int[,] distances)
        {
            this.n = n;

            //Error check
            if ( !IsSquareArray(weights, n) || !IsSquareArray(distances, n) )
            {
                throw new InvalidQAPException("The Weights and Distances arrays must be of size " + n + "*" + n + "!");
            }

            this.weights = weights;
            this.distances = distances;
        }

        /// <summary>
        /// Read a QAP from a file
        /// </summary>
        /// <param name="filepath"></param>
        public QuadraticAssignment(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            StreamReader file = new StreamReader(filepath);

            //Read N
            this.n = int.Parse(file.ReadLine());

            //Read Weights
            this.weights = new int[n,n];
            Utils.Read2DArrayFromFile(file, this.weights);

            //Read Distance
            this.distances = new int[n,n];
            Utils.Read2DArrayFromFile(file, this.distances);

            file.Close();

        }

        #region Public Methods

        /// <summary>
        /// Calculate the fitness of that solution for this QAP.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public int Evaluate(int[] solution)
        {
            if (solution.Length != n)
            {
                throw new InvalidQAPException("Solution must be of size N = " + n);
            }

            int sum = 0;

            for (int x = 0; x < n; x++)
            {
                for (int y = x + 1; y < n; y++)
                {
                    sum += weights[x, y] * distances[solution[x] - 1, solution[y] - 1];
                }
            }

            sum *= 2;

            return sum;
        }

        /// <summary>
        /// Calculate the fitness of that solution for this QAP.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public int Evaluate(QuadraticAssignmentSolution solution)
        {
            if (solution.N != n)
            {
                throw new InvalidQAPException("Solution must be of size N = " + n);
            }
            int sum = 0;

            for (int x = 0; x < n; x++)
            {
                for (int y = x + 1; y < n; y++)
                {
                    sum += weights[x, y] * distances[solution[x] - 1, solution[y] - 1];
                }
            }

            //Only works for A type Taillard Instance (B does not have symmetrical weights)
            sum *= 2;

            solution.Fitness = sum; //save its score

            return sum;

        }

        /// <summary>
        /// Compute and save the fitness of all solutions
        /// </summary>
        /// <param name="solutions"></param>
        public void Evaluate(QuadraticAssignmentSolution[] solutions)
        {
            for (int i = 0; i < solutions.Length; i++)
            {
                solutions[i].Fitness = Evaluate(solutions[i]);
            }
        }

        public Tuple<int, int> GetRandomInversion()
        {
            return GetRandomInversion(n);
        }

        public override string ToString()
        {
            string str = "";
            str += n + "\n\n";
            str += Utils.String2DArray(weights) + "\n";
            str += Utils.String2DArray(distances) + "\n";

            return str;
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Return an Identity permutation of size N.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return all possible inversions for a permutation of size N.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Tuple<int, int>[] GetInversions(int n)
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

        /// <summary>
        /// Return a random permutation for a permutation of size N
        /// </summary>
        /// <param name="sizeN"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetRandomInversion(int n)
        {
            Tuple<int, int>[] inversions = GetInversions(n);

            Random rand = RandomSingleton.Instance.CurrentAlgoRand;

            return inversions[rand.Next(0, inversions.Length)];
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Return true if the 2D array is of size N*N
        /// </summary>
        /// <param name="array"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static bool IsSquareArray(int[,] array, int n)
        {
            return (array.GetLength(0) == n && array.GetLength(1) == n);
        }

       

        #endregion
    }
}
