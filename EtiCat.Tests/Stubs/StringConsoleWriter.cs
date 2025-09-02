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
        private string? _output;

        public void WriteLine(string message)
        {
            _output = message;
        }

        public override string ToString()
        {
            return _output!;
        }
    }
}
