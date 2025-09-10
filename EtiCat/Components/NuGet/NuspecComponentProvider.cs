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
        public string Name => "Nuget";

        public IEnumerable<ModuleParameter> SupportedParameters
        {
            get
            {
                yield return new ModuleParameter("dotnet_pack_args", "Parameters passed to the .NET CLI when building a package based on a nuspec file", "--no-build");
                yield return new ModuleParameter("nuget_pack_args", "Parameters passed to nuget if no MSBuild project is found to create the package with the .NET CLI", "-Symbols -SymbolPackageFormat snupkg");
            }
        }

        public bool CanProvideComponent(string path, string extension)
        {
            return extension == ".nuspec";
        }

        public void Flush(IProcessExecutor processExecutor)
        {
        }

        public Component ProvideComponent(string path)
        {
            return new NuspecComponent(this, path);
        }
    }
}
