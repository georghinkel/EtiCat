using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using IOPath = System.IO.Path;

namespace EtiCat.Components.Csproj
{
    internal class MsbuildComponent : Component
    {
        private readonly XElement _tree;
        private readonly MsbuildComponentProvider _provider;

        public MsbuildComponent(MsbuildComponentProvider provider, string path) : base(path)
        {
            _tree = XElement.Load(path);
            _provider = provider;

            var directory = IOPath.GetDirectoryName(path);

            var dependencies = _tree.XPathSelectElements("//*[local-name()='ProjectReference']");
            foreach (var dependency in dependencies)
            {
                var dependencyPath = dependency.Attribute(XName.Get("Include"));
                if (dependencyPath != null)
                {
                    Dependencies.Add(new Dependency(IOPath.GetFullPath(IOPath.Combine(directory!, dependencyPath.Value))) { Behavior = DependencyBehavior.LowerBound });
                }
            }
        }

        public override string Type => "csproj";

        public override IComponentProvider Provider => _provider;

        public override void Apply(ExtendedVersionInfo versionInfo, bool dry)
        {
            ReplaceOrInsert("AssemblyVersion", $"{versionInfo.Version.Major}.0.0.0");

            var fullVersion = $"{versionInfo.Version.Major}.{versionInfo.Version.Minor}.{versionInfo.Version.Patch}.{versionInfo.BuildNumber.GetValueOrDefault(0)}";
            ReplaceOrInsert("FileVersion", fullVersion);
            ReplaceOrInsert("InformationalVersion", versionInfo.Commit != null ? fullVersion + "+" + versionInfo.Commit : fullVersion);
            ReplaceOrInsert("PackageVersion", versionInfo.Version.Semver(versionInfo.Prerelease, versionInfo.Commit));

            if (dry)
            {
                var writer = new StringWriter();
                _tree.WriteTo(XmlWriter.Create(writer));

                Console.WriteLine($"Write to {Path}:");
                Console.WriteLine(writer.ToString());
                Console.WriteLine();
            }
            else
            {
                _tree.Save(Path);
            }
        }

        private void ReplaceOrInsert(string name, string value)
        {
            var exactElement = _tree.XPathSelectElement("//*[local-name()='PropertyGroup']/*[local-name()='" + name + "']");
            if (exactElement == null)
            {
                var parentElement = _tree.XPathSelectElement("//*[local-name()='PropertyGroup']");
                if (parentElement == null)
                {
                    parentElement = new XElement(XName.Get("PropertyGroup"));
                    _tree.Add(parentElement);
                }
                exactElement = new XElement(XName.Get(name));
                parentElement.Add(exactElement);
            }
            exactElement.Value = value;
        }

        public override void Compile(IProcessExecutor processExecutor)
        {
            _provider.Schedule(Path, "build --configuration Release", processExecutor);
        }

        public override void Test(IProcessExecutor processExecutor)
        {
            _provider.Schedule(Path, "test --configuration Release", processExecutor);
        }

        public override void Pack(IProcessExecutor processExecutor)
        {
            _provider.Schedule(Path, "pack --no-build --include-symbols", processExecutor);
        }
    }
}
