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
    internal class MatchStatsView : AbstractView
    {
        private readonly IViewRegister _viewRegister;
        private readonly IDataCache _dataCache;
        private readonly DataFilter _filter;

        private RLTableComponent _matchRLTable = new RLTableComponent(0, 0, 0, 0);

        public MatchStatsView(IViewRegister viewRegister, IDataCache dataCache, DataFilter filter)
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
            return "Match Statistics";
        }

        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

            _matchRLTable = new RLTableComponent(1, 1, Dim.Fill(), Dim.Fill());

            _matchRLTable.AddColumn(typeof(string), "Mode", minWidth: 20, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "Games", minWidth: 17, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "Goals", minWidth: 17, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "OT", minWidth: 17, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "FF", minWidth: 17, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);

            _matchRLTable.AddToView(content);

        }

        public override void Update()
        {
            var matches = _dataCache.MatchResults
                .Where(mr => _filter.GameModeFilter(mr.Match))
                .OrderByDescending(mr => mr.Date).ToList();

            AddStatsRow("Total", matches);

            foreach (var gameMode in Enum.GetValues<GameMode>())
            {
                var gmMatches = matches.Where(m => m.Match.GameMode == gameMode && m.Match.IsRanked).ToList();
                AddStatsRow(gameMode.ToString()+" Ranked", gmMatches);
                gmMatches = matches.Where(m => m.Match.GameMode == gameMode && !m.Match.IsRanked).ToList();
                AddStatsRow(gameMode.ToString(), gmMatches);
            }

        }

        public void AddStatsRow(string mode, List<MatchResult> matches)
        {
            if (matches.Count == 0)
                return;

            var games = matches.Count;
            var wins = matches.Count(m => m.Match.Result == Result.Win);
            var losses = games - wins;
            var winPercent = "0.00%";
            if (games > 0 && wins > 0)
            {
                winPercent = ((float)wins / games * 100).ToString("0.00") + "%";
            }

            var gfs = matches.Sum(m => m.Teams[0].TeamScore);
            var gas = matches.Sum(m => m.Teams[1].TeamScore);
            var goalsPercent = "0.00%";
            if (gfs > 0 && gas > 0)
            {
                goalsPercent = ((float)gfs / (gfs + gas) * 100).ToString("0.00") + " %";
            }
            

            var otWins = matches.Count(m => m.Match.Result == Result.Win && m.Match.IsOvertime == true);
            var otLosses = matches.Count(m => m.Match.Result == Result.Loss && m.Match.IsOvertime == true);
            var totalOt = otWins + otLosses;
            var otPercent = "0.00%";
            var totalOtPercent = "0.0%";
            if (games > 0 && totalOt > 0)
            {
                otPercent = ((float)otWins / totalOt * 100).ToString("0.00") + "%";
                totalOtPercent = ((float)totalOt / games * 100).ToString("0.0") + "%";
            }

            var ffWins = matches.Count(m => m.Match.Result == Result.Win && m.Match.IsForfeit == true);
            var ffLosses = matches.Count(m => m.Match.Result == Result.Loss && m.Match.IsForfeit == true);
            var totalFf = ffWins + ffLosses;
            var ffPercent = "0.00%";
            var totalFfPercent = "0.0%";
            if (games > 0 && totalFf > 0)
            {
                ffPercent = ((float)ffWins / totalFf * 100).ToString("0.00") + "%";
                totalFfPercent = ((float)totalFf / games * 100).ToString("0.0") + "%";
            }

            _matchRLTable.AddRow(mode, games, $"{gfs+gas}", $"{totalOt} ({totalOtPercent})", $"{totalFf} ({totalFfPercent})");
            _matchRLTable.AddRow("", $"{wins}-{losses}", $"{gfs}:{gas}", $"{otWins}-{otLosses}", $"{ffWins}-{ffLosses}");
            _matchRLTable.AddRow("", "("+winPercent+")", "(" + goalsPercent + ")", "(" + otPercent + ")", "(" + ffPercent + ")");
            _matchRLTable.AddRow("", "", "", "", "");

        }


    }
}
