using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.Docker
{
    internal class DockerComponentProvider : IComponentProvider
    {
        public string Name => "Docker";

        public IEnumerable<ModuleParameter> SupportedParameters
        {
            get
            {
                yield return new ModuleParameter("docker_tag", "Template from which tag names for docker containers are built.", "%module%");
            }
        }

        public bool CanProvideComponent(string path, string extension)
        {
            return Path.GetFileName(path) == "Dockerfile";
        }

        public void Flush(IProcessExecutor processExecutor)
        {
        }

        public Component ProvideComponent(string path)
        {
            return new DockerfileComponent(path, this);
        }
    }
}
