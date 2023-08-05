using RLMatchResultConsole.Common;
using RLMatchResultConsole.Components;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class SessionView : AbstractView
    {

        private readonly IViewRegister _viewRegister;
        private readonly IDataCache _dataCache;
        private readonly DataFilter _filter;
        private readonly MatchView _matchView;

        private Session _session = new Session();

        private RLTableComponent _sessionRLTable = new RLTableComponent(0, 0, 0, 0);
        private RLTableComponent _matchesRLTable = new RLTableComponent(0, 0, 0, 0);

        private Button _backButton = new Button();

        private IList<MatchResult> _shownMatches = new List<MatchResult>();

        private readonly ColorScheme NormalNoFocusColorScheme = new ColorScheme();
        private readonly ColorScheme WinColorScheme = new ColorScheme();
        private readonly ColorScheme LossColorScheme = new ColorScheme();


        public SessionView(IViewRegister viewRegister, IDataCache dataCache, DataFilter filter, MatchView matchView)
        {
            _viewRegister = viewRegister;
            _dataCache = dataCache;
            _filter = filter;
            _matchView = matchView;
        }

        public void SetSession(Session session)
        {
            _session = session;
        }

        public override void Execute()
        {
            Update();
        }

        public override string GetTitle()
        {
            return $"Session {Formatting.FormatDateTimeFull(_session.FirstMatch)}";
        }

        // Session: Num games, Wins, Losses, GF, GA
        // Per match: GameMode, Date, Win/Loss, GF, GA, Special, MVP
        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

            _backButton = new Button(1, 1, "< Session List");
            _backButton.Clicked += () =>
            {
                _viewRegister.SwitchToPreviousView();
            };

            // SESSION SUMMARY TABLE

            _sessionRLTable = new RLTableComponent(1, 3, 46, 3, "Session summary");
            _sessionRLTable.AddColumn(typeof(int), "Matches", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionRLTable.NormalNoFocus);
            _sessionRLTable.AddColumn(typeof(int), "Wins", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionRLTable.MatchWin);
            _sessionRLTable.AddColumn(typeof(int), "Losses", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionRLTable.MatchLoss);
            _sessionRLTable.AddColumn(typeof(int), "GF", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionRLTable.NormalNoFocus);
            _sessionRLTable.AddColumn(typeof(int), "GA", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionRLTable.NormalNoFocus);

            // MATCHES TABLE

            Label matchesLabel = new Label(1, 7, "Matches");

            _matchesRLTable = new RLTableComponent(1, 9, Dim.Fill() - 1, Dim.Fill(), "Matches");
            _matchesRLTable.AddColumn(typeof(string), "Mode", minWidth: 17);
            _matchesRLTable.AddColumn(typeof(string), "Time", minWidth: 8, align: TextAlignment.Centered);
            var resultCol = _matchesRLTable.AddColumn(typeof(string), "Result", minWidth: 8, align: TextAlignment.Centered);
            _matchesRLTable.AddColumn(typeof(int), "GF", minWidth: 8, align: TextAlignment.Centered);
            _matchesRLTable.AddColumn(typeof(int), "GA", minWidth: 8, align: TextAlignment.Centered);
            _matchesRLTable.AddColumn(typeof(string), "Special", minWidth: 8, align: TextAlignment.Centered);
            _matchesRLTable.AddColumn(typeof(string), "MVP (★)");
            _matchesRLTable.FullRowSelect = true;

            _matchesRLTable.SetColumnColorsByValue(
                resultCol, 
                new Dictionary<string, ColorScheme>{ 
                    { "Win", _matchesRLTable.MatchWin },
                    { "Loss", _matchesRLTable.MatchLoss }
                });

            _matchesRLTable.AddCellActionCallback((int col, int row) =>
            {
                if (row < _shownMatches.Count)
                {
                    _matchView.SetMatchResult(_shownMatches.ElementAt(row));
                    _viewRegister.SwitchCurrentView(_matchView, true);
                }
            });

            _sessionRLTable.AddToView(content);
            _matchesRLTable.AddToView(content);
            content.Add(matchesLabel, _backButton);
        }

        public override void Update()
        {

            _shownMatches = _session.MatchResults.Where(mr => _filter.GameModeFilter(mr.Match)).ToList();

            // Session
            _sessionRLTable.ClearRows();

            var matches = _shownMatches.Count;
            var wins = String.Format("{0,8}", _shownMatches.Count(mr => mr.Match.Result == Result.Win));
            var losses = _shownMatches.Count(mr => mr.Match.Result == Result.Loss);
            var gfs = _shownMatches.Sum(mr => mr.Teams[0].TeamScore);
            var gas = _shownMatches.Sum(mr => mr.Teams[1].TeamScore);

            _sessionRLTable.AddRow(matches, wins, losses, gfs, gas);
            _sessionRLTable.Update();

            // Matches
            _matchesRLTable.ClearRows();

            foreach (var matchResult in _shownMatches)
            {
                var match = matchResult.Match;

                var mode = (match.IsRanked ? "Ranked " : "") + matchResult.Match.GameMode.ToString();
                var time = Formatting.FormatDateTimeHourMinute(matchResult.Date);
                var result = match.Result.ToString();
                var gf = matchResult.Teams[0].TeamScore;
                var ga = matchResult.Teams[1].TeamScore;
                var special = (match.IsForfeit ? "FF " : "") + (match.IsOvertime ? "OT" : "");

                string? mvp = "n/a";
                foreach (var playerList in matchResult.Players)
                {
                    foreach (var player in playerList)
                    {
                        if (player.IsMvp)
                        {
                            mvp = player.Name;
                            break;
                        }
                    }
                }

                _matchesRLTable.AddRow(mode, time, result, gf, ga, special, mvp);
            }

            _matchesRLTable.Update();
            _backButton.FocusFirst();

        }
    }
}
