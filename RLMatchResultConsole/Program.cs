using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using Terminal.Gui;

namespace RLMatchResultConsole {

    public class Program {

        private static IViewRegister? _vr = null;
        private static ISettings _settings = new Settings();

        public static readonly string SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string SettingsFileName = "RLMatchResultConsole.config.json";

        public static int Main ()
        {
            Application.Init();

            LoadSettings();

            var serviceProvider = new ServiceCollection()
                // Settings
                .AddSingleton<ISettings>(_settings)
                // Data
                .AddSingleton<DataFilter, DataFilter>()
                .AddSingleton<IDataCache, DataCache>()
                .AddTransient<DataLoader, DataLoader>()
                .AddSingleton<DataFileWatcher, DataFileWatcher>()
                // Views
                .AddTransient<DataInitialisationView, DataInitialisationView>()
                .AddTransient<FiltersView, FiltersView>()
                .AddTransient<SessionListView, SessionListView>()
                .AddTransient<SessionView, SessionView>()
                .AddTransient<MatchView, MatchView>()
                .AddSingleton<IViewRegister, ViewRegister>()
                .AddTransient<MatchStatsView, MatchStatsView>()
                .AddTransient<PlayerStatsView, PlayerStatsView>()
                // Main
                .AddSingleton<RLMatchResult, RLMatchResult>()
                .BuildServiceProvider();

            var mr = serviceProvider.GetRequiredService<RLMatchResult>();
            _vr = serviceProvider.GetRequiredService<IViewRegister>();

            var toplevel = mr.Start();

            Application.Run(toplevel, ExceptionHandler);
            Application.Shutdown();

            return 0;
        }

        internal static bool ExceptionHandler (Exception e) {

            if (_vr is not null)
            {
                _vr.ShowError(e.ToString());
                return true;
            }

            Console.Error.WriteLine(e.ToString());
            //Console.Error.WriteLine(e.StackTrace);
            return false;
        }

        private static void LoadSettings()
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Program.SettingsFolder)
                .AddJsonFile(SettingsFileName, optional: true, reloadOnChange: false)
                .Build();

            ISettings? settings = configuration.Get<Settings>();

            if (settings is null)
            {
                _settings = new Settings();
                SaveSettings();
            }
            else
            {
                _settings = settings;
            }

        }

        public static void SaveSettings()
        {

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(SettingsFolder + Path.DirectorySeparatorChar + SettingsFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, _settings);
            }

        }

    }

}

