using EtiCat.Contracts;
using EtiCat.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EtiCat.Components.PackageJson
{
    internal class PackageJsonComponent : Component
    {
        private readonly PackageJsonProvider _provider;
        private readonly JObject _tree;

        public override string Type => "npm";

        public override IComponentProvider Provider => _provider;

        public PackageJsonComponent(PackageJsonProvider provider, string path) : this(provider, path, JObject.Parse(File.ReadAllText(path)))
        {            
        }

        private PackageJsonComponent(PackageJsonProvider provider, string path, JObject tree) : base(path, tree.Property("name")?.Value<string>())
        {
            _provider = provider;
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
            _tree = tree;
        }

        private DependencyBehavior ReadBehavior(string? versionString)
        {
            if (versionString == null || versionString.StartsWith('>')) return DependencyBehavior.LowerBound;

            if (versionString.StartsWith('~')) return DependencyBehavior.ExactMinor;
            if (versionString.StartsWith('^')) return DependencyBehavior.ExactMajor;

            return DependencyBehavior.ExactVersion;
        }

        public override void Apply(ExtendedVersionInfo versionInfo, bool dry)
        {
            TextWriter writer = dry ? new StringWriter() : File.CreateText(Path);
            using (writer)
            {
                _tree.Property("version")!.Value = JToken.FromObject(versionInfo.Version.Semver(versionInfo.Prerelease, null));
                JsonSerializer.Create(new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                }).Serialize(writer, _tree);

                if (dry)
                {
                    Console.WriteLine($"Would write to {Path}:");
                    Console.WriteLine(writer.ToString());
                    Console.WriteLine();
                }
            }
        }

        private string? Directory => System.IO.Path.GetDirectoryName(Path);

        public override void Compile(IProcessExecutor processExecutor)
        {
            processExecutor.ExecuteAndCheck("npm", "install", Directory);
            processExecutor.ExecuteAndCheck("npm", "run " + Module!.GetSettingOrDefault("npm_build_target", "build"), Directory);
        }

        public override void Test(IProcessExecutor processExecutor)
        {
            processExecutor.ExecuteAndCheck("npm", "run " + Module!.GetSettingOrDefault("npm_test_target", "test"), Directory);
        }

        public override void Pack(IProcessExecutor processExecutor)
        {
            processExecutor.ExecuteAndCheck("npm", "run " + Module!.GetSettingOrDefault("npm_pack_target", "pack"), Directory);
        }
    }
}
