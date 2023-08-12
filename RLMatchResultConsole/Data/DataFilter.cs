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
        public bool DisableFilters { get; private set; } = false;
        public List<GameMode> GameModeFilters { get; private set; } = new();

        public Func<Match, bool> GameModeFilter { get; private set; } = (Match m) => false;

        public DataFilter(ISettings settings)
        {
            _settings = settings;
            
            SetGameModeFilters(_settings.DefaultFilters.GameModes);
            SetRankedOnlyFilter(_settings.DefaultFilters.RankedOnly);
            SetDisableFilters(_settings.DefaultFilters.DisableFilters);

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
        public void SetDisableFilters(bool disableFilters)
        {
            DisableFilters = disableFilters;
        }

        public bool IsAllowed(Match m)
        {
            if (DisableFilters)
            {
                return true;
            }

            if (RankedOnly && m.IsRanked == false)
            {
                return false;
            }

            return GameModeFilters.Contains(m.GameMode);
        }

    }
}
