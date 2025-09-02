using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public enum DependencyBehavior
    {
        ExactVersion,
        ExactMajor,
        ExactMinor,
        LowerBound
    }
}
