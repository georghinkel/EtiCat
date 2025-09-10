using CommandLine;
using EtiCat.Contracts;
using EtiCat.Model;
using NMF.Analyses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("pack", HelpText = "Packs modules")]
    internal class PackVerb : DryRunVerbBase
    {
        public PackVerb() { }

        internal PackVerb(VerbBase verb) : base(verb) { }

        [Option('a', "affected", HelpText = "If set, only the affected packages are packed")]
        public bool OnlyAffected { get; set; }

        [Option("skip-build", HelpText = "If set, compilation steps are omitted")]
        public bool SkipBuild { get; set; }

        [Option("skip-test", HelpText = "If set, tests are omitted")]
        public bool SkipTest { get; set; }

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var layers = Layering<Module>.CreateLayers(modules.Where(m => !OnlyAffected || m.IsChangedSinceBaseline || m.IsTestOnlyChanges),
                m => m.CompileComponents.SelectMany(c => c.Dependencies.Select(d => d.TargetComponent?.Module!).Where(it => it != null)));

            if (!SkipBuild) Compile(layers);
            if (!SkipTest) Test(modules);
            Pack(modules);
        }

        private void Pack(IReadOnlyCollection<Module> modules)
        {
            IComponentProvider? _lastProvider = null;
            var somethingToPack = false;
            foreach (var module in modules.Where(m => !OnlyAffected || m.IsChangedSinceBaseline))
            {
                foreach (var component in module.PublishComponents)
                {
                    somethingToPack = true;
                    FlushProvider(ref _lastProvider, component);
                    ConsoleWriter.WriteLine($"Packing {component.Name}");
                    component.Pack(ProcessExecutor);
                }
            }
            _lastProvider?.Flush(ProcessExecutor);
            if (!somethingToPack)
            {
                ConsoleWriter.WriteLine("No components needed to be packed.");
            }
        }

        private void Test(IReadOnlyCollection<Module> modules)
        {
            IComponentProvider? _lastProvider = null;
            var somethingToTest = false;
            foreach (var module in modules.Where(m => !OnlyAffected || m.IsChangedSinceBaseline || m.IsTestOnlyChanges))
            {
                foreach (var component in module.TestComponents)
                {
                    somethingToTest = true;
                    FlushProvider(ref _lastProvider, component);
                    ConsoleWriter.WriteLine($"Testing {component.Name}");
                    component.Test(ProcessExecutor);
                }
            }
            _lastProvider?.Flush(ProcessExecutor);
            if (!somethingToTest)
            {
                ConsoleWriter.WriteLine("No components needed to be tested.");
            }
        }

        private void Compile(IList<ICollection<Module>> layers)
        {
            IComponentProvider? _lastProvider = null;
            var somethingToCompile = false;
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].Count > 1)
                {
                    throw new InvalidOperationException($"Found a cyclic dependency between modules {string.Join(", ", layers[i].Select(m => m.Name))}");
                }
                foreach (var component in layers[i].First().CompileComponents)
                {
                    somethingToCompile = true;
                    FlushProvider(ref _lastProvider, component);
                    ConsoleWriter.WriteLine($"Compiling {component.Name}");
                    component.Compile(ProcessExecutor);
                }
            }
            _lastProvider?.Flush(ProcessExecutor);
            if (!somethingToCompile)
            {
                ConsoleWriter.WriteLine("No components needed to be built.");
            }
        }

        private void FlushProvider(ref IComponentProvider? _lastProvider, Component component)
        {
            if (component.Provider != _lastProvider)
            {
                _lastProvider?.Flush(ProcessExecutor);
                _lastProvider = component.Provider;
            }
        }

        public override bool LoadChanges => OnlyAffected;
    }
}
