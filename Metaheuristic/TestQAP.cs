

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

            TestSolutionsValidity();


            Console.WriteLine("Click on any key to exit.");
            Console.ReadKey();
        }

        private static void TestSolutionsValidity() {

            //We pair the tests to their expected results.
            Dictionary<string, bool> tests = new Dictionary<string, bool>();

            tests.Add("8 1 6 2 11 10 3 5 9 7 12 4", true);
            tests.Add("8 1 6 2 11 10 9 7 12 4", false);
            tests.Add("8 -1 6 2 11 10 3 5 9 7 12 4", false);
            tests.Add("8 1 6 2 11 10 3 5 9 7 13 4", false);
            tests.Add("8 1 6 2 11 10 3 5 9 7 13 4 3 4", false);


            Console.WriteLine("*** TEST SOLUTION COHERENCE***");

            //We execute the tests
            foreach (string test in tests.Keys)
            {

                Console.WriteLine("*** TEST");
                Console.WriteLine(test);

                bool isValid = QuadraticAssignmentSolution.IsValid(test);
                Console.Write(isValid + " ");
                if (isValid == tests[test])
                {
                    Console.WriteLine("SUCCESSFUL");
                }
                else
                {
                    Console.WriteLine("FAILED");
                }

            }
        }

        private static void TestPaths()
        {
            List<string> paths = new List<string>();
            paths.Add(Directory.GetCurrentDirectory() + path + filename);
            paths.Add(path + filename);
            paths.Add("..\\DataTaillard\\tai12a.dat"); //FOUND
            paths.Add("DataTaillard\\tai12a.dat");
            paths.Add("..\\DataTaillard\tai12a.dat");
            paths.Add(@"..\DataTaillard\tai12a.dat"); //FOUND
            paths.Add(@"DataTaillard\tai12a.dat");

            
            foreach (string pf in paths) {
                Console.Write(pf + "   : ");
                if (File.Exists(pf)) {
                    Console.WriteLine("FOUND");
                }
                else {
                    Console.WriteLine("NOT FOUND");    
                }
            } 

        }

        private static void TestQAPGeneration()
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



            QuadratricAssignment qap = new QuadratricAssignment(3, weights, distances);
            
            
            List<int[]> solutions = new List<int[]>();
            solutions.Add(new int[] { 1, 2, 3 });
            solutions.Add(new int[] { 3, 2, 1 });
            solutions.Add(new int[] { 1, 3, 2 });

            foreach (int[] solution in solutions)
            {
                Console.WriteLine("Solution: [{0}]", string.Join(", ", solution));
                Console.WriteLine("Value: " + qap.EvaluateSolution(solution));
            }
        }

        private static void TestQAPFileReading()
        {
            QuadratricAssignment qapf = new QuadratricAssignment(path + filename);
            Console.WriteLine(qapf.ToString());

            QuadraticAssignmentSolution sol = new QuadraticAssignmentSolution("8 1 6 2 11 10 3 5 9 7 12 4");

            Console.WriteLine("Solution : ");
            Console.WriteLine(qapf.ToString());

            
            Console.WriteLine("Score : ");
            Console.WriteLine(qapf.EvaluateSolution(sol));
            Console.WriteLine(qapf.EvaluateSolution(sol) * 2);

        }
    }
}
