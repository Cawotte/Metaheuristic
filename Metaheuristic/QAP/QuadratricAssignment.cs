

namespace Metaheuristic.QAP
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Text.RegularExpressions;

    class QuadratricAssignment
    {

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

        public QuadratricAssignment(int n, int[,] weights, int[,] distances)
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

        public QuadratricAssignment(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            StreamReader file = new StreamReader(filepath);

            //Read N
            this.n = int.Parse(file.ReadLine());
            file.ReadLine();

            //Read Weights
            this.weights = new int[n,n];
            Read2DArrayFromFile(file, this.weights);
            file.ReadLine();

            //Read Distance
            this.distances = new int[n,n];
            Read2DArrayFromFile(file, this.distances);

            file.Close();

        }

        #region Public Methods
        public int EvaluateSolution(int[] solution)
        {
            if (solution.Length != n)
            {
                throw new InvalidQAPException("Solution must be of size N : " + n);
            }

            int[] locationAssignments = new int[n + 1];
            for (int i = 0; i < n; i++)
            {
                locationAssignments[solution[i]] = i;
            }

            int sum = 0;

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    sum += weights[x, y] * distances[locationAssignments[x], locationAssignments[y]];
                }
            }

            return sum;
        }

        public int EvaluateSolution(QuadraticAssignmentSolution solution)
        {
            if (solution.N != n)
            {
                throw new InvalidQAPException("Solution must be of size N : " + n);
            }
            int sum = 0;

            for (int x = 0; x < n; x++)
            {
                for (int y = x + 1; y < n; y++)
                {
                    sum += weights[x, y] * distances[solution.Locations[x], solution.Locations[y]];
                }
            }

            sum *= 2;

            return sum;

        }

       
        public override string ToString()
        {
            string str = "";
            str += n + "\n\n";
            str += String2DArray(weights) + "\n";
            str += String2DArray(distances) + "\n";

            return str;
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

        private static string String2DArray<T>(T[,] matrix)
        {
            string str = "";
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    str += string.Format("{0, 3} ", matrix[i, j]);
                }
                str += "\n";
            }

            return str;
        }

        private static void Read2DArrayFromFile(StreamReader file, int[,] array)
        {
            int n = array.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                //Get all the integers in the next line
                string[] numbers = Regex.Split(file.ReadLine(), @"\D+");

                int j = 0;
                foreach (string number in numbers)
                {
                    //There might be empty strings, so we ignore them.
                    if (!string.IsNullOrEmpty(number))
                    {
                        array[i, j] = int.Parse(number);
                        j++;
                    }
                }

                //If there's more than n numbers, there'll be an error, but it shouldn't happen.
            }
        }

        #endregion
    }
}
