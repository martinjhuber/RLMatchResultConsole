using RLMatchResultConsole.Common;
using RLMatchResultConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Data
{
    internal class DataFilter
    {

        private readonly ISettings _settings;

        public bool RankedOnly { get; private set; } = true;
        public List<GameMode> GameModeFilters { get; private set; } = new();

        public Func<Match, bool> GameModeFilter { get; private set; } = (Match m) => false;

        public DataFilter(ISettings settings)
        {
            _settings = settings;
            
            SetGameModeFilters(_settings.DefaultFilters.GameModes);
            SetRankedOnlyFilter(_settings.DefaultFilters.RankedOnly);

        }

        public void SetGameModeFilters(string[] gameModeFilterArray)
        {
            foreach (string gameModeFilter in gameModeFilterArray)
            {
                GameMode gm = (GameMode)Enum.Parse(typeof(GameMode), gameModeFilter);
                GameModeFilters.Add(gm);
            }

            GameModeFilter = (Match m) => { return IsAllowed(m); };
        }

        public void SetRankedOnlyFilter(bool rankedOnly)
        {
            RankedOnly = rankedOnly;
        }

        public bool IsAllowed(Match m)
        {
            if (RankedOnly && m.IsRanked == false)
            {
                return false;
            }

            return GameModeFilters.Contains(m.GameMode);
        }

    }
}
