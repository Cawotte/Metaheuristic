

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

            //Read Weights
            this.weights = new int[n,n];
            Utils.Read2DArrayFromFile(file, this.weights);

            //Read Distance
            this.distances = new int[n,n];
            Utils.Read2DArrayFromFile(file, this.distances);

            file.Close();

        }

        #region Public Methods
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

            solution.Score = sum; //save its score

            return sum;

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
