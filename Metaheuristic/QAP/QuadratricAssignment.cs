using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.QAP
{
    class QuadratricAssignment : IQuadratricAssignment
    {
        private int n;
        private int[,] weights;
        private int[,] distances;

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

        public int EvaluateSolution(int[] solution)
        {
            if (solution.Length != n)
            {
                throw new InvalidQAPException("Solution must be of size N : " + n);
            }

            int sum = 0;

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    sum += weights[x, y] * GetDistance(x, y, solution);
                }
            }

            return sum;
        }

        private int GetDistance(int a, int b, int[] solution)
        {
            int i = 0;
            int j = 0;
            for (int k = 0; k < n; k++)
            {
                if (solution[k] == a) i = k;
                if (solution[k] == b) j = k;
            }

            return GetDistance(i, j);
        }
        public int GetDistance(int a, int b)
        {
            return distances[a, b];
        }

        public int GetN()
        {
            return n;
        }

        public int GetWeight(int a, int b)
        {
            return weights[a, b];
        }


        /// <summary>
        /// Return true if the 2D array is of size N*N
        /// </summary>
        /// <param name="array"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsSquareArray(int[,] array, int n)
        {
            return (array.GetLength(0) == n && array.GetLength(1) == n);
        }
    }
}
