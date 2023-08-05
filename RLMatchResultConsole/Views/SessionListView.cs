using RLMatchResultConsole.Common;
using RLMatchResultConsole.Components;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class SessionListView : AbstractView
    {
        private readonly IViewRegister _viewRegister;
        private readonly IDataCache _dataCache;
        private readonly SessionView _sessionView;
        private readonly DataFilter _filter;

        private RLTableComponent _sessionsRLTable = new RLTableComponent(0, 0, 0, 0);

        private IList<Session> _shownSessions = new List<Session>();

        public SessionListView(IViewRegister viewRegister, IDataCache dataCache, SessionView sessionView, DataFilter filter)
        {
            _viewRegister = viewRegister;
            _dataCache = dataCache;
            _sessionView = sessionView;
            _filter = filter;
        }

        public override string GetTitle()
        {
            return "Session List";
        }

        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

            /*
            IList sessions = _dataCache.GetFilteredSessions().OrderByDescending(s => s.FirstMatch).ToList();

            Label l = new Label(1, 0, "Select a session:");

            FrameView f = new FrameView()
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 1,
                Height = Dim.Fill()
            };

            ListView lv = new ListView(sessions)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            lv.OpenSelectedItem += (arg) => { 
                var session = arg.Value as Session;
                if (session != null)
                {
                    _sessionView.SetSession(session);
                    _viewRegister.SwitchCurrentView(_sessionView, true);
                }
                else
                {
                    _viewRegister.ShowError("This session could not be loaded correctly.");
                }
            };

            f.Add(lv);
            content.Add(l, f);
            */

            Label l = new Label(1, 1, "Select a session.");

            _sessionsRLTable = new RLTableComponent(1, 3, Dim.Fill() - 1, Dim.Fill(), "Session List");

            _sessionsRLTable.AddColumn(typeof(string), "Date", minWidth: 21);
            _sessionsRLTable.AddColumn(typeof(int), "Wins", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionsRLTable.MatchWin);
            _sessionsRLTable.AddColumn(typeof(int), "Loss", minWidth: 8, align: TextAlignment.Centered, scheme: _sessionsRLTable.MatchLoss);
            _sessionsRLTable.AddColumn(typeof(int), "GF", minWidth: 8, align: TextAlignment.Centered);
            _sessionsRLTable.AddColumn(typeof(int), "GA", minWidth: 8, align: TextAlignment.Centered);
            _sessionsRLTable.AddColumn(typeof(string), "Notes", align: TextAlignment.Left);
            _sessionsRLTable.FullRowSelect = true;

            _sessionsRLTable.AddCellActionCallback((int col, int row) =>
            {
                if (row < _shownSessions.Count)
                {
                    _sessionView.SetSession(_shownSessions.ElementAt(row));
                    _viewRegister.SwitchCurrentView(_sessionView, true);
                }
            });

            _sessionsRLTable.AddToView(content);
            content.Add(l);
        }

        public override void Execute()
        {
            Update();
        }


        public override void Update()
        {
            _sessionsRLTable.ClearRows();

            _shownSessions = _dataCache.GetFilteredSessions().OrderByDescending(s => s.FirstMatch).ToList();

            foreach (var session in _shownSessions)
            {
                var shownMatches = session.MatchResults.Where(mr => _filter.GameModeFilter(mr.Match)).ToList();

                var date = Formatting.FormatDateTimeFull(session.FirstMatch);
                var wins = shownMatches.Count(mr => mr.Match.Result == Result.Win);
                var losses = shownMatches.Count(mr => mr.Match.Result == Result.Loss);
                var gfs = shownMatches.Sum(mr => mr.Teams[0].TeamScore);
                var gas = shownMatches.Sum(mr => mr.Teams[1].TeamScore);
                var games = (session.MatchResults.Count != shownMatches.Count) ? $"Filtered: {(session.MatchResults.Count - shownMatches.Count)}" : "";

                _sessionsRLTable.AddRow(date, wins, losses, gfs, gas, games);

            }

            _sessionsRLTable.Update();

        }
    }
}
