using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.Docker
{
    internal class DockerfileComponent : Component
    {
        private readonly DockerComponentProvider _provider;
        private string? _version;

        public DockerfileComponent(string path, DockerComponentProvider provider) : base(path)
        {
            _provider = provider;
        }

        public override string Type => "docker";

        public override IComponentProvider Provider => _provider;

        public override void Apply(ExtendedVersionInfo versionInfo, bool dry)
        {
            _version = versionInfo.Version.Semver(versionInfo.Prerelease, versionInfo.Commit);
        }

        public override void Pack(IProcessExecutor processExecutor)
        {
            var tag = Module!.GetSettingOrDefault("docker_tag", "%module%").Replace("%module%", Module.Name);
            processExecutor.ExecuteAndCheck("docker", $"buildx build -t {tag}:{_version} -t {tag}:latest .", System.IO.Path.GetDirectoryName(Path));
        }
    }
}
