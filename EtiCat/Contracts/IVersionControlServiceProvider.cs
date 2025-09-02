using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    public interface IVersionControlServiceProvider
    {
        bool IsUsed(string path);

        IVersionControlServices GetServices(string path);
    }
}
