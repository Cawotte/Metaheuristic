

namespace Metaheuristic
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using QAP;
    class TestQAP
    {
        static String path = @"DataTaillard\";
        static String filename = "tai12a.dat";

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

            /*
            List<string> paths = new List<string>();
            //paths.Add(Directory.GetCurrentDirectory() + path + filename);
            paths.Add(path + filename);
            paths.Add("..\\DataTaillard\\tai12a.dat"); //FOUND
            paths.Add("DataTaillard\\tai12a.dat");
            paths.Add("..\\DataTaillard\tai12a.dat");
            paths.Add(@"..\DataTaillard\tai12a.dat"); //FOUND
            paths.Add(@"DataTaillard\tai12a.dat");

            
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

            } */

            //QuadratricAssignment qap = new QuadratricAssignment(3, weights, distances);

            QuadratricAssignment qapf = new QuadratricAssignment(path + filename);
            Console.WriteLine(qapf.ToString());

            QuadraticAssignmentSolution sol = new QuadraticAssignmentSolution("8 1 6 2 11 10 3 5 9 7 12 4");
            //QuadraticAssignmentSolution sol = new QuadraticAssignmentSolution("8 1 6 2 11 10 3 5 9 7 11 4");
            //QuadraticAssignmentSolution sol = new QuadraticAssignmentSolution("8 1 6 3 5 9 7 11 4");

            Console.WriteLine("Solution : ");
            Console.WriteLine(qapf.ToString());


            Console.WriteLine("IsValid : " + sol.IsValid());
            Console.WriteLine("Score : ");
            Console.WriteLine(qapf.EvaluateSolution(sol));
            Console.WriteLine(qapf.EvaluateSolution(sol) * 2);
            /*
            List<int[]> solutions = new List<int[]>();
            solutions.Add(new int[] { 1, 2, 3 });
            solutions.Add(new int[] { 3, 2, 1 });
            solutions.Add(new int[] { 1, 3, 2 });

            foreach (int[] solution in solutions)
            {
                Console.WriteLine("Solution: [{0}]", string.Join(", ", solution));
                Console.WriteLine("Value: " + qap.EvaluateSolution(solution));
            }*/


            Console.WriteLine("Click on any key to exit.");
            Console.ReadKey();
        }
    }
}
