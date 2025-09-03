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

        public bool IsChangedSinceBaseline { get; set; }

        public bool IsTestOnlyChanges { get; set; }

        public List<string> Folders { get; } = new List<string>();

        public List<string> TestFolders { get; } = new List<string>();

        public List<Component> Components { get; } = new List<Component>();

        public List<Component> PublishComponents { get; } = new List<Component>();

        public List<Component> CompileComponents { get; } = new List<Component>();

        public List<Component> TestComponents { get; } = new List<Component>();

        public SortedDictionary<Version, VersionInfo> Versions { get; } = new SortedDictionary<Version, VersionInfo>();

        public Version Latest { get; set; }
    }
}
