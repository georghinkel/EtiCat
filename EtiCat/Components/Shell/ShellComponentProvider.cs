using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.Shell
{
    internal class ShellComponentProvider : IComponentProvider
    {
        public string Name => "Shell";

        public IEnumerable<ModuleParameter> SupportedParameters
        {
            get
            {
                yield return new ModuleParameter("shell_build", "Additional command-line argument executed for build tasks", "build");
                yield return new ModuleParameter("shell_test", "Additional command-line argument executed for test tasks", "test");
                yield return new ModuleParameter("shell_pack", "Additional command-line argument executed for pack tasks", "pack");
            }
        }

        public bool CanProvideComponent(string path, string extension)
        {
            return extension == ".sh";
        }

        public void Flush(IProcessExecutor processExecutor)
        {
        }

        public Component ProvideComponent(string path)
        {
            return new ShellComponent(path, this);
        }
    }
}
