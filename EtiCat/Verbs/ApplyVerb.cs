using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("apply", true, HelpText = "Applies the version histories")]
    internal class ApplyVerb : VerbBase
    {
        public ApplyVerb() : base() { }

        internal ApplyVerb(VerbBase other) : base(other) { }

        [Option('p', "prerelease", HelpText = "Sets the prerelease version information")]
        public string? Prerelease { get; set; }

        [Option('n', "build", HelpText = "Sets the build number")]
        public int? BuildNumber { get; set; }

        [Option('c', "commit", HelpText = "Sets the commit identifier")]
        public string? CommitId { get; set; }

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            if (CommitId == null)
            {
                CommitId = VersionControlServices!.LastCommitIdentifier();
            }

            foreach (var module in modules)
            {
                ConsoleWriter.WriteLine($"Applying versions for {module.Name}");

                var version = new ExtendedVersionInfo(module.Latest, Prerelease, BuildNumber, CommitId, VersionControlServices!.CommitsSince(Baseline));

                foreach (var component in module.Components)
                {
                    component.Apply(version);
                }
            }
        }

        public override bool LoadChanges => true;
    }
}
