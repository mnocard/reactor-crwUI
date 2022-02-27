using r_crwUI_A.Interfaces;

using System.Diagnostics;
using System.IO;
using System.Linq;

namespace r_crwUI_A.Services
{
    internal class LoadDefaultSettings : ILoadDefaultSettings
    {
        private const string _exe = ".exe";
        private const string _json = ".json";
        private const string _settings = "settings ";

        public string GetDefaultFilePath()
        {
            var path = Process.GetCurrentProcess().MainModule.FileName;
            var name = Path.GetFileName(path);
            return Directory.GetFiles(Path.GetDirectoryName(path))
                .Where(f => f.Contains(_exe) && !f.Contains(name))
                .OrderBy(f => f)
                .FirstOrDefault();
        }

        public string GetDefaultSettings() => Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                .Where(f => f.Contains(_json) && f.Contains(_settings))
                .OrderByDescending(f => f)
                .FirstOrDefault();
    }
}
