using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Views;
using System;
using System.IO;
using Terminal.Gui;

namespace RLMatchResultConsole {

    public class Program {

        public static int Main ()
        {
            Application.Init();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            ISettings? settings = configuration.GetRequiredSection("AppSettings").Get<Settings>();

            if (settings == null) {
                Console.Error.WriteLine("No appsettings.json file found. Please add one to the directory.");
                Application.Shutdown();
                return 1;
            }

            var serviceProvider = new ServiceCollection()
                // Settings
                .AddSingleton<ISettings>(settings)
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
                // Main
                .AddSingleton<RLMatchResult, RLMatchResult>()
                .BuildServiceProvider();

            var mr = serviceProvider.GetRequiredService<RLMatchResult>();
            var toplevel = mr.Start();

            Application.Run(toplevel, ExceptionHandler);
            Application.Shutdown();

            return 0;
        }

        internal static bool ExceptionHandler (Exception e) {

            Console.Error.WriteLine(e.ToString());
            Console.Error.WriteLine(e.StackTrace);
            return false;
        }

    }

}

