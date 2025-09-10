using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    /// <summary>
    /// Denotes a component to write to the console (used for testing purposes)
    /// </summary>
    public interface IConsoleWriter
    {
        /// <summary>
        /// Writes the given line without line break
        /// </summary>
        /// <param name="message">the message</param>
        void Write(string message);

        /// <summary>
        /// Writes the given line
        /// </summary>
        /// <param name="message">the message</param>
        void WriteLine(string message);
    }
}
