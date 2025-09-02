using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("publish", HelpText = "Publishes all packages that changes since the given baseline")]
    internal class PublishVerb : VerbBase
    {
        [Option('t', "type", HelpText = "Limits the output to artifacts of the given type")]
        public string? Type { get; set; }

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            foreach (var component in from m in modules
                                      where m.IsChangedSinceBaseline
                                      from c in m.PublishComponents
                                      where (Type == null || string.Equals(Type, c.Type, StringComparison.OrdinalIgnoreCase))
                                      select c)
            {
                ConsoleWriter.WriteLine(component.Path);
            }
        }

        public override bool LoadChanges => true;
    }
}
