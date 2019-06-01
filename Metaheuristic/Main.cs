using System;
using System.Collections.Generic;
using System.Text;

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
    using System.Text.RegularExpressions;
    using System.Reflection;
    using Metaheuristic.MethodTabou;

    class MainMeta
    {
        private static State state = State.Menu;
        private static Algo algo = Algo.Recuit;
        private static int seed = 0;
        private static int bestFitness = 0;
        
        private static string instancePath = @"DataTaillard\";
        private static string resultPath = @"Results\";
        private static string filename = "tai12a";
        
        private static string returnToMenu = "Return to the main menu...\n";

        private static QuadraticAssignment qap;
        private static QuadraticAssignmentSolution bestKnownSolution;

        private static string paramString = "";
        private static string resultString = "";

        static void Main()
        {
            seed = new Random().Next(0, 1000); //random default seed
            RandomSingleton.Instance.Seed = seed;

            string[] line;

            qap = new QuadraticAssignment(GetInstancePath(filename));
            bestKnownSolution = new QuadraticAssignmentSolution(GetSolutionPath(filename), true);

            while (state != State.Quit)
            {
                Console.WriteLine("------------");
                Console.WriteLine(WelcomeMessage());
                Console.WriteLine(GetInstanceInfo());
                Console.WriteLine("------------");

                line = GetLine();

                switch (line[0].ToLower()) {
                    case "q":
                    case "quit":
                        state = State.Quit;
                        break;
                    case "s":
                    case "seed":
                        Console.WriteLine("Veuillez entrer un nouveau seed > 0:");

                        int newSeed = -1;
                        if (!GetCorrectInt(out newSeed, val => val > 0))
                        {
                            break;
                        }

                        seed = newSeed;
                        RandomSingleton.Instance.Seed = seed;
                        Console.WriteLine("Le nouveau seed est : " + seed);
                        break;
                    case "t":
                    case "tai":

                        bool found = false;
                        while (!found) {

                            Console.WriteLine("Tapez le nom de l'instance de taillard à charger (sans son .dat):");
                            line = GetLine();

                            //early exit or quit
                            if (TryExitOrQuit(line[0]))
                            {
                                break;
                            }

                            found = FindTaillardInstance(line[0]);

                            if (!found)
                            {
                                Console.WriteLine("Fichier non trouvé ! Réessayer ou quitter (x or exit):");
                            }
                        }

                        filename = line[0];

                        break;
                    case "r":
                    case "rec":
                    case "recuit":
                        algo = Algo.Recuit;
                        RunRecuit();
                        break;
                    case "tab":
                    case "tabou":
                        algo = Algo.Tabou;
                        RunTabou();
                        break;

                    case "test":
                        string csv = GetNameCSV();
                        string txt = GetNameTxt();


                        File.Create(resultPath + csv).Dispose();
                        File.Create(resultPath + txt).Dispose();


                        Console.WriteLine("\nFichiers crées!");
                        break;
                    default:
                        Console.WriteLine("\nCommande invalide!");
                        break;

                }
            }

        }

        private static void TestPaths()
        {

            string path = @"DataTaillard\";
            List<string> paths = new List<string>();
            //paths.Add(Directory.GetCurrentDirectory() + path + filename);
            paths.Add(path + filename + ".dat"); //FOUND
            paths.Add(Path.Combine(path, filename + ".dat")); //FOUND
            paths.Add("DataTaillard\\tai12a.dat");  //FOUND
            paths.Add("..\\DataTaillard\\tai12a.dat");
            paths.Add("..\\DataTaillard\tai12a.dat");
            paths.Add(@"..\DataTaillard\tai12a.dat");
            paths.Add(@"DataTaillard\tai12a.dat"); //FOUND


            foreach (string pf in paths)
            {
                Console.Write(pf + "   : ");
                if (File.Exists(pf))
                {
                    Console.WriteLine("FOUND");
                }
                else
                {
                    Console.WriteLine("NOT FOUND");
                }
            }

        }
        
        private static bool RunRecuit()
        {
            algo = Algo.Recuit;
            QuadraticAssignmentSolution initialSol = new QuadraticAssignmentSolution(qap.N);
            double initialTemp = -1d;
            double temperatureDecrease = -1d;
            int maxSteps = -1;
            int nbNeighborPerStep = -1;
            RecuitSimuleParameters param;

            Console.WriteLine("Veuillez entrez les paramètres du Recuit : ");
            
            Console.WriteLine("Température Initiale (Prenez un grand nombre > 0 de l'ordre des fitness) :");
            if ( !GetCorrectDouble(out initialTemp, (val) => val > 0d) )
            {
                return false;
            }

            Console.WriteLine("Baisse de Température ( Réel à ]0,1[ ) :");
            if (!GetCorrectDouble(out temperatureDecrease, (val) => val > 0d && val < 1d))
            {
                return false;
            }

            Console.WriteLine("Nombre d'étapes de l'exécution ( > 0) : ");
            if (!GetCorrectInt(out maxSteps, (val) => val > 0))
            {
                return false;
            }

            Console.WriteLine("Nombre de voisins à tester par étapes (> 0) :");
            if (!GetCorrectInt(out nbNeighborPerStep, (val) => val > 0))
            {
                return false;
            }

            param = new RecuitSimuleParameters(initialSol, initialTemp, temperatureDecrease, maxSteps, nbNeighborPerStep);
            
            //param = new RecuitSimuleParameters(qap.N);
            Console.WriteLine("Tout les paramètres sont entrées, commencer l'exécution ? ( o/n, y/n )");
            Console.WriteLine(param.ToString());

            string str = "";
            if (!GetCorrectString(out str, (s) => IsValidation(s)))
            {
                return false;
            }

            if (!IsYes(str)) {
                return false;
            }

            //Lancer l'exécution
            RecuitSimule recuit = new RecuitSimule(qap);

            recuit.Verbose = true;

            bestFitness = recuit.Execute(param).Fitness;

            Console.WriteLine("Paramètres :");
            Console.WriteLine(param.ToString());

            Console.WriteLine("Résultats :");
            Console.WriteLine(recuit.Logs.FinalLog.ToString());

            paramString = param.ToString();
            resultString = recuit.Logs.FinalLog.ToString();

            //Save Results
            SaveResults();
            Console.WriteLine("Résultats sauvegardées dans " + GetResultPath() + "!");

            //Save Logs
            recuit.Logs.SaveLogsTo(GetCSVPath());
            Console.WriteLine("Logs sauvegardées dans " + GetCSVPath() + "!");

            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu.");
            Console.ReadKey();
            Console.WriteLine();


            return true;
        }

        private static bool RunTabou()
        {

            algo = Algo.Tabou;
            QuadraticAssignmentSolution initialSol = new QuadraticAssignmentSolution(qap.N);
            int sizeTabou = -1;
            int steps = -1;
            TabouParameters param;

            Console.WriteLine("Veuillez entrez les paramètres du Tabou : ");


            Console.WriteLine("Taille de la liste Tabou (>= 0) :");
            if (!GetCorrectInt(out sizeTabou, (val) => val >= 0))
            {
                return false;
            }

            Console.WriteLine("Nombre d'étapes de l'exécution ( > 0) : ");
            if (!GetCorrectInt(out steps, (val) => val > 0))
            {
                return false;
            }


            param = new TabouParameters(initialSol, sizeTabou, steps);

            //param = new RecuitSimuleParameters(qap.N);
            Console.WriteLine("Tout les paramètres sont entrées, commencer l'exécution ? ( o/n, y/n )");
            Console.WriteLine(param.ToString());

            string str = "";
            if (!GetCorrectString(out str, (s) => IsValidation(s)))
            {
                return false;
            }

            if (!IsYes(str))
            {
                return false;
            }

            //Lancer l'exécution
            Tabou tabou = new Tabou(qap);

            tabou.Verbose = true;

            bestFitness = tabou.Run(param).Fitness;

            Console.WriteLine("Paramètres :");
            Console.WriteLine(param.ToString());

            Console.WriteLine("Résultats :");
            Console.WriteLine(tabou.Logs.FinalLog.ToString());

            paramString = param.ToString();
            resultString = tabou.Logs.FinalLog.ToString();

            //Save Results
            SaveResults();
            Console.WriteLine("Résultats sauvegardées dans " + GetResultPath() + "!");

            //Save Logs
            tabou.Logs.SaveLogsTo(GetCSVPath());
            Console.WriteLine("Logs sauvegardées dans " + GetCSVPath() + "!");

            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu.");
            Console.ReadKey();
            Console.WriteLine();

            return true;
        }


        #region Get Correct Values
        /// <summary>
        /// Ask to read a double until the condition is met. Return true if succeed, false is exit/quit.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private static bool GetCorrectInt(out int number, Func<int, bool> condition)
        {
            string[] line;
            string str;
            do
            {

                line = GetLine();
                str = line[0];

                if (TryExitOrQuit(str))
                {
                    number = -1;
                    return false;
                }

                bool isNumber = TryGetInt(str, out number);

                if (!isNumber)
                {
                    Console.WriteLine("Veuillez entrez un nombre correct");
                }
            } while (!condition(number));

            return true;

        }

       

        /// <summary>
        /// Ask to read a double until the condition is met. Return true if succeed, false is exit/quit.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private static bool GetCorrectDouble(out double number, Func<double, bool> condition)
        {
            string[] line;
            string str;
            do
            {

                line = GetLine();
                str = line[0];

                if (TryExitOrQuit(str))
                {
                    number = -1;
                    return false;
                }

                bool isNumber = TryGetDouble(str, out number);

                if (!isNumber)
                {
                    Console.WriteLine("Veuillez entrez un nombre correct");
                }
            } while (!condition(number));

            return true;

        }

        private static bool GetCorrectString(out string str, Func<string, bool> condition)
        {
            string[] line;
            do
            {

                line = GetLine();
                str = line[0];

                if (TryExitOrQuit(str))
                {
                    str = "";
                    return false;
                }
                

                if (!condition(str))
                {
                    Console.WriteLine("Veuillez entrez une entrée correct");
                }
            } while (!condition(str));

            return true;

        }
        #endregion

        private static void SaveResults()
        {
            string str = "";
            switch (algo)
            {
                case Algo.Recuit:
                    str += "Recuit Simule";
                    break;
                case Algo.GA:
                    str += "Algorithme Génétique";
                    break;
                case Algo.RecuitGA:
                    str += "Paramètre Recuit Simule sur Algo Génétique";
                    break;
                case Algo.Tabou:
                    str += "Méthode Tabou";
                    break;
            }
            str += "\nParametres :\n" +
                paramString + "\n\n" +
                "Resultats:\n" + resultString;

            //put good CLRF
            str = str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");



            System.IO.File.WriteAllText(GetResultPath(), str);
        }

        #region Get Or Find Path
        private static string GetResourcesPath()
        {
            string runningPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = string.Format("{0}Resources", Path.GetFullPath(Path.Combine(runningPath, @"..\..\")));

            return fileName;
        }

        private static bool FindTaillardInstance(string name)
        {
            string path = GetInstancePath(name);
            //Console.WriteLine("path :\n\t" + path);
            return File.Exists(path);
        }

        private static string GetInstancePath(string name) {
            return Path.Combine(instancePath, name + ".dat");
        }

        private static string GetSolutionPath(string name)
        {
            return Path.Combine(instancePath, name + ".sln");
        }

        private static string GetResultPath()
        {
            return Path.Combine(resultPath, GetNameTxt());
        }
        
        private static string GetCSVPath()
        {
            return Path.Combine(resultPath, GetNameCSV());
        }

        private static bool FindFile(string path)
        {
            return File.Exists(path);
        }
        #endregion


        private static string WelcomeMessage()
        {
            string str = "";
            str += "\nBienvenue dans le solveur de QAP !\n";
            str += "\tEntrez 't' ou 'tai' pour changer d'instance de taillard à utiliser (actuel : " + filename + ")\n";
            str += "\tEntrez 's' ou 'seed' pour changer de seed (seed actuel : " + seed + " )\n\n";

            str += "\tEntrez 'r' ou 'rec' ou 'recuit' pour lancer un Recuit Simulé\n";
            str += "\tEntrez 'tab' ou 'tabou' pour lancer une Méthode Tabou\n";

            str += "\n\tEntrez 'x' ou 'e' ou 'exit' pour retourner au menu n'importe quand\n";
            str += "\tEntrez 'q' ou 'quit' pour quitter\n";

            return str;
                
        }

        private static string GetInstanceInfo()
        {
            return "Nom de l'instance : " + filename + "\n" +
                "Taille : " + qap.N + "\n" +
                "Meilleure solution : " + bestKnownSolution.ToString() + "\n" +
                "Fitness meilleur solution : " + bestKnownSolution.Fitness + "\n"
                ;
        }

        #region Test on String Input
        private static bool IsExit(string word)
        {
            return word.ToLower().Equals("x") || word.ToLower().Equals("e") || word.ToLower().Equals("exit");
        }

        private static bool IsQuit(string word)
        {
            return word.ToLower().Equals("q") || word.ToLower().Equals("quit");
        }

        private static bool IsValidation(string str) {
            return IsExitOrQuit(str) || str.ToLower().Equals("o") || str.ToLower().Equals("y") || str.ToLower().Equals("n");
        }

        private static bool IsYes(string str) {
            switch (str) {
                case "o":
                case "y":
                    return true;
                default:
                    return false;

            }
        }

        private static bool IsExitOrQuit(string word)
        {
            return IsExit(word) || IsQuit(word);
        }

        private static bool TryExitOrQuit(string word)
        {
            if (IsExit(word))
            {
                Console.WriteLine(returnToMenu);
                return true;
            }
            else if (IsQuit(word))
            {
                state = State.Quit;
                return true;
            }
            return false;
        }
        #endregion

        #region Get Names
        private static string GetNameFile()
        {
            return filename + "_" + algo.GetString() +  "_f" + bestFitness + "_s" + seed;
        }

        private static string GetNameCSV()
        {
            return GetNameFile() + ".csv";
        }

        private static string GetNameTxt()
        {
            return GetNameFile() + ".txt";
        }
        #endregion



        #region Get Inputs
        private static string[] GetLine()
        {
            string str = Console.ReadLine();
            string[] line = str.Split(new char[0]);

            //DEBUG :
            /*
            Console.Write("DEBUG: ");
            foreach (string st in line)
            {
                Console.Write(line + ", ");
            }
            Console.WriteLine(); */

            return line;
        }

        private static int GetIntegerFrom(string str)
        {
            str = Regex.Match(str, @"\d+").Value;
            return Int32.Parse(str);
        }

        private static int GetInt()
        {
            return GetInt(GetLine()[0]);
        }

        private static int GetInt(string word) {
            int number;
            if (Int32.TryParse(word, out number))
            {
                return number;
            }
            else
            {
                return -1;
            }
        }

        private static bool TryGetInt(string str, out int number)
        {
            return Int32.TryParse(str, out number);
        }

        private static bool TryGetDouble(string str, out double number)
        {
            return double.TryParse(str, out number);
        }


        private static double GetDouble() {
            return GetDouble(GetLine()[0]);
        }

        private static double GetDouble(string word)
        {
            double number;
            if (double.TryParse(word, out number))
            {
                return number;
            }
            else
            {
                return -1d;
            }
        }
        #endregion
    }

    public enum State
    {
        Menu, Seed, Quit,
    }

    public enum Algo
    {
        Recuit, GA, Tabou, RecuitGA
    }

    static class AlgoMethods
    {
        /// <summary>
        /// Custom To String
        /// </summary>
        /// <param name="algo"></param>
        /// <returns></returns>
        public static string GetString(this Algo algo)
        {
            switch (algo) {
                case Algo.Recuit:
                    return "recuit";
                case Algo.GA:
                    return "ga";
                case Algo.Tabou:
                    return "tabou";
                case Algo.RecuitGA:
                    return "recuitGA";
                default:
                    return "error";
            }
        }
    }
}
