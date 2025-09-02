using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public record ExtendedVersionInfo(Version Version, string? Prerelease, int? BuildNumber, string? Commit, int CommitsSinceBaseline)
    {
        public string ToStringNoCommit() => Version.Semver(Prerelease, null);

        public override string ToString()
        {
            return Version.Semver(Prerelease, Commit);
        }
    }
}
