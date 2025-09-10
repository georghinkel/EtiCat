using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    /// <summary>
    /// Denotes a parameter on module level, used by component providers
    /// </summary>
    /// <param name="Name">the name of the parameter</param>
    /// <param name="HelpText">a helpful description</param>
    /// <param name="DefaultValue">the default value</param>
    public record ModuleParameter(string Name, string HelpText, string DefaultValue)
    {
    }
}
