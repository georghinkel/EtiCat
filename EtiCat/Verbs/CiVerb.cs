using CommandLine;
using EtiCat.Model;
using NMF.Analyses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("ci", HelpText = "Runs check, apply and pack in sequence")]
    internal class CiVerb : VerbBase
    {

        [Option('p', "prerelease", HelpText = "Sets the prerelease version information")]
        public string? Prerelease { get; set; }

        [Option('n', "build", HelpText = "Sets the build number")]
        public int? BuildNumber { get; set; }

        [Option('c', "commit", HelpText = "Sets the commit identifier")]
        public string? CommitId { get; set; }


        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var check = new CheckVerb(this);
            check.ExecuteCore(modules);
            var apply = new ApplyVerb(this) { Prerelease = Prerelease, BuildNumber = BuildNumber, CommitId = CommitId };
            apply.ExecuteCore(modules);
            var pack = new PackVerb(this) { OnlyAffected = true };
            pack.ExecuteCore(modules);
        }

        public override bool LoadChanges => true;
    }
}
