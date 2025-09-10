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
        public string Name => "NPM";

        public IEnumerable<ModuleParameter> SupportedParameters
        {
            get
            {
                yield return new ModuleParameter("npm_build_target", "NPM target to use for compile tasks", "build");
                yield return new ModuleParameter("npm_test_target", "NPM target to use for test tasks", "test");
                yield return new ModuleParameter("npm_pack_target", "NPM target to use for pack tasks", "pack");
            }
        }

        public bool CanProvideComponent(string path, string extension)
        {
            return path.EndsWith("package.json");
        }

        public void Flush(IProcessExecutor processExecutor)
        {
        }

        public Component ProvideComponent(string path)
        {
            return new PackageJsonComponent(this, path);
        }
    }
}
