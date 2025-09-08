using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    /// <summary>
    /// Denotes an object that provides version control services
    /// </summary>
    public interface IVersionControlServices
    {
        /// <summary>
        /// Gets the full identifier of the last commit
        /// </summary>
        /// <returns>A full identifier, e.g., commit-hash</returns>
        string LastCommitIdentifier();

        /// <summary>
        /// Gets a reference to the second-last commit
        /// </summary>
        /// <returns>A full identifier (e.g., commit hash) or a reference such as HEAD~</returns>
        string SecondLastCommitReference();

        /// <summary>
        /// Calculates the number of commits since the given baseline version
        /// </summary>
        /// <param name="baseVersion">the baseline version</param>
        /// <returns>the number of commits</returns>
        int CommitsSince(string baseVersion);

        /// <summary>
        /// Gets full paths of files that have been changed since the given baseline version
        /// </summary>
        /// <param name="baseVersion">the baseline version</param>
        /// <returns>A collection of full paths</returns>
        IEnumerable<string> FilesChangedSince(string baseVersion);
    }
}
