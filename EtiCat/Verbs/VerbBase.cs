using CommandLine;
using EtiCat.Components.AssemblyInfo;
using EtiCat.Components.Csproj;
using EtiCat.Components.NuGet;
using EtiCat.Components.PackageJson;
using EtiCat.Contracts;
using EtiCat.Core;
using EtiCat.Model;
using EtiCat.Services;
using EtiCat.VersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    internal abstract class VerbBase
    {
        protected VerbBase()
        {
            ComponentProviders = [new NuspecComponentProvider(), new CsprojComponentProvider(), new PackageJsonProvider(), new AssemblyInfoProvider()];
            VersionControlProviders = [new GitServiceProvider()];
            ConsoleWriter = new ConsoleWriter();
        }

        protected VerbBase(VerbBase other)
        {
            ComponentProviders = other.ComponentProviders;
            VersionControlProviders = other.VersionControlProviders;
            VersionControlServices = other.VersionControlServices;
            ConsoleWriter = other.ConsoleWriter;
            WorkingDirectory = other.WorkingDirectory;
            Baseline = other.Baseline;
        }

        [Option('w', "working-directory", HelpText = "Sets a different working directory", Required = false)]
        public string? WorkingDirectory { get; set; }

        [Option('b', "baseline", HelpText = "A commit specifier to specify the baseline commit. If there are no commits since the baseline, the system falls back to the second-last commit", Required = false, Default = "main")]
        public string Baseline { get; set; } = "main";

        public abstract void ExecuteCore(IReadOnlyCollection<Module> modules);

        public IEnumerable<IComponentProvider> ComponentProviders { get; internal set; }

        public IEnumerable<IVersionControlServiceProvider> VersionControlProviders { get; internal set; }

        public IVersionControlServices? VersionControlServices { get; internal set; }

        public IConsoleWriter ConsoleWriter { get; internal set; }

        public virtual bool LoadChanges => false;

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

            SetVersionControlServices();

            var reader = new ModuleReader(ComponentProviders);

            if (VersionControlServices!.CommitsSince(Baseline) == 0)
            {
                Baseline = VersionControlServices.SecondLastCommitReference();
            }

            try
            {
                var modules = reader.LoadModules(WorkingDirectory);
                if (LoadChanges)
                {
                    LoadChangesSinceBaseline(modules);
                }
                ExecuteCore(modules);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private void SetVersionControlServices()
        {
            if (VersionControlServices != null)
            {
                return;
            }

            foreach (var provider in VersionControlProviders)
            {
                if (provider.IsUsed(WorkingDirectory!))
                {
                    VersionControlServices = provider.GetServices(WorkingDirectory!);
                    return;
                }
            }

            throw new InvalidOperationException($"Directory {WorkingDirectory} is not part of a supported version control system.");
        }

        protected void LoadChangesSinceBaseline(IEnumerable<Module> modules)
        {
            var affected = new HashSet<string>(VersionControlServices!.FilesChangedSince(Baseline));

            foreach (var module in modules)
            {
                if (affected.Contains(module.Path))
                {
                    module.IsChangedSinceBaseline = true;
                    foreach (var comp in module.Components)
                    {
                        comp.IsChangedSinceBaseline = true;
                    }
                }
            }
        }
    }
}
