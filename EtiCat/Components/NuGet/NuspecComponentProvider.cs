using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.NuGet
{
    internal class NuspecComponentProvider : IComponentProvider
    {
        public bool CanProvideComponent(string path, string extension)
        {
            return extension == ".nuspec";
        }

        public Component ProvideComponent(string path)
        {
            return new NuspecComponent(path);
        }
    }
}
