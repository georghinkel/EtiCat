using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Version = EtiCat.Model.Version;

namespace EtiCat.Core
{
    internal partial class ModuleReader
    {
        private readonly IEnumerable<IComponentProvider> _componentProviders;

        public ModuleReader(IEnumerable<IComponentProvider> componentProviders)
        {
            _componentProviders = componentProviders;
        }

        public IReadOnlyCollection<Module> LoadModules(string workingDirectory)
        {
            var result = new List<Module>();
            var histories = Directory.GetFiles(workingDirectory, "*.history", SearchOption.AllDirectories);
            var componentDict = new Dictionary<string, Component>();

            foreach (var file in histories)
            {
                try
                {
                    var module = ReadModule(file);
                    result.Add(module);

                    foreach (var component in module.Components)
                    {
                        componentDict[component.Name] = component;
                        componentDict[component.Path] = component;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error reading history file {file}. Ignoring this module. Error message was: {ex.Message}");
                }
            }

            foreach (var module in result)
            {
                foreach (var component in module.Components)
                {
                    foreach (var dependency in component.Dependencies)
                    {
                        if (componentDict.TryGetValue(dependency.Target, out var resolvedComponent))
                        {
                            dependency.TargetComponent = resolvedComponent;
                        }
                    }
                }
            }

            return result;
        }

        private Module ReadModule(string path)
        {
            var lines = File.ReadAllLines(path);
            var module = null as Module;

            var header = true;
            var version = default(Version);
            var sb = new StringBuilder();
            var lastLineNo = 0;
            var directory = Path.GetDirectoryName(path)!;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (header && string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("module ", StringComparison.OrdinalIgnoreCase))
                {
                    if (module != null)
                    {
                        throw new InvalidOperationException($"Unexpected module declaration in line {i}");
                    }
                    module = new Module(path, line.Substring(7));
                    continue;
                }
                if (module == null)
                {
                    module = new Module(path, Path.GetFileNameWithoutExtension(path));
                }
                if (header)
                {
                    if (ProcessHeader(module, directory, line)) continue;
                }
                var match = GetVersionRegex().Match(line);
                if (match.Success)
                {
                    if (!header)
                    {
                        module.Versions[version] = new VersionInfo(lastLineNo, sb.ToString());
                        sb.Clear();
                    }
                    AdjustVersion(match, ref version);
                    header = false;
                    lastLineNo = i;
                    sb.Append(line.Substring(match.Length).Trim());
                }
                else if (!header)
                {
                    sb.AppendLine(line);
                } 
                else
                {
                    throw new InvalidOperationException($"Failed to read module information at line {i} in {path}.");
                }
            }

            if (module == null) throw new InvalidOperationException("Module declaration not found in " + path);

            module.Versions[version] = new VersionInfo(lastLineNo, sb.ToString());
            module.Latest = version;

            return module;
        }

        private void AdjustVersion(Match match, ref Version version)
        {
            if (match.Groups["major"].Success)
            {
                var major = short.Parse(match.Groups["major"].Value);
                var minor = short.Parse(match.Groups["minor"].Value);
                var patchGroup = match.Groups["patch"];
                var patch = patchGroup != null ? short.Parse(patchGroup.Value) : (short)0;
                version = new Version(major, minor, patch);
            }
            else
            {
                var op = match.Groups["op"].Value;
                switch (op.ToLowerInvariant())
                {
                    case "major":
                        version = version.MajorChange();
                        break;
                    case "minor":
                        version = version.MinorChange();
                        break;
                    case "patch":
                        version = version.PatchChange();
                        break;
                }
            }
        }

        [GeneratedRegex(@"^(?<op>(major|minor|patch))\s*(\((?<major>\d+)\.(?<minor>\d+)(\.(?<patch>\d+))?\)\s*)?:", RegexOptions.IgnoreCase)]
        private static partial Regex GetVersionRegex();

        private bool ProcessHeader(Module module, string directory, string line)
        {
            if (line.StartsWith("folder ", StringComparison.OrdinalIgnoreCase))
            {
                module.Folders.Add(Path.GetFullPath(Path.Combine(directory, line.Substring(7))));
                return true;
            }
            if (line.StartsWith("testFolder ", StringComparison.OrdinalIgnoreCase))
            {
                module.TestFolders.Add(Path.GetFullPath(Path.Combine(directory, line.Substring(11))));
                return true;
            }
            if (line.StartsWith("component ", StringComparison.OrdinalIgnoreCase))
            {
                module.PublishComponents.Add(ParseComponent(module, directory, line, 10));
                return true;
            }
            if (line.StartsWith("compile ", StringComparison.OrdinalIgnoreCase))
            {
                module.CompileComponents.Add(ParseComponent(module, directory, line, 8));
                return true;
            }
            if (line.StartsWith("test ", StringComparison.OrdinalIgnoreCase))
            {
                module.TestComponents.Add(ParseComponent(module, directory, line, 5));
                return true;
            }
            if (line.StartsWith("use ", StringComparison.OrdinalIgnoreCase))
            {
                var index = line.IndexOf('=', 4);
                if (index == -1)
                {
                    module.Settings[line.Substring(4).Trim()] = "true";
                } 
                else
                {
                    module.Settings[line.Substring(4, index - 4).Trim()] = line.Substring(index + 1).Trim();
                }
            }
            return false;
        }

        private Component ParseComponent(Module module, string directory, string line, int start)
        {
            string componentPath;
            var dependent = Enumerable.Empty<string>();
            var fromIndex = line.IndexOf(" from ", start, StringComparison.OrdinalIgnoreCase);
            if (fromIndex == -1)
            {
                componentPath = line.Substring(start);
            }
            else
            {
                componentPath = line.Substring(start, fromIndex - start);
                var dependencies = line.Substring(fromIndex + 6);
                dependent = dependencies.Split(",").Select(dep => Path.GetFullPath(Path.Combine(directory, dep.Trim())));
            }
            componentPath = Path.GetFullPath(Path.Combine(directory, componentPath));

            var component = CreateComponent(componentPath, dependent, module);
            return component;
        }

        private Component CreateComponent(string componentPath, IEnumerable<string> dependent, Module module)
        {
            var existing = module.Components.FirstOrDefault(c => c.Path == componentPath);
            if (existing != null) { return existing; }
            var extension = Path.GetExtension(componentPath);
            foreach (var provider in _componentProviders)
            {
                if (provider.CanProvideComponent(componentPath, extension))
                {
                    var component = provider.ProvideComponent(componentPath);
                    module.Components.Add(component);
                    component.Module = module;
                    foreach (var dependency in dependent)
                    {
                        component.BasedOn.Add(CreateComponent(dependency, Enumerable.Empty<string>(), module));
                    }
                    return component;
                }
            }
            throw new InvalidOperationException($"Component {componentPath} is not supported.");
        }
    }
}
