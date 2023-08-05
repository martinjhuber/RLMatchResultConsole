using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Common
{
    public sealed class Settings : ISettings
    {
        public string? MatchResultDirectory { get; set; }

        public DefaultFilters DefaultFilters { get; set; } = new DefaultFilters();

    }

    public sealed class DefaultFilters
    {
        public string[] GameModes { get; set; } = new string[0];

        public bool RankedOnly { get; set; } = true;

    }

}
