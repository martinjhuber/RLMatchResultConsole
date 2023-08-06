using Microsoft.Extensions.FileSystemGlobbing;
using RLMatchResultConsole.Common;
using RLMatchResultConsole.Models;
using RLMatchResultConsole.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;
using static RLMatchResultConsole.Data.DataLoader;

namespace RLMatchResultConsole.Data
{
    internal class DataFileWatcher
    {

        private readonly ISettings _settings;
        private readonly DataLoader _dataLoader;
        private readonly IViewRegister _viewRegister;
        private readonly SessionListView _sessionListView;

        private FileSystemWatcher _watcher;

        public DataFileWatcher(ISettings settings, DataLoader dataLoader, IViewRegister viewRegister, SessionListView sessionListView)
        {
            _settings = settings;
            _dataLoader = dataLoader;
            _viewRegister = viewRegister;
            _sessionListView = sessionListView;

            string path = _settings.MatchResultDirectory;

            _watcher = new FileSystemWatcher(path);

            _watcher.NotifyFilter = NotifyFilters.FileName;
            _watcher.Filter = "*.json";

            _watcher.Created += UpdateDatabase;
            _watcher.Changed += UpdateDatabase;
            _watcher.Deleted += UpdateDatabase;

        }

        public void StartWatching()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void PauseWatching()
        {
            _watcher.EnableRaisingEvents = false;
        }

        private void UpdateDatabase(object source, FileSystemEventArgs e)
        {

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                Thread.Sleep(1000);     // FIXME: workaround for "File used by another process" issue

                var fileInfo = new FileInfo(e.FullPath);
                var matches = _dataLoader.LoadFile(fileInfo, ProgressUpdate);
                foreach (MatchResult matchResult in matches.OrderByDescending(mr => mr.Date))
                {
                    _dataLoader.GenerateOrAddToSession(matchResult, ProgressUpdate);
                }

                Application.MainLoop.Invoke(() => {
                    _viewRegister.UpdateCurrentView();
                    _viewRegister.ShowStatus($"Found new data file {e.Name}.");
                });
            }
            else if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Deleted)
            {

                _dataLoader.LoadFullData(ProgressUpdate);

                Application.MainLoop.Invoke(() => {
                    _viewRegister.ShowStatus("Data files changed. Reloading data.");
                    _viewRegister.SwitchCurrentView(_sessionListView);
                    _viewRegister.ShowStatus("Data files were changed. Switched to session list.");
                });
            }
        }

        public void ProgressUpdate(DataLoader.ProgressType progressType, int count)
        {

        }

    }
}
