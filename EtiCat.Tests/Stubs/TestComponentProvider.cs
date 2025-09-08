using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Tests.Stubs
{
    internal class TestComponentProvider : IComponentProvider
    {
        public event Action<TestComponent>? ComponentCreated;

        public event Action<TestComponent, ExtendedVersionInfo>? Applied;

        public List<string> RequestedComponents { get; } = new List<string>();

        public bool CanProvideComponent(string path, string extension)
        {
            RequestedComponents.Add(path);
            return true;
        }

        public Component ProvideComponent(string path)
        {
            var result = new TestComponent(path, this);
            ComponentCreated?.Invoke(result);
            return result;
        }

        internal void RaiseApplied(TestComponent testComponent, ExtendedVersionInfo versionInfo)
        {
            Applied?.Invoke(testComponent, versionInfo);
        }

        public void Flush(IProcessExecutor processExecutor)
        {
        }
    }
}
