using RLMatchResultConsole.Common;
using RLMatchResultConsole.Models;
using System;
using System.Collections.Generic;

namespace RLMatchResultConsole.Data
{
    internal class DataFilter
    {

        private readonly ISettings _settings;

        public bool RankedOnly {
            get
            {
                return _settings.Filters.RankedOnly;
            }
            set
            {
                _settings.Filters.RankedOnly = value;
                Program.SaveSettings();
            }
        }

        public bool DisableFilters
        {
            get
            {
                return _settings.Filters.DisableFilters;
            }
            set
            {
                _settings.Filters.DisableFilters = value;
                Program.SaveSettings();
            }
        }

        public Func<Match, bool> GameModeFilter { get; private set; } = (Match m) => false;

        public DataFilter(ISettings settings)
        {
            _settings = settings;
            GameModeFilter = (Match m) => { return IsAllowed(m); };
        }

        public void SetGameModeFilter(GameMode gameMode, bool isShown)
        {
            string gms = gameMode.ToString();

            if (!isShown && _settings.Filters.GameModes.Contains(gms))
            {
                _settings.Filters.GameModes.Remove(gms);
                Program.SaveSettings();
            }
            if (isShown && !_settings.Filters.GameModes.Contains(gms))
            {
                _settings.Filters.GameModes.Add(gms);
                Program.SaveSettings();
            }
        }

        public bool IsGameModeShown(GameMode gameMode)
        {
            return _settings.Filters.GameModes.Contains(gameMode.ToString());
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

            return _settings.Filters.GameModes.Contains(m.GameMode.ToString());
        }

    }
}
