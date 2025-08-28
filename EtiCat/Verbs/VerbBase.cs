using CommandLine;
using EtiCat.Contracts;
using EtiCat.Core;
using EtiCat.Model;
using EtiCat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    internal abstract class VerbBase
    {
        public VerbBase()
        {
            VersionControlServices = new GitServices();
        }

        [Option('w', "working-directory", HelpText = "Sets a different working directory", Required = false)]
        public string? WorkingDirectory { get; set; }

        protected abstract void ExecuteCore(IReadOnlyCollection<Module> modules);

        public IVersionControlServices VersionControlServices { get; }

        public int Execute()
        {
            if (WorkingDirectory == null)
            {
                WorkingDirectory = Environment.CurrentDirectory;
            }
            else
            {
                WorkingDirectory = Path.GetFullPath(WorkingDirectory);
            }

            var reader = new ModuleReader();

            try
            {
                var modules = reader.LoadModules(WorkingDirectory);
                ExecuteCore(modules);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
