using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class DataInitialisationView : AbstractView
    {

        private readonly IViewRegister _viewRegister;
        private readonly DataLoader _dataLoader;
        private readonly SessionListView _sessionListView;

        private Label _progressText = new Label();
        private ProgressBar _progressBar = new ProgressBar();

        public DataInitialisationView(IViewRegister viewRegister, DataLoader dataLoader, SessionListView sessionListView)
        {
            _viewRegister = viewRegister;
            _dataLoader = dataLoader;
            _sessionListView = sessionListView;
        }

        public override string GetTitle()
        {
            return "Rocket League Match Result Console";
        }

        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

            content.Add(new Label("Loading data...")
                {
                    X = 3,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = 1
                });


            _progressBar = new ProgressBar()
            {
                X = 3,
                Y = 3,
                Width = Dim.Fill() - 3,
                Height = 1,
                ColorScheme = Colors.Error,
                Fraction = 0F,
                ProgressBarStyle = ProgressBarStyle.Continuous
            };

            _progressText = new Label("Completed 0 of 0.")
            {
                X = 3,
                Y = 5,
                Width = Dim.Fill(),
                Height = 1
            };

            content.Add(_progressText, _progressBar);

        }

        public override void Execute()
        {
            var task = new Task(() => _dataLoader.LoadData(ProgressUpdate, DataLoadComplete));
            task.Start();
        }

        public bool ProgressUpdate(float fraction, int loaded, int total, int sessions)
        {
            Application.MainLoop.Invoke(() =>
            {
                _progressBar.Fraction = fraction;
                _progressText.Text = $"Completed {loaded} of {total}.";
                if (sessions > 0)
                {
                    _progressText.Text += $" {sessions} play sessions found.";
                }
            });

            return true;
        }

        public void DataLoadComplete()
        {
            _viewRegister.SwitchCurrentView(_sessionListView);
        }

        public override void Update()
        {
            // nothing to do in this view
        }
    }
}
