using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Contracts
{
    public interface IVersionControlServices
    {
        string LastCommitIdentifier();

        string SecondLastCommitReference();

        int CommitsSince(string baseVersion);

        IEnumerable<string> FilesChangedSince(string baseVersion);
    }
}
