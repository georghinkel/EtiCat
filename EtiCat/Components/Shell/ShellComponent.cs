using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.Shell
{
    internal class ShellComponent : Component
    {
        private readonly IComponentProvider _provider;

        public ShellComponent(string path, IComponentProvider componentProvider) : base(path)
        {
            _provider = componentProvider;
        }

        public override string Type => "sh";

        public override IComponentProvider Provider => _provider;

        public override void Apply(ExtendedVersionInfo versionInfo, bool dry)
        {
        }

        private string? Directory => System.IO.Path.GetDirectoryName(Path);

        public override void Compile(IProcessExecutor processExecutor)
        {
            processExecutor.ExecuteAndCheck(Path, Module!.GetSettingOrDefault("shell_build", "build"), Directory);
        }

        public override void Test(IProcessExecutor processExecutor)
        {
            processExecutor.ExecuteAndCheck(Path, Module!.GetSettingOrDefault("shell_test", "test"), Directory);
        }

        public override void Pack(IProcessExecutor processExecutor)
        {
            processExecutor.ExecuteAndCheck(Path, Module!.GetSettingOrDefault("shell_pack", "pack"), Directory);
        }
    }
}
