namespace Metaheuristic.GA
{
    using System;

    public class InvalidGAException : Exception
    {
        public InvalidGAException(string message) : base(message)
        {
            //Base Exception Constructor
        }
    }
}