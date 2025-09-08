using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.AssemblyInfo
{
    internal class AssemblyInfoProvider : IComponentProvider
    {
        public bool CanProvideComponent(string path, string extension)
        {
            return path.EndsWith("AssemblyInfo.cs");
        }

        public void Flush(IProcessExecutor processExecutor)
        {
        }

        public Component ProvideComponent(string path)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(path)!);
            if (directory.Name == "Properties")
            {
                directory = directory.Parent;
            }
            return new AssemblyInfoComponent(path, directory?.Name, this);
        }
    }
}
