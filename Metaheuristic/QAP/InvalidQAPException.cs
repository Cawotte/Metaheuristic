

namespace Metaheuristic.QAP
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    class InvalidQAPException : Exception
    {
        public InvalidQAPException(string message): base(message)
        {
            //Base Exception Constructor
        }
    }
}
