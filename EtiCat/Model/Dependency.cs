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

        public Component? TargetComponent { get; set; }

        public DependencyBehavior Behavior { get; set; }
    }
}
