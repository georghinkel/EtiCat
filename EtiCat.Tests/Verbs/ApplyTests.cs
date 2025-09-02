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
    public class ApplyTests
    {
        [Test]
        public void Apply_AppliesVersions()
        {
            var git = new Mock<IVersionControlServices>();
            var testProvider = new TestComponentProvider();

            var toApply = new List<string> { "A", "B" };
            git.SetupChangedFiles("Test/ComponentA/A.history");

            testProvider.Applied += (cmp, version) =>
            {
                if (!toApply.Remove(cmp.Name))
                {
                    Assert.Fail("Unknown component");
                }
                Assert.That(version.Commit, Is.EqualTo("commit_123"));
                Assert.That(version.Prerelease, Is.EqualTo("test"));
                if (cmp.Name == "A")
                {
                    Assert.That(version.Version.Major, Is.EqualTo(1));
                    Assert.That(version.Version.Minor, Is.EqualTo(0));
                    Assert.That(version.Version.Patch, Is.EqualTo(1));
                }
                else if (cmp.Name == "B")
                {
                    Assert.That(version.Version.Major, Is.EqualTo(1));
                    Assert.That(version.Version.Minor, Is.EqualTo(0));
                    Assert.That(version.Version.Patch, Is.EqualTo(0));
                }
            };

            var apply = new ApplyVerb
            {
                CommitId = "commit_123",
                Prerelease = "test",
                VersionControlServices = git.Object,
                ComponentProviders = [testProvider]
            };

            Assert.That(apply.Execute(), Is.EqualTo(0));

            Assert.That(toApply, Is.Empty);
        }

        [Test]
        public void Apply_UsesCommitIdentifier()
        {
            var git = new Mock<IVersionControlServices>();
            var testProvider = new TestComponentProvider();

            var toApply = new List<string> { "A", "B" };
            git.SetupChangedFiles("Test/ComponentA/A.history");
            git.Setup(m => m.LastCommitIdentifier()).Returns("commit_123");

            testProvider.Applied += (cmp, version) =>
            {
                if (!toApply.Remove(cmp.Name))
                {
                    Assert.Fail("Unknown component");
                }
                Assert.That(version.Commit, Is.EqualTo("commit_123"));
                Assert.That(version.Prerelease, Is.EqualTo("test"));
                if (cmp.Name == "A")
                {
                    Assert.That(version.Version.Major, Is.EqualTo(1));
                    Assert.That(version.Version.Minor, Is.EqualTo(0));
                    Assert.That(version.Version.Patch, Is.EqualTo(1));
                }
                else if (cmp.Name == "B")
                {
                    Assert.That(version.Version.Major, Is.EqualTo(1));
                    Assert.That(version.Version.Minor, Is.EqualTo(0));
                    Assert.That(version.Version.Patch, Is.EqualTo(0));
                }
            };

            var apply = new ApplyVerb
            {
                VersionControlServices = git.Object,
                Prerelease = "test",
                ComponentProviders = [testProvider]
            };

            Assert.That(apply.Execute(), Is.EqualTo(0));

            Assert.That(toApply, Is.Empty);
        }
    }
}
