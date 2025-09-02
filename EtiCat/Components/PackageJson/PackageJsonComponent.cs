using EtiCat.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EtiCat.Components.PackageJson
{
    internal class PackageJsonComponent : Component
    {
        public override string Type => "npm";

        public PackageJsonComponent(string path) : this(path, JObject.Parse(File.ReadAllText(path)))
        {            
        }

        private PackageJsonComponent(string path, JObject tree) : base(path, tree.Property("name")?.Value<string>())
        {
            var dependencies = tree.Property("dependencies")?.Value as JObject;
            if (dependencies != null)
            {
                foreach (var dependency in dependencies.Properties())
                {
                    Dependencies.Add(new Dependency(dependency.Name)
                    {
                        Behavior = ReadBehavior(dependency.Value<string>())
                    });
                }
            }
        }

        private DependencyBehavior ReadBehavior(string? versionString)
        {
            if (versionString == null || versionString.StartsWith('>')) return DependencyBehavior.LowerBound;

            if (versionString.StartsWith('~')) return DependencyBehavior.ExactMinor;
            if (versionString.StartsWith('^')) return DependencyBehavior.ExactMajor;

            return DependencyBehavior.ExactVersion;
        }

        public override void Apply(ExtendedVersionInfo versionInfo)
        {
            throw new NotImplementedException();
        }
    }
}
