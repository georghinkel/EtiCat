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
