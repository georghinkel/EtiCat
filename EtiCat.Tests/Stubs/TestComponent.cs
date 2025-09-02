using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Tests.Stubs
{
    internal class TestComponent : Component
    {
        private readonly TestComponentProvider _provider;

        public TestComponent(string path, TestComponentProvider provider) : base(path)
        {
            _provider = provider;
        }

        public override string Type => "test";

        public override void Apply(ExtendedVersionInfo versionInfo)
        {
            _provider.RaiseApplied(this, versionInfo);
        }
    }
}
