using EtiCat.Contracts;
using EtiCat.Core;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using IOPath = System.IO.Path;

namespace EtiCat.Components.NuGet
{
    internal class NuspecComponent : Component
    {
        private const string NuspecXmlns = "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd";

        private readonly XElement _tree;

        private readonly NuspecComponentProvider _provider;

        public override string Type => "nuspec";

        public override IComponentProvider Provider => _provider;

        private NuspecComponent(NuspecComponentProvider provider, string path, XElement tree)
            : base(path, ExtractId(tree))
        {
            _provider = provider;

            var dependencies = tree.XPathSelectElements("//*[local-name()='dependency']");
            foreach (var dependency in dependencies)
            {
                var depString = (dependency.Attribute(XName.Get("id", NuspecXmlns)) ?? dependency.Attribute(XName.Get("id")))?.Value;
                if (depString != null)
                {
                    var dep = Dependencies.FirstOrDefault(d => d.Target == depString);
                    if (dep == null)
                    {
                        var versionString = (dependency.Attribute(XName.Get("version", NuspecXmlns)) ?? dependency.Attribute(XName.Get("version")))?.Value;


                        Dependencies.Add(new Dependency(depString)
                        {
                            Behavior = GetBehaviour(versionString)
                        });
                    }
                } 
            }

            _tree = tree;
        }

        private static string? ExtractId(XElement tree)
        {
            var element = tree.XPathSelectElement("//*[local-name()='id']");
            return element?.Value;
        }

        private static DependencyBehavior GetBehaviour(string? versionString)
        {
            if (versionString == null || !versionString.StartsWith('['))
            {
                return DependencyBehavior.LowerBound;
            }
            
            if (versionString!.EndsWith(".0)"))
            {
                return DependencyBehavior.ExactMajor;
            }
            else if (versionString.EndsWith("]"))
            {
                return DependencyBehavior.ExactVersion;
            }

            return DependencyBehavior.ExactMinor;
        }

        public NuspecComponent(NuspecComponentProvider provider, string path) : this(provider, path, XElement.Load(path))
        {
        }

        public override void Apply(ExtendedVersionInfo versionInfo, bool dry)
        {
            var version = _tree.XPathSelectElement("//*[local-name()='version']");
            if (version != null)
            {
                version.Value = versionInfo.ToStringNoCommit();
            }

            var dependencies = _tree.XPathSelectElements("//*[local-name()='dependency']");
            foreach (var dependency in dependencies)
            {
                var depString = (dependency.Attribute(XName.Get("id", NuspecXmlns)) ?? dependency.Attribute(XName.Get("id")))?.Value;
                if (depString != null)
                {
                    var dep = Dependencies.FirstOrDefault(d => d.Target == depString);
                    if (dep != null && dep.TargetComponent != null)
                    {
                        var current = dep.TargetComponent.IsChangedSinceBaseline ? dep.TargetComponent.Version.Semver(versionInfo.Prerelease, null) : dep.TargetComponent.Version.Semver(null, null);
                        var versionAttribute = dependency.Attribute(XName.Get("version", NuspecXmlns)) ?? dependency.Attribute(XName.Get("version"));
                        if (versionAttribute == null)
                        {
                            versionAttribute = new XAttribute(XName.Get("version", NuspecXmlns), string.Empty);
                            dependency.Add(versionAttribute);
                        }
                        switch (dep.Behavior)
                        {
                            case DependencyBehavior.ExactMajor:
                                versionAttribute.Value = $"[{current},{dep.TargetComponent.Version.NextMajor()})";
                                break;
                            case DependencyBehavior.ExactMinor:
                                versionAttribute.Value = $"[{current},{dep.TargetComponent.Version.NextMinor()})";
                                break;
                            case DependencyBehavior.ExactVersion:
                                versionAttribute.Value = $"[{current}]";
                                break;
                            default:
                                dependency.Value = current;
                                break;
                        }
                    }
                }
            }

            var repository = (_tree.XPathEvaluate("//*[local-name()='repository']/@commit") as IEnumerable<object>)?.OfType<XAttribute>();
            if (repository != null && versionInfo.Commit != null)
            {
                foreach (var repoInfo in repository)
                {
                    repoInfo.Value = versionInfo.Commit;
                }
            }

            if (dry)
            {
                var writer = new StringWriter();
                _tree.WriteTo(XmlWriter.Create(writer));
                Console.WriteLine($"Would write to {Path}:");
                Console.WriteLine(writer.ToString());
                Console.WriteLine();
            }
            else
            {
                _tree.Save(Path);
            }
        }

        public override void Pack(IProcessExecutor processExecutor)
        {
            var csproj = Module?.Components.FirstOrDefault(c => c.Type == "csproj")?.Path;

            if (csproj == null)
            {
                Console.WriteLine($"Module {Module?.Name} contains no csproj, therefore using NuGet.exe to pack {Path}");
                processExecutor.ExecuteAndCheck("nuget", $"pack {Path} -Symbols -SymbolPackageFormat snupkg");
            }
            else
            {
                processExecutor.ExecuteAndCheck("dotnet", $"pack --no-build --include-symbols -p:NuspecFile={Path} -p:NuspecBasePath={IOPath.GetDirectoryName(Path)} {csproj}");
            }
        }
    }
}
