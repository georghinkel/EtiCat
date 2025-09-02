using EtiCat.Contracts;
using EtiCat.Tests.Stubs;
using EtiCat.Verbs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Tests.Verbs
{
    [TestFixture]
    public class PublishTests
    {
        [Test]
        public void Publish_ReturnsPathsToAffectedComponents()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/A.history");

            var publish = new PublishVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(publish.GetConsoleOutput(), Is.EqualTo(Path.GetFullPath("Test/ComponentA/A.test")));
        }
    }
}
