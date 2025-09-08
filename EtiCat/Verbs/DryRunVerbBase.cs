using CommandLine;
using EtiCat.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    internal abstract class DryRunVerbBase : VerbBase
    {
        public DryRunVerbBase(VerbBase other) : base(other)
        {
        }

        protected DryRunVerbBase()
        {
        }

        [Option('d', "dry-run", HelpText = "If set, any changes to the file system are omitted")]
        public bool DryRun { get; set; }

        public override int Execute()
        {
            if (DryRun)
            {
                ProcessExecutor = new DryExecutor();
            }
            return base.Execute();
        }
    }
}
