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

            processExecutor.ExecuteAndCheck("dotnet", $"{_task} \"{path}\"");

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
