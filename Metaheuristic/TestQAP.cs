

namespace Metaheuristic
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using QAP;
    class TestQAP
    {
        static String path = @"..\Data\";
        static String filename = "tai12.txt";

        static void Main(string[] args)
        {
            int[,] weights = new int[,]
            {
                { 0, 2, 3 },
                { 2, 0, 1 },
                { 3, 1, 0 },
            };

            int[,] distances = new int[,]
            {
                { 0, 7, 5 },
                { 7, 0, 9 },
                { 5, 9, 0 },
            };

            List<string> paths = new List<string>();
            //paths.Add(Directory.GetCurrentDirectory() + path + filename);
            paths.Add(path + filename);
            paths.Add("..\\Data\\tai12.txt"); //FOUND
            //paths.Add("Data\\tai12.txt");
            //paths.Add("..\\Data\tai12.txt");
            paths.Add(@"..\Data\tai12.txt"); //FOUND
            //paths.Add(@"Data\tai12.txt");

            
            foreach (string pf in paths) {
                try {
                    if (File.Exists(pf)) {
                        Console.WriteLine("FOUND : " + pf);
                    }
                    else {
                        Console.WriteLine("Not found : " + pf);    
                    }
                }
                catch (FileNotFoundException err) {
                    Console.WriteLine("EX : " + pf);    
                }

            }

            IQuadratricAssignment qap = new QuadratricAssignment(3, weights, distances);

            IQuadratricAssignment qapf = new QuadratricAssignment(path + filename);

            Console.WriteLine(qapf.ToString());

            List<int[]> solutions = new List<int[]>();
            solutions.Add(new int[] { 1, 2, 3 });
            solutions.Add(new int[] { 3, 2, 1 });
            solutions.Add(new int[] { 1, 3, 2 });

            foreach (int[] solution in solutions)
            {
                Console.WriteLine("Solution: [{0}]", string.Join(", ", solution));
                Console.WriteLine("Value: " + qap.EvaluateSolution(solution));

            }


            Console.WriteLine("Click on any key to exit.");
            Console.ReadKey();
        }
    }
}
