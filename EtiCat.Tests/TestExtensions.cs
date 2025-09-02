using EtiCat.Contracts;
using EtiCat.Tests.Stubs;
using EtiCat.Verbs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Tests
{
    internal static class TestExtensions
    {

        public static void SetupChangedFiles(this Mock<IVersionControlServices> mock, params string[] files)
        {
            mock.Setup(m => m.FilesChangedSince(It.IsAny<string>())).Returns(files.Select(Path.GetFullPath));
        }

        public static string? GetConsoleOutput(this VerbBase verb)
        {
            var writer = new StringConsoleWriter();
            verb.ConsoleWriter = writer;
            if (verb.Execute() == 0)
            {
                return writer.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
