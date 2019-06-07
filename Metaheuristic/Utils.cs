
namespace Metaheuristic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;


    public class Utils
    {

        public static int[] Multiply(int[] p1, int[] p2)
        {
            int[] product = new int[p1.Length];

            for (int i = 0; i < product.Length; i++)
            {
                product[i] = p2[p1[i]-1];
            }

            return product;
        }

        public static bool ArrayAreEquals(int[] a, int[] b)
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

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
        {
            //Quick and poor shuffler, it can be faster + should be able to use a seed.
            ///TODO : Rework shuffler into something better

            Random rand = RandomSingleton.Instance.CurrentAlgoRand;


            return source.OrderBy<T, int>((item) => rand.Next());

            /*
            //Randomly swap spots.
            for (int i = 0; i < array.Length - 1; i++)
            {
                int j = rand.Next(i, array.Length);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            } */
        }
        /// <summary>
        /// Parse the string to return an array with all integers from the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int[] ParseStringToIntArray(string str)
        {
            List<int> arrangement = new List<int>();

            MatchCollection numbers = Regex.Matches(str, @"-?\d+");
            for (int i = 0; i < numbers.Count; i++)
            {
                string num = numbers[i].Value;

                //There might be empty strings, so we ignore them.
                if (!string.IsNullOrEmpty(num))
                {
                    arrangement.Add(int.Parse(num));
                }
            }


            return arrangement.ToArray();
        }

        /// <summary>
        /// Format the 2D Array in a good looking string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static string String2DArray<T>(T[,] matrix)
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

        /// <summary>
        /// Read the next N integers in the file and return the in an array, even across lines.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="count">Number of integers to read</param>
        /// <returns></returns>
        public static int[] ReadNextIntegers(StreamReader file, int count)
        {
            int[] integers = new int[count];
            int index = 0;
            while (index < count)
            {
                
                int[] nextLineNumbers = ParseStringToIntArray(file.ReadLine());

                for (int i = 0; i < nextLineNumbers.Length; i++)
                {
                    integers[index++] = nextLineNumbers[i];

                    if (index == count && i != nextLineNumbers.Length - 1)
                    {
                        Console.WriteLine("WARNING ReadLineInt : Some extras integers were not read!");
                        break;
                    }

                }
            }

            return integers;
        }

        /// <summary>
        /// Parse a file to file an array.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="array"></param>
        public static void Read2DArrayFromFile(StreamReader file, int[,] array)
        {
            int n = array.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                int[] numbers;
                do
                {
                    //Get all the integers in the next line
                    numbers = ParseStringToIntArray(file.ReadLine());
                } while (numbers.Length <= 1);

                if (numbers.Length != n)
                {
                    throw new QAP.InvalidQAPException("Error when reading the file, must be at the wrong format!");
                }

                for (int j = 0; j < n; j++)
                {
                    array[i, j] = numbers[j];
                }

                //If there's more than n numbers, there'll be an error, but it shouldn't happen.
            }
        }
    }
}
