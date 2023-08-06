using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Common
{
    public sealed class Settings : ISettings
    {
        const string DEFAULT_MATCHRESULTS_DIR = ".\\MatchResults";

        private string _matchResultDirectory = DEFAULT_MATCHRESULTS_DIR;

        public string MatchResultDirectory
        {
            get { return _matchResultDirectory; }
            set { _matchResultDirectory = value; }
        }

        public DefaultFilters DefaultFilters { get; set; } = new DefaultFilters();

    }

    public sealed class DefaultFilters
    {
        public string[] GameModes { get; set; } = new string[0];

        public bool RankedOnly { get; set; } = true;

    }

}
