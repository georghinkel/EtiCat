using EtiCat.Contracts;
using EtiCat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.VersionControl
{
    internal class GitServiceProvider : IVersionControlServiceProvider
    {
        public IVersionControlServices GetServices(string path)
        {
            return new GitServices();
        }

        public bool IsUsed(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            while (dirInfo != null)
            {
                if (dirInfo.EnumerateDirectories().Any(d => d.Name == ".git"))
                {
                    return true;
                }
                dirInfo = dirInfo.Parent;
            }
            return false;
        }
    }
}
