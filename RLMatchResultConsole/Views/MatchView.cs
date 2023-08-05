using RLMatchResultConsole.Common;
using RLMatchResultConsole.Components;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Models;
using RLMatchResultConsole.Models.JsonV1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class MatchView : AbstractView
    {

        private readonly IViewRegister _viewRegister;
        private readonly IDataCache _dataCache;

        private MatchResult _matchResult = new MatchResult();

        private RLTableComponent _matchRLTable = new RLTableComponent(0, 0, 0, 0);
        private RLTableComponent _teamRLTable = new RLTableComponent(0, 0, 0, 0);
        private Label _fileNameLabel = new Label();

        private readonly ColorScheme NormalNoFocusColorScheme = new ColorScheme();
        private readonly ColorScheme WinColorScheme = new ColorScheme();
        private readonly ColorScheme LossColorScheme = new ColorScheme();


        public MatchView(IViewRegister viewRegister, IDataCache dataCache)
        {
            _viewRegister = viewRegister;
            _dataCache = dataCache;
        }

        public void SetMatchResult(MatchResult matchResult)
        {
            _matchResult = matchResult;
        }

        public override void Execute()
        {
            Update();
        }

        public override string GetTitle()
        {
            return $"Match {Formatting.FormatDateTimeFull(_matchResult.Date)}";
        }

        // Match: GameMode, Date, Win/Loss, GF, GA, Special
        // Team: Goals, TeamName
        // Per Player: Name, Score, Goals, Assists, Saves, Shots
        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

            Button backButton = new Button(1, 1, "< Back to session");
            backButton.Clicked += () =>
            {
                _viewRegister.SwitchToPreviousView();
            };

            content.Add(backButton);

            // MATCH SUMMARY TABLE

            _matchRLTable = new RLTableComponent(1, 3, 64, 3);

            _matchRLTable.AddColumn(typeof(string), "Mode", minWidth: 17, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "Time", minWidth: 8, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            var resultCol = _matchRLTable.AddColumn(typeof(string), "Result", minWidth: 8, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "GF", minWidth: 8, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "GA", minWidth: 8, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);
            _matchRLTable.AddColumn(typeof(string), "Special", minWidth: 8, align: TextAlignment.Centered, scheme: _matchRLTable.NormalNoFocus);

            _matchRLTable.SetColumnColorsByValue(
                resultCol,
                new Dictionary<string, ColorScheme>{
                    { "Win", _matchRLTable.MatchWin },
                    { "Loss", _matchRLTable.MatchLoss }
                });

            Label scoresLabel = new Label(1, 7, "Scores");

            // TEAM TABLE
            _teamRLTable = new RLTableComponent(1, 9, Dim.Fill()-1, Dim.Fill());
            _teamRLTable.AddColumn(typeof(string), " ", minWidth: 5, align: TextAlignment.Centered);
            _teamRLTable.AddColumn(typeof(string), "Name", minWidth: 30, maxWidth: 25);
            _teamRLTable.AddColumn(typeof(string), "Score", minWidth: 7, align: TextAlignment.Centered);
            _teamRLTable.AddColumn(typeof(string), "Goals", minWidth: 7, align: TextAlignment.Centered);
            _teamRLTable.AddColumn(typeof(string), "Assists", minWidth: 7, align: TextAlignment.Centered);
            _teamRLTable.AddColumn(typeof(string), "Saves", minWidth: 7, align: TextAlignment.Centered);
            _teamRLTable.AddColumn(typeof(string), "Shots", minWidth: 7, align: TextAlignment.Centered);

            _matchRLTable.AddToView(content);
            _teamRLTable.AddToView(content);

            _fileNameLabel = new Label("Test") {
                X = 1,
                Y = Pos.Bottom(content) - 4
            };

            content.Add(scoresLabel, _fileNameLabel);


        }

        public override void Update()
        {
            Models.Match match = _matchResult.Match;

            _matchRLTable.ClearRows();

            var mode = (match.IsRanked ? "Ranked " : "") + _matchResult.Match.GameMode.ToString();
            var time = Formatting.FormatDateTimeHourMinute(_matchResult.Date);
            var result = match.Result.ToString();
            var gf = _matchResult.Teams[0].TeamScore;
            var ga = _matchResult.Teams[1].TeamScore;
            var special = (match.IsForfeit ? "FF " : "") + (match.IsOvertime ? "OT" : "");

            _matchRLTable.AddRow(mode, time, result, gf, ga, special);
            _matchRLTable.Update();

            _teamRLTable.ClearRows();

            for (int t = 0; t <= 1; t++)
            {
                Team team = _matchResult.Teams[t];
                _teamRLTable.AddRow(team.TeamScore.ToString(), team.TeamName, null, null, null, null, null);

                List<Player> players = _matchResult.Players.Count > t ? _matchResult.Players[t] : new List<Player>();

                foreach (var player in players.OrderByDescending(p => p.Score))
                {
                    _teamRLTable.AddRow(
                        "",
                        player.Name + (player.IsMvp ? " (★)" : ""),
                        player.Score.ToString(),
                        player.Goals.ToString(),
                        player.Assists.ToString(),
                        player.Saves.ToString(),
                        player.Shots.ToString());
                }
            }

            int team0playerCount = _matchResult.Players.Count > 1 ? _matchResult.Players[0].Count : 0;

            _teamRLTable.SetRowColors(new Dictionary<int, ColorScheme>()
            {
                { 0, _teamRLTable.RowHighlight },
                { team0playerCount + 1, _teamRLTable.RowHighlight },
            });

            _matchRLTable.Update();

            _fileNameLabel.Text = $"({_matchResult.FileName})";
        }
    }

}
