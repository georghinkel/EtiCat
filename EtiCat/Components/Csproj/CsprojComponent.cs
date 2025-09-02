using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using IOPath = System.IO.Path;

namespace EtiCat.Components.Csproj
{
    internal class CsprojComponent : Component
    {
        private XElement _tree;

        public CsprojComponent(string path) : base(path)
        {
            _tree = XElement.Load(path);

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

        public override void Apply(ExtendedVersionInfo versionInfo)
        {
            ReplaceOrInsert("AssemblyVersion", $"{versionInfo.Version.Major}.0.0.0");

            var fullVersion = $"{versionInfo.Version.Major}.{versionInfo.Version.Minor}.{versionInfo.Version.Patch}.{versionInfo.BuildNumber.GetValueOrDefault(0)}";
            ReplaceOrInsert("FileVersion", fullVersion);
            ReplaceOrInsert("InformationalVersion", versionInfo.Commit != null ? fullVersion + "+" + versionInfo.Commit : fullVersion);
            ReplaceOrInsert("PackageVersion", versionInfo.Version.Semver(versionInfo.Prerelease, versionInfo.Commit));

            _tree.Save(Path);
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

        public override void Compile()
        {
            ExecuteAndCheck("dotnet", $"build --no-dependencies {Path} --configuration Release");
        }

        public override void Test()
        {
            ExecuteAndCheck("dotnet", $"test {Path} --configuration Release");
        }

        public override void Pack()
        {
            ExecuteAndCheck("dotnet", $"pack {Path} --no-build --include-symbols");
        }
    }
}
