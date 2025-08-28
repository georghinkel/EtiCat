using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public class Dependency
    {
        public Dependency(string target)
        {
            Target = target;
        }

        public string Target { get; }

        public Component? TargetComponent { get; }

        public bool IsLowerBound { get; set; }

        public bool IsUpperBound { get; set; }

        public Version? MinVersion { get; set; }

        public Version? MaxVersion { get; set; }
    }
}
