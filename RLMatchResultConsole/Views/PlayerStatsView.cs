using RLMatchResultConsole.Common;
using RLMatchResultConsole.Components;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class PlayerStatsView : AbstractView
    {
        private readonly IViewRegister _viewRegister;
        private readonly IDataCache _dataCache;
        private readonly DataFilter _filter;

        private RLTableComponent _playersRLTable = new RLTableComponent(0, 0, 0, 0);
        private int _sortColumn = 0;
        private bool _sortDescending = true;
        private List<Button> _sortButtons = new List<Button>();

        public PlayerStatsView(IViewRegister viewRegister, IDataCache dataCache, DataFilter filter)
        {
            _viewRegister = viewRegister;
            _dataCache = dataCache;
            _filter = filter;
        }

        public override void Execute()
        {
            Update();
        }

        public override string GetTitle()
        {
            return "Player Statistics";
        }

        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;
            _sortButtons.Clear();
            _sortColumn = 0;
            _sortDescending = true;

            _playersRLTable = new RLTableComponent(1, 2, Dim.Fill(), Dim.Fill());

            _playersRLTable.AddColumn(typeof(string), "Name", minWidth: 30, scheme: _playersRLTable.NormalNoFocus);
            _playersRLTable.AddColumn(typeof(int), "Matches", minWidth: 9, align: TextAlignment.Centered, scheme: _playersRLTable.NormalNoFocus);
            _playersRLTable.AddColumn(typeof(int), "Goals", minWidth: 9, align: TextAlignment.Centered, scheme: _playersRLTable.NormalNoFocus);
            _playersRLTable.AddColumn(typeof(int), "Assists", minWidth: 9, align: TextAlignment.Centered, scheme: _playersRLTable.NormalNoFocus);
            _playersRLTable.AddColumn(typeof(int), "Saves", minWidth: 9, align: TextAlignment.Centered, scheme: _playersRLTable.NormalNoFocus);
            _playersRLTable.AddColumn(typeof(int), "Shots", minWidth: 9, align: TextAlignment.Centered, scheme: _playersRLTable.NormalNoFocus);
            _playersRLTable.AddColumn(typeof(int), "MVPs", minWidth: 9, align: TextAlignment.Centered, scheme: _playersRLTable.NormalNoFocus);

            for (int i = 1; i <= 6; i++)
            {
                int x = 24 + 10 * i;
                var b = new Button(x, 1, "   ");
                _sortButtons.Add(b);
                var col = i;
                b.Clicked += () =>
                {
                    SetSort(col);
                };
                content.Add(b);
            }

            _playersRLTable.AddToView(content);

        }

        private class PlayerStats
        {
            public int Matches = 0;
            public int Goals = 0;
            public int Assists = 0;
            public int Saves = 0;
            public int Shots = 0;
            public int MVPs = 0;
            public int CalcScore = 0;

            public PlayerStats() { }
        }

        public override void Update()
        {
            var matches = _dataCache.MatchResults
                .Where(mr => _filter.GameModeFilter(mr.Match));
            Dictionary<string, PlayerStats> playerStats = new Dictionary<string, PlayerStats>();

            foreach (var match in matches)
            {
                if (match.Players.Count != 2)
                    continue;

                var playersCombined = match.Players[0].Concat(match.Players[1]);
                foreach (var player in playersCombined) {
                    if (player.Name is null)
                        continue;

                    PlayerStats ps;
                    if (playerStats.ContainsKey(player.Name))
                    {
                        ps = playerStats[player.Name];
                    }
                    else
                    {
                        ps = new PlayerStats();
                        playerStats[player.Name] = ps;
                    }

                    ps.Matches += 1;
                    ps.Goals += player.Goals;
                    ps.Assists += player.Assists;
                    ps.Saves += player.Saves;
                    ps.Shots += player.Shots;
                    ps.MVPs += player.IsMvp ? 1 : 0;
                }
            }

            foreach (var kvp in playerStats)
            {
                if (kvp.Value.Matches > 1)
                {
                    _playersRLTable.AddRow(kvp.Key, kvp.Value.Matches, kvp.Value.Goals, kvp.Value.Assists, kvp.Value.Saves, kvp.Value.Shots, kvp.Value.MVPs);
                }
            }

            SetSort(1);

        }

        internal void SetSort(int columnIndex) {

            if (_sortColumn > 0 && _sortColumn <= 6)
                _sortButtons[_sortColumn - 1].Text = "   ";

            if (_sortColumn == columnIndex)
            {
                _sortDescending = !_sortDescending;
            }
            else
            {
                _sortDescending = true;
            }

            _sortButtons[columnIndex - 1].Text = (_sortDescending ? " ▼ " : " ▲ ");
            _sortColumn = columnIndex;

            Application.MainLoop.Invoke(() => {
                _playersRLTable.SortBy(_sortColumn, _sortDescending);
                _playersRLTable.Update();
            });
        }

    }
}
