using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    /// <summary>
    /// Represents an object that supports components in EtiCat
    /// </summary>
    public interface IComponentProvider
    {
        /// <summary>
        /// Determies whether this provider can provide a component for the given path
        /// </summary>
        /// <param name="path">the path for which to create a component</param>
        /// <param name="extension">the extension of the path</param>
        /// <returns>true, if the current provider can support the given component</returns>
        bool CanProvideComponent(string path, string extension);

        /// <summary>
        /// Creates a component for the given path
        /// </summary>
        /// <param name="path">the path for which to create a component</param>
        /// <returns>the assembled component</returns>
        Component ProvideComponent(string path);

        /// <summary>
        /// Flushes any outstanding tasks
        /// </summary>
        /// <param name="processExecutor">a component to process subtasks</param>
        void Flush(IProcessExecutor processExecutor);

        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a collection of parameters
        /// </summary>
        IEnumerable<ModuleParameter> SupportedParameters { get; }
    }
}
