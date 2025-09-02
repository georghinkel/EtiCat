using CommandLine;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Verbs
{
    [Verb("tag", HelpText = "Calculates a tag name for the current commit")]
    internal class TagVerb : VerbBase
    {
        [Option('p', "prerelease", HelpText = "Sets the prerelease version information")]
        public string? Prerelease { get; set; }

        [Option('t', "template", HelpText = "The template for the tag name. Can include placeholders %module% and %version%", Default = "%module%/%version%")]
        public string TagTemplate { get; set; } = "%module%/%version%";

        public override void ExecuteCore(IReadOnlyCollection<Module> modules)
        {
            var tagBuilder = new StringBuilder();

            foreach (var module in modules)
            {
                if (module.IsChangedSinceBaseline)
                {
                    if (tagBuilder.Length > 0)
                    {
                        tagBuilder.Append(',');
                    }
                    tagBuilder.Append(TagTemplate.Replace("%module%", module.Name).Replace("%version%", module.Latest.Semver(Prerelease, null)));
                }
            }

            if (tagBuilder.Length > 0)
            {
                ConsoleWriter.WriteLine(NormalizeTag(tagBuilder.ToString()));
            }
            else
            {
                throw new InvalidOperationException("No tag should be created for the current commit because there are no relevant changes.");
            }
        }

        private string NormalizeTag(string tag)
        {
            var builder = new StringBuilder();
            var slashAllowed = false;
            var dotAllowed = false;
            foreach (var ch in tag)
            {
                switch (ch)
                {
                    case '~':
                    case ':':
                    case '[':
                    case '\\':
                    case '?':
                    case '^':
                    case '*':
                    case ' ':
                        continue;
                    case '/':
                        if (slashAllowed)
                        {
                            builder.Append(ch);
                            slashAllowed = false;
                            dotAllowed = true;
                        }
                        break;
                    case '.':
                        if (dotAllowed)
                        {
                            builder.Append(ch);
                            dotAllowed = false;
                            slashAllowed = true;
                        }
                        break;
                    default:
                        if (char.IsControl(ch)) continue;
                        slashAllowed = true;
                        dotAllowed = true;
                        builder.Append(ch);
                        break;
                }
            }
            return builder.ToString().TrimEnd('.').Replace("@{",string.Empty);
        }

        public override bool LoadChanges => true;
    }
}
