using EtiCat.Contracts;
using EtiCat.Model;
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
    public class CheckTests
    {
        [Test]
        public void Check_AllChangesExplained_Pass()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/src/foo.bar", "Test/ComponentA/A.history");

            var check = new CheckVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(check.Execute(), Is.EqualTo(0));
        }

        [Test]
        public void Check_MissingExplanation_Fails()
        {
            var git = new Mock<IVersionControlServices>();

            git.SetupChangedFiles("Test/ComponentA/src/foo.bar");

            var check = new CheckVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [new TestComponentProvider()]
            };

            Assert.That(check.Execute(), Is.Not.EqualTo(0));
        }

        [TestCase(DependencyBehavior.ExactMajor)]
        [TestCase(DependencyBehavior.ExactMinor)]
        [TestCase(DependencyBehavior.ExactVersion)]
        public void Check_BrokenDependency_Fails(DependencyBehavior dependencyBehavior)
        {
            var git = new Mock<IVersionControlServices>();
            var tests = new TestComponentProvider();

            tests.ComponentCreated += component =>
            {
                if (component.Name == "A")
                {
                    component.Dependencies.Add(new Dependency("B")
                    {
                        Behavior = dependencyBehavior
                    });
                }
            };

            git.SetupChangedFiles("Test/ComponentB/B.history");

            var check = new CheckVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [tests]
            };

            Assert.That(check.Execute(), Is.Not.EqualTo(0));
        }

        [TestCase(DependencyBehavior.ExactMajor)]
        [TestCase(DependencyBehavior.ExactMinor)]
        [TestCase(DependencyBehavior.LowerBound)]
        public void Check_LivingDependency_Succeeds(DependencyBehavior dependencyBehavior)
        {
            var git = new Mock<IVersionControlServices>();
            var tests = new TestComponentProvider();

            tests.ComponentCreated += component =>
            {
                if (component.Name == "B")
                {
                    component.Dependencies.Add(new Dependency("A")
                    {
                        Behavior = dependencyBehavior
                    });
                }
            };

            git.SetupChangedFiles("Test/ComponentA/A.history");

            var check = new CheckVerb
            {
                VersionControlServices = git.Object,
                ComponentProviders = [tests]
            };

            Assert.That(check.Execute(), Is.EqualTo(0));
        }
    }
}
