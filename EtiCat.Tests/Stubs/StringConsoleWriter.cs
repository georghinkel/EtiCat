using EtiCat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Tests.Stubs
{
    internal class StringConsoleWriter : IConsoleWriter
    {
        private StringBuilder _output = new StringBuilder();

        public void WriteLine(string message)
        {
            _output.AppendLine(message);
        }

        public override string ToString()
        {
            return _output.ToString();
        }

        public void Write(string message)
        {
            _output.Append(message);
        }
    }
}
