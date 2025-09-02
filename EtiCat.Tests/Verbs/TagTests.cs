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
    public class TagTests
    {
        [Test]
        public void Tag_NoChanges_ReturnsNoTag()
        {
            var git = new Mock<IVersionControlServices>();

            var tag = new TagVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(tag.GetConsoleOutput(), Is.Null);
        }

        [Test]
        public void Tag_NoPrerelease_ReturnsCorrectTag()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/A.history");

            var tag = new TagVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(tag.GetConsoleOutput(), Is.EqualTo("A/1.0.1"));
        }

        [Test]
        public void Tag_Prerelease_ReturnsCorrectTag()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/A.history");

            var tag = new TagVerb
            {
                VersionControlServices = git.Object,
                Prerelease = "test",
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(tag.GetConsoleOutput(), Is.EqualTo("A/1.0.1-test"));
        }

        [Test]
        public void Tag_CustomTemplate_ReturnsCorrectTag()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/A.history");

            var tag = new TagVerb
            {
                VersionControlServices = git.Object,
                TagTemplate = "release/%module%/v%version%",
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(tag.GetConsoleOutput(), Is.EqualTo("release/A/v1.0.1"));
        }

        [Test]
        public void Tag_CustomTemplate_NormalizesWeiredTags()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/A.history");

            var tag = new TagVerb
            {
                VersionControlServices = git.Object,
                TagTemplate = ".release//%module%.../ [%version%.",
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(tag.GetConsoleOutput(), Is.EqualTo("release/A./1.0.1"));
        }

        [Test]
        public void Tag_MultipleChanges_ReturnsCorrectTag()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/A.history", "Test/ComponentB/B.history");

            var tag = new TagVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(tag.GetConsoleOutput(), Is.EqualTo("A/1.0.1,B/1.0.0"));
        }
    }
}
