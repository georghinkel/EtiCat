using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    /// <summary>
    /// Denotes an object that spawns child processes
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        /// Executes the given command and checks that the exit code is 0
        /// </summary>
        /// <param name="command">the executable</param>
        /// <param name="arguments">arguments to pass</param>
        /// <param name="workingDirectory">the working directory</param>
        void ExecuteAndCheck(string command, string arguments, string? workingDirectory = null);
    }
}
