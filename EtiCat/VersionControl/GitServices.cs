using EtiCat.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Services
{
    internal class GitServices : IVersionControlServices
    {
        public int CommitsSince(string baseVersion)
        {
            var response = CallGit($"rev-list {baseVersion}..HEAD --count", "obtaining number of commits");
            return string.IsNullOrEmpty(response) ? 0 : int.Parse(response);
        }

        public IEnumerable<string> FilesChangedSince(string baseVersion)
        {
            return CallGit($"diff --name-only {baseVersion} HEAD", "obtaining affected files")
                .Split('\n')
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => Path.GetFullPath(p.Trim()));
        }

        public string LastCommitIdentifier()
        {
            return CallGit("rev-parse HEAD", "obtaining the latest commit").Trim('\r', '\n');
        }

        public string SecondLastCommitReference()
        {
            return "HEAD~";
        }

        private string CallGit(string command, string explanation)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            try
            {
                var process = new Process();
                process.StartInfo = processStartInfo;
                var sb = new StringBuilder();
                process!.OutputDataReceived += (_, e) =>
                {
                    sb.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (_, e) =>
                {
                    Console.Error.WriteLine(e.Data);
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error {explanation} (executing 'git {command}'): {ex.Message}", ex);
            }
        }
    }
}
