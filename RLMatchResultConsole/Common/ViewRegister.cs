using RLMatchResultConsole.Models;
using RLMatchResultConsole.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Common
{
    internal class ViewRegister : IViewRegister
    {

        private AbstractView? _currentView = null;
        private Stack<AbstractView> _viewStack = new Stack<AbstractView>();

        public Window ContentWindow { get; set; } = new Window();

        public StatusBar StatusBar { get; set; } = new StatusBar();

        public void UpdateCurrentView()
        {
            if (_currentView != null)
            {
                _currentView.Update();
            }
        }

        public void SwitchCurrentView(AbstractView view, bool addToStack = false)
        {
            if (addToStack && _currentView is not null)
            {
                _viewStack.Push(_currentView);
            }

            ContentWindow.RemoveAll();
            _currentView = view;
            _currentView.Start();
            ContentWindow.Title = view.GetTitle();
            StatusBar.Items = new StatusItem[] { };
        }

        public void SwitchToPreviousView()
        {
            if (_viewStack.Count > 0) {
                var view = _viewStack.Pop();
                SwitchCurrentView(view, false);
                UpdateCurrentView();
            }
        }

        public void ShowError(string message, string title = "Error")
        {
            var n = MessageBox.Query(title, message, "OK", "Quit");
            if (n == 1) { 
                Application.RequestStop();
                Environment.Exit(-2);
            }

        }

        public void ShowStatus(string message)
        {
            StatusBar.Items = new StatusItem[] {
                new StatusItem(Key.Null, message, () => { })
            }; 
        }

    }
}
