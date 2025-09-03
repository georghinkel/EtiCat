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
    internal class PackVerb : VerbBase
    {
        public PackVerb() { }

        internal PackVerb(VerbBase verb) : base(verb) { }

        [Option('a', "affected", HelpText = "If set, only the affected packages are packed")]
        public bool OnlyAffected { get; set; }

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var layers = Layering<Module>.CreateLayers(modules.Where(m => !OnlyAffected || m.IsChangedSinceBaseline || m.IsTestOnlyChanges),
                m => m.CompileComponents.SelectMany(c => c.Dependencies.Select(d => d.TargetComponent?.Module!).Where(it => it != null)));

            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].Count > 1)
                {
                    throw new InvalidOperationException($"Found a cyclic dependency between modules {string.Join(", ", layers[i].Select(m => m.Name))}");
                }
                foreach (var component in layers[i].First().CompileComponents)
                {
                    ConsoleWriter.WriteLine($"Compiling {component.Name}");
                    component.Compile();
                }
            }
            for (int i = 0; i < layers.Count; i++)
            {
                foreach (var component in layers[i].First().TestComponents)
                {
                    ConsoleWriter.WriteLine($"Testing {component.Name}");
                    component.Test();
                }
            }
            for (int i = 0; i < layers.Count; i++)
            {
                foreach (var component in layers[i].First().PublishComponents)
                {
                    if (!OnlyAffected || component.Module.IsChangedSinceBaseline)
                    {
                        ConsoleWriter.WriteLine($"Packing {component.Name}");
                        component.Pack();
                    }
                }
            }
        }

        public override bool LoadChanges => OnlyAffected;
    }
}
