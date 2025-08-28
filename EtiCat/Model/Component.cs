using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public abstract class Component
    {
        protected Component(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public Component? BasedOn { get; set; }

        public List<Dependency> Dependencies { get; } = new List<Dependency>();

        public Version? Version { get; set; }

        public abstract void Apply();
    }
}
