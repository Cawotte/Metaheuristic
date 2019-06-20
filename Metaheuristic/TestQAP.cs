

namespace Metaheuristic
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using QAP;
    using GA;
    using System.Linq;
    using Metaheuristic.Recuit;
    using Metaheuristic.MethodeTabou;

    public class TestQAP
    {
        static String path = @"DataTaillard\";
        static String pathCSV = @"CSV\";
        static String filename = "tai12a.dat";

        static void Main2(string[] args)
        {
            //TestTabou();
            //TestRecuitSimuleGA();
            //TestRecuitSimule();
            TestAlgorithmGenetic();

            Console.WriteLine("Click on any key to exit.");
            Console.ReadKey();
        }

        private static void TestTupleEquals()
        {
            Tuple<int, int> t1 = new Tuple<int, int>(0, 4);
            Tuple<int, int> t2 = new Tuple<int, int>(0, 4);
            Tuple<int, int> t3 = new Tuple<int, int>(4, 4);

            DisplaySuccess(t1.Equals(t2));

            DisplaySuccess(!t1.Equals(t3));
            
        }

        private static void TestTabou()
        {
            string problemFilepath = path + "tai40a" + ".dat";
            QuadraticAssignment qap = new QuadraticAssignment(problemFilepath);

            //Seed it!
            //RandomSingleton.Instance.Seed = 0;
            //QuadraticAssignmentSolution initialSol = QuadraticAssignmentSolution.GetIdentity(qap.N);

            RandomSingleton.Instance.Seed = 6;
            QuadraticAssignmentSolution initialSol = new QuadraticAssignmentSolution(qap.N);


            MethodeTabou.Tabou tabou = new MethodeTabou.Tabou(qap);
            tabou.Verbose = true;

            QuadraticAssignmentSolution best = tabou.Run(
                initialSol,
                80,
                500);

            Console.WriteLine(tabou.Logs.FinalLog.ToString());

            Console.WriteLine(tabou.Logs.ToStringImprovements());

            //Console.WriteLine("Best : " + best.ToString());
            //Console.WriteLine("Fitness : " + best.Fitness);

        }

        private static void TestRecuitSimuleGA()
        {
            string problemFilepath = path + "tai12a" + ".dat";
            QuadraticAssignment qap = new QuadraticAssignment(problemFilepath);

            //Seed it!
            RandomSingleton.Instance.Seed = 0;

            QuadraticAssignmentSolution initialSol = qap.Identity;

            RecuitSimule recuit = new RecuitSimule(qap);

            GeneticAlgorithmRecuit ga = new GeneticAlgorithmRecuit(recuit, 40, 15, 0.05d, 1, 3);

            ga.verbose = true;
            ga.WithLogs = true;
            RecuitSimuleParameters bestParam = ga.Run();
            QuadraticAssignmentSolution best = recuit.Run(bestParam);

            Console.WriteLine("Best Params : " + bestParam.ToString());
            Console.WriteLine("\nBest Solution : " + best.ToString());
            Console.WriteLine("Fitness : " + best.Fitness);

        }
        private static void TestRecuitSimule()
        {

            string problemFilepath = path + "tai12a" + ".dat";
            QuadraticAssignment qap = new QuadraticAssignment(problemFilepath);

            QuadraticAssignmentSolution initialSol = qap.Identity;
            QuadraticAssignmentSolution best;
            
            RecuitSimule recuit = new RecuitSimule(qap);
            best = recuit.Run(initialSol, 70000, 0.85f, 200);

            Console.WriteLine(recuit.Logs.FinalLog.ToString());
            Console.WriteLine(recuit.Logs.ToStringImprovements());
            
        }

        private static void TestAlgorithmGenetic()
        {

            string problemFilepath = path + "tai12a" + ".dat";
            QuadraticAssignment qap = new QuadraticAssignment(problemFilepath);

            //Seed it!
            RandomSingleton.Instance.Seed = 0;

            GeneticAlgorithmQAP ga = new GeneticAlgorithmQAP(qap, 100, 50, 0.05d, 2, 20);

            ga.verbose = true;
            ga.WithLogs = true;
            QuadraticAssignmentSolution best = ga.Run();

            string pathWrite = Path.Combine(pathCSV, "tai12a", "ga.csv");
            ga.Logs.SaveLogsTo(pathWrite);

            Console.WriteLine("Best Solution : " + best.ToString());
            Console.WriteLine("Fitness : " + best.Fitness);
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

        private static void TestDataReading()
        {
            List<string> filenames = new List<string>();
            filenames.Add("tai12a");
            //filenames.Add("tai12b");
            filenames.Add("tai15a");
            filenames.Add("tai17a");
            filenames.Add("tai20a");
            filenames.Add("tai25a");

            foreach (string fname in filenames)
            {
                Console.WriteLine("QAP : " + fname);

                string problemFilepath = path + fname + ".dat";
                string solutionFilepath = path + fname + ".sln";
                
                QuadraticAssignment qap = new QuadraticAssignment(problemFilepath);
                QuadraticAssignmentSolution solution = new QuadraticAssignmentSolution(solutionFilepath, false);

                int evaluation = qap.Evaluate(solution);

                Console.WriteLine("Evaluation      : " + evaluation);
                Console.WriteLine("Expected result : " + solution.Fitness);
                if (evaluation == solution.Fitness)
                {
                    Console.WriteLine("SUCCESSFUL");
                }
                else
                {
                    Console.WriteLine("FAILED");
                }


                Console.WriteLine("Click on any key to continue.");
                Console.ReadKey();
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



            QuadraticAssignment qap = new QuadraticAssignment(3, weights, distances);
            
            
            List<int[]> solutions = new List<int[]>();
            solutions.Add(new int[] { 1, 2, 3 });
            solutions.Add(new int[] { 3, 2, 1 });
            solutions.Add(new int[] { 1, 3, 2 });

            foreach (int[] solution in solutions)
            {
                Console.WriteLine("Solution: [{0}]", string.Join(", ", solution));
                Console.WriteLine("Value: " + qap.Evaluate(solution));
            }
        }

        private static void TestQAPFileReading()
        {
            QuadraticAssignment qapf = new QuadraticAssignment(path + filename);
            Console.WriteLine(qapf.ToString());

            QuadraticAssignmentSolution sol = new QuadraticAssignmentSolution("8 1 6 2 11 10 3 5 9 7 12 4");

            Console.WriteLine("Solution : ");
            Console.WriteLine(qapf.ToString());

            
            Console.WriteLine("Score : ");
            Console.WriteLine(qapf.Evaluate(sol));
            Console.WriteLine(qapf.Evaluate(sol) * 2);

        }

        private static void TestComposition()
        {
            int[] p2 = new int[] { 1, 4, 2, 3, 5 };
            int[] p1 = new int[] { 2, 3, 4, 5, 1 };

            Console.WriteLine(string.Join(",", p1));
            Console.WriteLine(string.Join(",", p2));

            int[] p3 = new int[5];
            p1.CopyTo(p3, 0);

            Console.WriteLine("Composition:");
            for (int i = 0; i < 6; i++)
            {
                p3 = Utils.Multiply(p3, p2);
                Console.WriteLine(string.Join(",", p3));
            }

            Console.WriteLine("-----");
        }

        private static void TestMultiplyPermutations()
        {
            int[] p1 = new int[] { 2, 1, 3};
            int[] p2 = new int[] { 1, 3, 2};

            Console.WriteLine("A = " + string.Join(",", p1));
            Console.WriteLine("B = " + string.Join(",", p2));

            int[] p3 = Utils.Multiply(p1, p2);

            Console.WriteLine("A * B = " + string.Join(",", p3));
            if (Utils.ArrayAreEquals(p3, new int[] { 3, 1, 2 }))
            {
                Console.WriteLine("\tSUCCESS");
            }
            else
            {
                Console.WriteLine("\tFAILED");
            }
            
            p3 = Utils.Multiply(p2, p1);
            Console.WriteLine("B * A = " + string.Join(",", p3));
            if (Utils.ArrayAreEquals(p3, new int[] { 2, 3, 1 }))
            {
                Console.WriteLine("\tSUCCESS");
            }
            else
            {
                Console.WriteLine("\tFAILED");
            }

            Console.WriteLine("-----");
        }

        private static void TestMultiplySolution()
        {
            QuadraticAssignmentSolution p1 = new QuadraticAssignmentSolution(new int[] { 2, 1, 3 });
            QuadraticAssignmentSolution p2 = new QuadraticAssignmentSolution(new int[] { 1, 3, 2 });

            Console.WriteLine("A = " + string.Join(",", p1));
            Console.WriteLine("B = " + string.Join(",", p2));

            QuadraticAssignmentSolution p3 = p1 * p2;

            Console.WriteLine("A * B = " + string.Join(",", p3));
            if (p3.Equals(new int[] { 3, 1, 2 }))
            {
                Console.WriteLine("\tSUCCESS");
            }
            else
            {
                Console.WriteLine("\tFAILED");
            }

            p3 = p2 * p1;
            Console.WriteLine("B * A = " + string.Join(",", p3));
            if (p3.Equals( new int[] { 2, 3, 1 }))
            {
                Console.WriteLine("\tSUCCESS");
            }
            else
            {
                Console.WriteLine("\tFAILED");
            }

            Console.WriteLine("-----");
        }
        private static void MultiplyPermutations()
        {
            int[] p1 = new int[] { 1, 4, 2, 3, 5 };
            int[] p2 = new int[] { 2, 3, 5, 4, 1 };

            Console.WriteLine("A = " + string.Join(",", p1));
            Console.WriteLine("B = " + string.Join(",", p2));

            int[] p3 = Utils.Multiply(p1, p2);
            Console.WriteLine("A * B = " + string.Join(",", p3));

            p3 = Utils.Multiply(p2, p1);
            Console.WriteLine("B * A = " + string.Join(",", p3));

            Console.WriteLine("-----");
        }
        private static void TestEquals()
        {

            Console.WriteLine("---- TEST EQUALS SOLUTIONS -----");
            int[] p1 = new int[] { 1, 4, 2, 3, 5 };
            int[] p2 = new int[] { 2, 3, 5, 4, 1 };

            QuadraticAssignmentSolution s1 = new QuadraticAssignmentSolution(p1);
            QuadraticAssignmentSolution s2 = new QuadraticAssignmentSolution(p2);


            Console.WriteLine("s1 = " + s1.ToString());
            Console.WriteLine("s2 = " + s2.ToString());
            Console.WriteLine();

            Console.WriteLine("s1.Equals(s1) == true");
            DisplaySuccess(s1.Equals(s1) == true);

            Console.WriteLine("s1.Equals(s2) == false");
            DisplaySuccess(s1.Equals(s2) == false);

            Console.WriteLine();

            Console.WriteLine("s1.Equals(p1) == true");
            DisplaySuccess(s1.Equals(p1) == true);

            Console.WriteLine("s1.Equals(p2) == false");
            DisplaySuccess(s1.Equals(p2) == false);


            Console.WriteLine();

            Console.WriteLine("s1.Equals(new int[] { 1, 4, 2, 3, 5 }) == true");
            DisplaySuccess(s1.Equals(new int[] { 1, 4, 2, 3, 5 }) == true);

            Console.WriteLine("s1.Equals(new int[] { 2, 3, 5, 4, 1 }) == false");
            DisplaySuccess(s1.Equals(new int[] { 2, 3, 5, 4, 1 }) == false);


            Console.WriteLine("------");
        }

        private static void DisplaySuccess(bool success)
        {
            if (success)
            {
                Console.WriteLine("\tSUCCESS");
            }
            else
            {
                Console.WriteLine("\tFAILED");
            }
        }
    }
}
