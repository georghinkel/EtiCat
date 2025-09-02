using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Model
{
    public class VersionInfo
    {
        public VersionInfo(int line, string content)
        {
            Line = line;
            Content = content;
        }

        public string Content { get; }

        public int Line { get; }
    }
}
