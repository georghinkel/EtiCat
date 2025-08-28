using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public record Version(int Major, int Minor, int Patch, int Build)
    {
        public Version MajorChange() => new Version(Major + 1, 0, 0, Build);

        public Version MinorChange() => new Version(Major, Minor + 1, 0, Build);

        public Version PatchChange() => new Version(Major, Minor, Patch + 1, Build);
    }
}
