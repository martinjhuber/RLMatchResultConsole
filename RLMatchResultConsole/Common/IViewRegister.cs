using RLMatchResultConsole.Models;
using RLMatchResultConsole.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Common
{
    internal interface IViewRegister
    {

        Window ContentWindow { get; set; }
        StatusBar StatusBar { get; set; }

        void SwitchCurrentView(AbstractView view, bool addToStack = false);
        void UpdateCurrentView();
        void SwitchToPreviousView();

        void ShowError(string message, string title = "Error");
        void ShowStatus(string message);

    }
}
