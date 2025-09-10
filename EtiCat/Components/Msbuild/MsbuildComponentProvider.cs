using EtiCat.Contracts;
using EtiCat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiCat.Components.Csproj
{
    internal class MsbuildComponentProvider : IComponentProvider
    {
        private readonly StringBuilder _affectedBuilder = new StringBuilder();
        private string? _task;

        public string Name => "MSBuild";

        public IEnumerable<ModuleParameter> SupportedParameters
        {
            get
            {
                yield return new ModuleParameter("msbuild_build", "Parameters passed to MSBuild when building a component", "build --configuration Release");
                yield return new ModuleParameter("msbuild_test", "Parameters passed to MSBuild when testing a component", "test --configuration Release");
                yield return new ModuleParameter("msbuild_pack", "Parameters passed to MSBuild when packing a component", "pack --no-build");
            }
        }

        public MsbuildComponentProvider()
        {
            WriteHeader();
        }

        private void WriteHeader()
        {
            _affectedBuilder.AppendLine("<Project Sdk=\"Microsoft.Build.Traversal/4.1.82\">");
            _affectedBuilder.AppendLine("<ItemGroup>");
        }

        private void WriteFooter()
        {
            _affectedBuilder.AppendLine("</ItemGroup>");
            _affectedBuilder.AppendLine("</Project>");
        }

        public bool CanProvideComponent(string path, string extension)
        {
            return extension == ".csproj" || extension == ".vbproj" || extension == ".fsproj";
        }

        public void Flush(IProcessExecutor processExecutor)
        {
            if (_task == null) { return; }

            var path = Path.Combine(Path.GetTempPath(), "affected.proj");
            WriteFooter();

            File.WriteAllText(path, _affectedBuilder.ToString());
            try
            {
                processExecutor.ExecuteAndCheck("dotnet", $"{_task} \"{path}\"");
            }
            finally
            {
                File.Delete(path);
            }
            _affectedBuilder.Clear();
            WriteHeader();
            _task = null;
        }

        public Component ProvideComponent(string path)
        {
            return new MsbuildComponent(this, path);
        }

        public void Schedule(string project, string task, IProcessExecutor processExecutor)
        {
            if (_task != task)
            {
                Flush(processExecutor);
                _task = task;
            }

            _affectedBuilder.AppendLine($"<ProjectReference Include=\"{project}\" />");
        }
    }
}
