using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public class Module
    {
        public Module(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public string Path { get; }

        public string Name { get; }

        public List<string> Folders { get; } = new List<string>();

        public List<Component> Components { get; } = new List<Component>();
    }
}
