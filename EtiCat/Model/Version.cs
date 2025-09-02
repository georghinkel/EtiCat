using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public record struct Version(short Major, short Minor, short Patch) : IComparable<Version>
    {
        public int CompareTo(Version other)
        {
            if (Major > other.Major) return -1;
            if (Major < other.Major) return 1;
            if (Minor > other.Minor) return -1;
            if (Minor < other.Minor) return 1;
            return other.Patch - Patch;
        }

        public Version MajorChange() => new Version((short)(Major + 1), 0, 0);

        public Version MinorChange() => new Version(Major, (short)(Minor + 1), 0);

        public Version PatchChange() => new Version(Major, Minor, (short)(Patch + 1));

        public string Semver(string? prerelease, string? buildInfo)
        {
            var version = $"{Major}.{Minor}.{Patch}";
            if (!string.IsNullOrEmpty(prerelease))
            {
                version += "-" + prerelease.Trim();
            }
            if (!string.IsNullOrEmpty(buildInfo))
            {
                version += "+" + buildInfo.Trim();
            }
            return version;
        }

        public string NextMajor() => $"{Major + 1}.0";

        public string NextMinor() => $"{Major}.{Minor + 1}";
    }
}
