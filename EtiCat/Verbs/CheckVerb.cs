using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = EtiCat.Model.Version;

namespace EtiCat.Verbs
{

    [Verb("check", HelpText = "Checks whether there is a module history change for all changes in the current working tree")]
    internal class CheckVerb : VerbBase
    {
        public CheckVerb() : base() { }

        internal CheckVerb(VerbBase other) : base(other) { }

        [Option('f', "fix-dependency-updates", HelpText = "If set, EtiCat automatically updates version histories with a patch change if only dependency versions changed.")]
        public bool FixDependencyUpdates { get; set; }

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var filesAffected = new HashSet<string>(VersionControlServices!.FilesChangedSince(Baseline));

            var missingModuleChanges = new List<string>();
            foreach (var module in modules)
            {
                ConsoleWriter.WriteLine($"Checking {module.Name}");

                if (filesAffected.Contains(module.Path))
                {
                    continue;
                }

                if (filesAffected.Any(path => module.Folders.Any(f => path.StartsWith(f))))
                {
                    missingModuleChanges.Add(module.Name);
                }
                else
                {
                    var majorChangeUpdates = (from comp in module.Components
                                              from dep in comp.Dependencies
                                              where dep.TargetComponent != null
                                              where filesAffected.Contains(dep.TargetComponent!.Module!.Path)
                                              where IsIncompatibleChange(dep.Behavior, dep.TargetComponent!.Version)
                                              select comp.Name).ToList();

                    if (majorChangeUpdates.Count > 0)
                    {
                        Console.Error.WriteLine($"Components {string.Join(", ", majorChangeUpdates)} require a new dependency range. This should be reflected in the version history.");
                        missingModuleChanges.Add(module.Name);

                        if (FixDependencyUpdates)
                        {
                            File.AppendAllText(module.Path, $"patch: Update dependency version ranges for components {string.Join(", ", majorChangeUpdates)}" + Environment.NewLine);
                        }
                    } 
                }
            }

            if (missingModuleChanges.Count > 0)
            {
                throw new InvalidOperationException($"The following modules have changes that are not reflected by changes to the module history: {string.Join(", ", missingModuleChanges)}");
            }
        }

        private bool IsIncompatibleChange(DependencyBehavior behavior, Version version)
        {
            switch (behavior)
            {
                case DependencyBehavior.ExactVersion:
                    return true;
                case DependencyBehavior.ExactMajor:
                    return version.Minor == 0 && version.Patch == 0;
                case DependencyBehavior.ExactMinor:
                    return version.Patch == 0;
                default:
                    return false;
            }
        }
    }
}
