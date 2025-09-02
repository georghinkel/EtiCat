using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public abstract class Component
    {
        protected Component(string path) : this(path, null) { }

        protected Component(string path, string? name)
        {
            Path = path;
            Name = name ?? System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Name { get; }

        public abstract string Type { get; }

        public string Path { get; }

        public List<Component> BasedOn { get; } = new List<Component>();

        public List<Dependency> Dependencies { get; } = new List<Dependency>();

        public Module? Module { get; internal set; }

        public Version Version => Module?.Latest ?? default(Version);

        public bool IsChangedSinceBaseline { get; internal set; }

        public abstract void Apply(ExtendedVersionInfo versionInfo);

        public virtual void Compile() 
        {
            throw new InvalidOperationException($"Compilation is not supported for a component of type {Type} (compiling {Path})"); 
        }

        public virtual void Test()
        {
            throw new InvalidOperationException($"Executing is not supported for a component of type {Type} (attempting to test {Path})");
        }

        public virtual void Pack()
        {
            throw new InvalidOperationException($"Packing is not supported for a component of type {Type} (packing {Path})");
        }

        protected void ExecuteAndCheck(string command, string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
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
