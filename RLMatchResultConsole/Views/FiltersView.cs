using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class FiltersView : AbstractView
    {

        private readonly IViewRegister _viewRegister;
        private readonly DataFilter _dataFilter;

        private CheckBox cbDuel = new CheckBox(2, 1, "Duel");
        private CheckBox cbDoubles = new CheckBox(2, 3, "Doubles");
        private CheckBox cbStandard = new CheckBox(2, 5, "Standard");
        private CheckBox cbChaos = new CheckBox(2, 7, "Chaos");
        private CheckBox cbTournament = new CheckBox(2, 9, "Tournament");
        private CheckBox cbRankedOnly = new CheckBox(2, 12, "Ranked only");

        private readonly GameMode[] _shownGameModeBoxes = new GameMode[] {
            GameMode.Duel, GameMode.Doubles, GameMode.Standard, GameMode.Chaos, GameMode.Tournament,
            GameMode.Rumble, GameMode.Dropshot, GameMode.Hoops, GameMode.SnowDay };

        private CheckBox[] _checkBoxes;
        private CheckBox _cbRankedOnly = new CheckBox();
        private CheckBox _cbDisableFilters = new CheckBox();

        public FiltersView(IViewRegister viewRegister, DataFilter dataFilter)
        {
            _viewRegister = viewRegister;
            _dataFilter = dataFilter;
            _checkBoxes = new CheckBox[_shownGameModeBoxes.Length];
        }

        public override void Execute()
        {
            // TODO
        }

        public override string GetTitle()
        {
            return "Data Filters";
        }

        public override void InitGui()
        {
            var content = _viewRegister.ContentWindow;

            Label l = new Label(1, 0, "Configure which filters are applied.");
            FrameView gameModeFrame = new FrameView("Allowed game modes:") { X = 1, Y = 1, Width = Dim.Fill(), Height = 17 };

            for (int i = 0; i < _shownGameModeBoxes.Length; i++)
            {
                GameMode gm = _shownGameModeBoxes[i];

                _checkBoxes[i] = new CheckBox(2, i+1, gm.ToViewString());
                _checkBoxes[i].Toggled += (isChecked) => BoxToggled(isChecked, gm);
                gameModeFrame.Add(_checkBoxes[i]);
            }

            _cbRankedOnly = new CheckBox(2, _shownGameModeBoxes.Length + 2, "Ranked only");
            _cbRankedOnly.Toggled += (isChecked) =>
            {
                // the status at the moment of clicking is given. we have to reverse that
                _dataFilter.SetRankedOnlyFilter(!isChecked);
            };
            gameModeFrame.Add(_cbRankedOnly);

            _cbDisableFilters = new CheckBox(2, _shownGameModeBoxes.Length + 4, "Disable all filters");
            _cbDisableFilters.Toggled += (isChecked) =>
            {
                _dataFilter.SetDisableFilters(!isChecked);
            };
            gameModeFrame.Add(_cbDisableFilters);

            Update();

            content.Add(l, gameModeFrame);

        }

        public override void Update()
        {
            for (int i = 0; i < _shownGameModeBoxes.Length; i++)
            {
                _checkBoxes[i].Checked = _dataFilter.GameModeFilters.Contains(_shownGameModeBoxes[i]);
            }
            _cbRankedOnly.Checked = _dataFilter.RankedOnly;
            _cbDisableFilters.Checked = _dataFilter.DisableFilters;
        }

        private void BoxToggled(bool isChecked, GameMode gm)
        {
            isChecked = !isChecked;     // the status at the moment of clicking is given. we have to reverse that
            if (!isChecked && _dataFilter.GameModeFilters.Contains(gm))
            {
                _dataFilter.GameModeFilters.Remove(gm);
            }
            if (isChecked && !_dataFilter.GameModeFilters.Contains(gm))
            {
                _dataFilter.GameModeFilters.Add(gm);
            }

        }
    }
}
