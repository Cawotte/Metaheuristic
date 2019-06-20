using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic.Logs
{
    public interface ILogs
    {
        List<ILog> Logs
        {
            get;
        }

        ILog FinalLog { get; }

        int Size { get; }

        void SaveLogsTo(string path);
    }
}
