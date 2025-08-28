using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.Csproj
{
    internal class CsprojComponentProvider : IComponentProvider
    {
        public bool CanProvideComponent(string path, string extension)
        {
            return extension == ".csproj";
        }

        public Component ProvideComponent(string path)
        {
            return new CsprojComponent(path);
        }
    }
}
