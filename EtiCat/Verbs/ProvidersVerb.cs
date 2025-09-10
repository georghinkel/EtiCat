using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("providers", HelpText = "Lists the supported providers with their supported module parameters")]
    internal class ProvidersVerb : VerbBase
    {
        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            foreach (var provider in ComponentProviders)
            {
                ConsoleWriter.WriteLine($"Provider {provider.Name}:");
                ConsoleWriter.WriteLine(" Supported Parameters:");
                foreach (var parameter in provider.SupportedParameters)
                {
                    ConsoleWriter.WriteLine($" - {parameter.Name} (default: {parameter.DefaultValue}): {parameter.HelpText}");
                }
                ConsoleWriter.WriteLine(" Used By:");
                var isUsed = false;
                foreach (var component in modules.SelectMany(m => m.Components).Where(c => c.Provider == provider))
                {
                    isUsed = true;
                    ConsoleWriter.WriteLine($" - {component.Name} ({component.Path})");
                }
                if (!isUsed)
                {
                    ConsoleWriter.WriteLine(" - (currently not used)");
                }
                ConsoleWriter.WriteLine(string.Empty);
            }
        }
    }
}
