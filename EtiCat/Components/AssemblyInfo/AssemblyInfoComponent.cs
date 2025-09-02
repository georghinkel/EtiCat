using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EtiCat.Components.AssemblyInfo
{
    internal partial class AssemblyInfoComponent : Component
    {
        public override string Type => "assembly-info";

        public AssemblyInfoComponent(string path, string? name) : base(path, name)
        {
        }

        public override void Apply(ExtendedVersionInfo versionInfo)
        {
            var lines = File.ReadAllText(Path);

            ReplaceOrInsert(ref lines, "AssemblyVersion", $"{versionInfo.Version.Major}.0.0.0");

            var fullVersion = $"{versionInfo.Version.Major}.{versionInfo.Version.Minor}.{versionInfo.Version.Patch}.{versionInfo.BuildNumber.GetValueOrDefault(versionInfo.CommitsSinceBaseline)}";
            ReplaceOrInsert(ref lines, "AssemblyFileVersion", fullVersion);
            ReplaceOrInsert(ref lines, "AssemblyInformationalVersion", versionInfo.Commit != null ? fullVersion + "+" + versionInfo.Commit : fullVersion);

            File.WriteAllText(Path, lines);
        }

        private void ReplaceOrInsert(ref string contents, string attributeName, string value)
        {
            value = value.Replace("\"", "\\\"");
            var match = AssemblyAttributeRegex().Match(contents);
            while (match.Success)
            {
                var name = match.Groups["Name"].Value;
                var val = match.Groups["Value"];

                if (name == attributeName || name == attributeName + nameof(Attribute))
                {
                    contents = contents.Substring(0, val.Index) + value + contents.Substring(val.Index + val.Length);
                    return;
                }
                else
                {
                    match = match.NextMatch();
                }
            }
            var usingStmt = "System.Reflection.";
            if (contents.Contains("using System.Reflection;")) usingStmt = string.Empty;
            contents += Environment.NewLine + $"[assembly: {usingStmt}{attributeName}Attribute(\"{value}\")]";
        }

        [GeneratedRegex(@"\[assembly:\s*(System\.Reflection\.)?(?<Name>\w+)\(""(?<Value>[^""]*)""\)\]")]
        private static partial Regex AssemblyAttributeRegex();
    }
}
