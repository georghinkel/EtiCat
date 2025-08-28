using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{

    [Verb("check", HelpText = "Checks whether there is a module history change for all changes in the current working tree")]
    internal class CheckVerb : VerbBase
    {
        [Option('b', "baseline", HelpText = "A commit specifier to specify the baseline commit", Required = false, Default = "main")]
        public string Baseline { get; set; } = "main";

        protected override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var filesAffected = VersionControlServices.FilesChangedSince(Baseline);

            var missingModuleChanges = new List<string>();
            foreach (var module in modules)
            {
                if (filesAffected.Contains(module.Path))
                {
                    continue;
                }

                var directory = Path.GetDirectoryName(module.Path);

                if (directory != null && filesAffected.Any(path => path.StartsWith(directory)))
                {
                    missingModuleChanges.Add(module.Name);
                }
            }

            if (missingModuleChanges.Count > 0)
            {
                throw new InvalidOperationException($"The following modules have changes that are not reflected by changes to the module history: {string.Join(", ", missingModuleChanges)}");
            }
        }
    }
}
