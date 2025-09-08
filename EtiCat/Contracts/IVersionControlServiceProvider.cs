using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    /// <summary>
    /// Denotes an object that integrates version control systems
    /// </summary>
    public interface IVersionControlServiceProvider
    {
        /// <summary>
        /// Determines whether the version control system is used in the given path
        /// </summary>
        /// <param name="path">the root path of EtiCat</param>
        /// <returns>true, if the given path is under version control of the given provider</returns>
        bool IsUsed(string path);

        /// <summary>
        /// Creates version control services for the given path
        /// </summary>
        /// <param name="path">the root path</param>
        /// <returns>an object to return version control services</returns>
        IVersionControlServices GetServices(string path);
    }
}
