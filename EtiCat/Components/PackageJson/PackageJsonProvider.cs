using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.PackageJson
{
    internal class PackageJsonProvider : IComponentProvider
    {
        public bool CanProvideComponent(string path, string extension)
        {
            return path.EndsWith("package.json");
        }

        public Component ProvideComponent(string path)
        {
            return new PackageJsonComponent(path);
        }
    }
}
