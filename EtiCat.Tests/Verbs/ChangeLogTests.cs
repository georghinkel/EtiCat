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
    public class ChangeLogTests
    {
        [Test]
        public void ChangeLog_ReturnsCorrectChangelog()
        {
            var git = new Mock<IVersionControlServices>();

            var changelog = new ChangeLogVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()],
                OutputPath = "%module%",
                FileNamePattern = "%module%_changelog.md"
            };

            Assert.That(changelog.Execute(), Is.EqualTo(0));

            Assert.That(File.Exists("A/A_changelog.md"));
            Assert.That(File.Exists("B/B_changelog.md"));

            var aContents = File.ReadAllText("A/A_changelog.md");
            var bContents = File.ReadAllText("B/B_changelog.md");

            Assert.That(aContents.Contains("Feature 2"));
            Assert.That(bContents.Contains("Feature 3"));
        }
    }
}
