using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("changelog", HelpText = "Creates changelogs for all modules")]
    internal class ChangeLogVerb : VerbBase
    {
        [Option('o', "output-directory", HelpText = "Output directory for changelogs. If no output directory is set, changelogs are generated next to the module files.")]
        public string? OutputPath { get; set; }

        [Option('f', "filename-pattern", HelpText = "Pattern to name output files.", Default = "%module%_changes.md")]
        public string FileNamePattern { get; set; } = "%module%_changes.md";

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            foreach (var module in modules)
            {
                ConsoleWriter.WriteLine($"Writing changelog for {module.Name}");

                var directory = OutputPath ?? Path.GetDirectoryName(module.Path)!;
                directory = directory.Replace("%module%", module.Name);
                var fileName = FileNamePattern.Replace("%module%", module.Name);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var writer = File.CreateText(Path.Combine(directory, fileName)))
                {
                    writer.WriteLine($"# {module.Name} Changelog");
                    writer.WriteLine();
                    foreach (var version in module.Versions)
                    {
                        writer.WriteLine($"## v{version.Key.Major}.{version.Key.Minor}.{version.Key.Patch}");
                        writer.WriteLine();
                        writer.WriteLine(version.Value.Content);
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}
