using Microsoft.VisualStudio.TestTools.UnitTesting;
using Metaheuristic;
using System;
using Metaheuristic.QAP;
using System.Collections.Generic;

namespace MetaheuristicTests
{
    [TestClass]
    public class TestBasics
    {
        [TestMethod]
        //Test if Permutations are being multiplied correctly
        public void TestMultiplyPermutations()
        {
            int[] p1 = new int[] { 2, 1, 3 };
            int[] p2 = new int[] { 1, 3, 2 };


            int[] p3 = Utils.Multiply(p1, p2);
            Assert.IsTrue(Utils.ArrayAreEquals(p3, new int[] { 3, 1, 2 }));

            p3 = Utils.Multiply(p2, p1);
            Assert.IsTrue(Utils.ArrayAreEquals(p3, new int[] { 2, 3, 1 }));


            QuadraticAssignmentSolution s1 = new QuadraticAssignmentSolution(p1);
            QuadraticAssignmentSolution s2 = new QuadraticAssignmentSolution(p2);


            Assert.IsTrue((s1 * s2).Equals(new int[] { 3, 1, 2 }));
            Assert.IsTrue((s2 * s1).Equals(new int[] { 2, 3, 1 }));
        }

        [TestMethod]
        //Test if the QapSolution Equals() override is correct.
        public void TestEqualsSolutions()
        {
            int[] p1 = new int[] { 1, 4, 2, 3, 5 };
            int[] p2 = new int[] { 2, 3, 5, 4, 1 };

            QuadraticAssignmentSolution s1 = new QuadraticAssignmentSolution(p1);
            QuadraticAssignmentSolution s2 = new QuadraticAssignmentSolution(p2);


            Assert.IsTrue(s1.Equals(s1));

            Assert.IsFalse(s1.Equals(s2));


            Assert.IsTrue(s1.Equals(p1));

            Assert.IsFalse(s1.Equals(p2));


            Assert.IsTrue(s1.Equals(new int[] { 1, 4, 2, 3, 5 }));

            Assert.IsFalse(s1.Equals(new int[] { 2, 3, 5, 4, 1 }));
        }

        /*
        [TestMethod]
        ///For several files, test if Evaluate calculate the correct solution score.
        public void TestDataReading()
        {
            string path = @"DataTaillard\";
            List<string> filenames = new List<string>();
            filenames.Add("tai12a");
            filenames.Add("tai15a");
            filenames.Add("tai17a");
            filenames.Add("tai20a");
            filenames.Add("tai25a");

            foreach (string fname in filenames)
            {
                string problemFilepath = path + fname + ".dat";
                string solutionFilepath = path + fname + ".sln";

                QuadratricAssignment qap = new QuadratricAssignment(problemFilepath);
                QuadraticAssignmentSolution solution = new QuadraticAssignmentSolution(solutionFilepath, false);

                int evaluation = qap.Evaluate(solution);
                
                Assert.AreEqual(evaluation, solution.Score);

                
            }
        }*/

        [TestMethod]
        public void TestSolutionCoherence()
        {
            //We pair the tests to their expected results.
            Dictionary<string, bool> tests = new Dictionary<string, bool>();

            tests.Add("8 1 6 2 11 10 3 5 9 7 12 4", true);
            tests.Add("8 1 6 2 11 10 9 7 12 4", false);
            tests.Add("8 -1 6 2 11 10 3 5 9 7 12 4", false);
            tests.Add("8 1 6 2 11 10 3 5 9 7 13 4", false);
            tests.Add("8 1 6 2 11 10 3 5 9 7 13 4 3 4", false);
            
            //We execute the tests
            foreach (string test in tests.Keys)
            {
                bool isValid = QuadraticAssignmentSolution.IsValid(test);
                Assert.AreEqual(isValid, tests[test]);

            }
        }
    }
}
