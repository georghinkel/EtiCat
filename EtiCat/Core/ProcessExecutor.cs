using EtiCat.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Core
{
    internal class ProcessExecutor : IProcessExecutor
    {
        public void ExecuteAndCheck(string command, string arguments, string? workingDirectory = null)
        {
            var process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            if (workingDirectory != null)
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }
            process.ErrorDataReceived += (_, e) =>
            {
                Console.Error.WriteLine(e.Data);
            };
            process.OutputDataReceived += (_, e) =>
            {
                Console.WriteLine(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Execution of '{command}' failed with exit code {process.ExitCode}.");
            }
        }
    }
}
