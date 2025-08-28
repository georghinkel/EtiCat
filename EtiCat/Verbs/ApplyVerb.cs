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
        protected override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            foreach (var module in modules)
            {
                foreach (var component in module.Components)
                {
                    component.Apply();
                }
            }
        }
    }
}
