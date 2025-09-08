using EtiCat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Core
{
    internal class DryExecutor : IProcessExecutor
    {
        public void ExecuteAndCheck(string command, string arguments, string? workingDirectory = null)
        {
            Console.WriteLine($"Would execute '{command} {arguments}'");
        }
    }
}
