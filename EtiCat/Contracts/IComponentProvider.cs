using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    public interface IComponentProvider
    {
        bool CanProvideComponent(string path, string extension);

        Component ProvideComponent(string path);
    }
}
