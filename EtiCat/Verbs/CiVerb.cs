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
    internal class CiVerb : DryRunVerbBase
    {

        [Option('p', "prerelease", HelpText = "Sets the prerelease version information")]
        public string? Prerelease { get; set; }

        [Option('n', "build", HelpText = "Sets the build number")]
        public int? BuildNumber { get; set; }

        [Option('c', "commit", HelpText = "Sets the commit identifier")]
        public string? CommitId { get; set; }


        [Option("skip-build", HelpText = "If set, compilation steps are omitted")]
        public bool SkipBuild { get; set; }

        [Option("skip-test", HelpText = "If set, tests are omitted")]
        public bool SkipTest { get; set; }

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var check = new CheckVerb(this);
            check.ExecuteCore(modules);
            var apply = new ApplyVerb(this) { Prerelease = Prerelease, BuildNumber = BuildNumber, CommitId = CommitId };
            apply.ExecuteCore(modules);
            var pack = new PackVerb(this) 
            {
                OnlyAffected = true,
                SkipBuild = SkipBuild,
                SkipTest = SkipTest,
            };
            pack.ExecuteCore(modules);
        }

        public override bool LoadChanges => true;
    }
}
