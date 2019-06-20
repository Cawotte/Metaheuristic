using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.QAP
{
    public class BadTypeException : Exception
    {
        public BadTypeException() : base("Parameters is of the wrong type !")
        {
            //Base Exception Constructor
        }
    }
}
