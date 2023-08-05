using RLMatchResultConsole.Common;
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

        public SessionListView(IViewRegister viewRegister, IDataCache dataCache, SessionView sessionView)
        {
            _viewRegister = viewRegister;
            _dataCache = dataCache;
            _sessionView = sessionView;
        }

        public override string GetTitle()
        {
            return "Session List";
        }

        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

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

        }

        public override void Execute()
        {
            // TODO
        }


        public override void Update()
        {
            // TODO
        }
    }
}
