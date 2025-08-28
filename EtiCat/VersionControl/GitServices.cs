using EtiCat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Services
{
    internal class GitServices : IVersionControlServices
    {
        public int CommitsSince(string baseVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FilesChangedSince(string baseVersion)
        {
            throw new NotImplementedException();
        }
    }
}
