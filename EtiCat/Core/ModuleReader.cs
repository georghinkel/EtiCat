using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Core
{
    internal class ModuleReader
    {
        public IReadOnlyCollection<Module> LoadModules(string workingDirectory)
        {
            var result = new List<Module>();
            var histories = Directory.GetFiles(workingDirectory, "*.history", SearchOption.AllDirectories);

            // iterate history files, set versions

            // resolve dependencies

            return result;
        }
    }
}
