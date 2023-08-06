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
        private readonly DataFileWatcher _dataFileWatcher;

        private Label _progressText = new Label();
        private ProgressBar _progressBar = new ProgressBar();

        private Dictionary<DataLoader.ProgressType, int> _progress = new Dictionary<DataLoader.ProgressType, int>();

        public DataInitialisationView(
            IViewRegister viewRegister, 
            DataLoader dataLoader, 
            SessionListView sessionListView,
            DataFileWatcher dataFileWatcher)
        {
            _viewRegister = viewRegister;
            _dataLoader = dataLoader;
            _sessionListView = sessionListView;
            _dataFileWatcher = dataFileWatcher;
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
            _progress[DataLoader.ProgressType.MatchesFound] = 0;
            _progress[DataLoader.ProgressType.MatchesLoaded] = 0;
            _progress[DataLoader.ProgressType.MatchesAnalysed] = 0;
            _progress[DataLoader.ProgressType.SessionsGenerated] = 0;

            var task = new Task(() => _dataLoader.LoadFullData(ProgressUpdate));
            task.Start();
        }

        public void ProgressUpdate(DataLoader.ProgressType progressType, int count)
        {
            if (progressType == DataLoader.ProgressType.FinishedLoading)
            {
                _dataFileWatcher.StartWatching();
                _viewRegister.SwitchCurrentView(_sessionListView);
                return;
            }

            _progress[progressType] += count;

            float found = (float)_progress[DataLoader.ProgressType.MatchesFound];
            float loaded = (float)_progress[DataLoader.ProgressType.MatchesLoaded];
            float analysed = (float)_progress[DataLoader.ProgressType.MatchesAnalysed];
            float sessions = (float)_progress[DataLoader.ProgressType.SessionsGenerated];

            float fraction = found > 0 ? loaded / found * 0.8F + analysed / found * 0.2F : 0;

            Application.MainLoop.Invoke(() =>
            {
                _progressBar.Fraction = fraction;
                _progressText.Text = $"Completed {loaded} of {found}.";
                if (sessions > 0)
                {
                    _progressText.Text += $" {sessions} play sessions found.";
                }
            });

        }

        public override void Update()
        {
            // nothing to do in this view
        }
    }
}
