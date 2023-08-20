using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Common
{
    public sealed class Settings : ISettings
    {
        const string DEFAULT_MATCHRESULTS_DIR = "%APPDATA%\\bakkesmod\\bakkesmod\\data\\MatchResults";

        public string MatchResultDirectory { get; set; } = DEFAULT_MATCHRESULTS_DIR;

        public Filters Filters { get; set; } = new Filters();

        public string GetParsedMatchResultDirectory()
        {
            if (MatchResultDirectory.Contains("%APPDATA%"))
            {
                return MatchResultDirectory.Replace("%APPDATA%",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
            return MatchResultDirectory;
        }
    }

    public sealed class Filters
    {
        public List<string> GameModes { get; set; } = new();

        public bool RankedOnly { get; set; } = true;
        public bool DisableFilters { get; set; } = true;

    }

}
