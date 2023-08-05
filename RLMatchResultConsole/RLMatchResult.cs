using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole
{
    internal class RLMatchResult
    {

        private readonly IViewRegister _viewRegister;
        private readonly DataInitialisationView _dataInitView;
        private readonly SessionListView _sessionListView;
        private readonly FiltersView _filtersView;

        public RLMatchResult(IViewRegister viewRegister,
            DataInitialisationView dataInitView,
            SessionListView sessionListView,
            FiltersView filtersView) {

            _viewRegister = viewRegister;
            _dataInitView = dataInitView;
            _sessionListView = sessionListView;
            _filtersView = filtersView;
        }

        public Toplevel Start()
        {
            // Init GUI
            var contentWindow = CreateContentWindow();
            _viewRegister.ContentWindow = contentWindow;
            var menuBar = CreateMenuBar();
            var toplevel = GenerateTopLevel();
            toplevel.Add(contentWindow, menuBar);

            // Load first view -> DataInitialisation
            _viewRegister.SwitchCurrentView(_dataInitView);

            return toplevel;
        } 

        private Toplevel GenerateTopLevel()
        {
            return new Toplevel()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

        }

        private Window CreateContentWindow()
        {
            return new Window()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

        }

        private MenuBar CreateMenuBar()
        {
            return new MenuBar(
                new MenuBarItem[] {
                    new MenuBarItem ("_App",
                        new MenuItem [] {
                            new MenuItem ("_Filters", "", () => _viewRegister.SwitchCurrentView(_filtersView)),
                            new MenuItem ("_Close", "", () => {
                                Application.RequestStop ();
                                Environment.Exit(0);
                            })
                        }
                    ),
                    new MenuBarItem ("_Sessions",
                        new MenuItem [] {
                            new MenuItem ("_List all sessions", "", () => _viewRegister.SwitchCurrentView(_sessionListView))
                        }
                    ),
                }
            );

        }

    }
}
