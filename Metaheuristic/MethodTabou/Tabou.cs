
namespace Metaheuristic.MethodeTabou
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Metaheuristic.QAP;
    using System.Linq;
    using Metaheuristic.MethodTabou;

    /**
     * Pick des voisins : Seulement prendre les voisins avec des emplacements à une distance X ou moins.
     * */
    public class Tabou
    {
        public bool Verbose = true;

        private Queue<Tuple<int, int>> forbiddenMoves;
        private int sizeTabou = 1;

        private QuadratricAssignment problem;

        private TabouLogs logs;

        public TabouLogs Logs { get => logs; }

        public Tabou(QuadratricAssignment problem)
        {
            this.problem = problem;
        }

        public QuadraticAssignmentSolution Run(
            QuadraticAssignmentSolution initialSol,
            int sizeTabou,
            int nbSteps)
        {

            //Initialization
            forbiddenMoves = new Queue<Tuple<int, int>>();
            this.sizeTabou = sizeTabou;

            logs = new TabouLogs();

            QuadraticAssignmentSolution best = initialSol;
            initialSol.Fitness = problem.Evaluate(initialSol);

            QuadraticAssignmentSolution current = initialSol;

            //Add step 0
            logs.AddStep(current, best, forbiddenMoves.Count);

            //Stop after X iterations
            for (int i = 0; i < nbSteps; i++)
            {
                if (Verbose)
                {
                    Console.WriteLine("Step #" + (i+1));
                    Console.WriteLine("Current Fitness : " + current.Fitness);
                }

                //At each iteration we choose the best neighbord
                QuadraticAssignmentSolution solution = GetBestInNeighborhood(current);

                solution.Fitness = problem.Evaluate(solution);

                //If it's a better solution
                if (solution.Fitness < current.Fitness)
                {
                    best = solution;

                    if (Verbose)
                    {
                        Console.WriteLine("New Best : " + best.Fitness);
                    }
                }
                else
                {
                    AddInterdiction(GetDifference(current, solution));
                }


                if (Verbose)
                {
                    if (solution.Fitness < current.Fitness)
                    {
                        Console.WriteLine("Current has improved");
                        Console.WriteLine();
                    }
                }

                current = solution;
                
                logs.AddStep(current, best, forbiddenMoves.Count);

            }

            logs.AddFinalLog();

            return best;
        }

        private void AddInterdiction(Tuple<int, int> inversion)
        {
            //If the inversion is not in the list
            if (!forbiddenMoves.Any(inv => inv.Equals(inversion)) )
            {
                //Add it to the list
                forbiddenMoves.Enqueue(inversion);

                //If the max size is reached, dequeue the last interdiction.
                if (forbiddenMoves.Count > sizeTabou)
                {
                    forbiddenMoves.Dequeue();
                }
            }
        }

        private QuadraticAssignmentSolution[] GetNeighborhood(QuadraticAssignmentSolution current, out Tuple<int, int>[] inversions)
        {
            //List of possible inversions minus the forbidden transformations
            inversions = QuadraticAssignmentSolution.GetInversions(problem.N).Except(forbiddenMoves).ToArray();
            return current.GetNeighbors(inversions);
        }

        private QuadraticAssignmentSolution GetBestInNeighborhood(QuadraticAssignmentSolution current)
        {
            Tuple<int, int>[] inversions; //not used
            QuadraticAssignmentSolution[] neighborhood = GetNeighborhood(current, out inversions);

            //compute neighborhood fitnesses
            problem.Evaluate(neighborhood);

            //get best fitness
            int bestFitness = neighborhood.Min(sol => sol.Fitness);
            //return elm with best fitness
            return neighborhood.First(sol => sol.Fitness == bestFitness);
        }

        /// <summary>
        /// Return the diff inversion between s1 and s2. WARNING : They must be neighbor and have only one inversion of diffenrence!
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private Tuple<int, int> GetDifference(QuadraticAssignmentSolution s1, QuadraticAssignmentSolution s2)
        {
            int[] inversion = new int[2];
            int index = 0;

            for (int i = 0; i < s1.N; i++)
            {
                if (s1.Solution[i] != s2.Solution[i])
                {
                    inversion[index++] = i;
                }
            }

            return new Tuple<int, int>(inversion[0], inversion[1]);
        }
        
    }
}
