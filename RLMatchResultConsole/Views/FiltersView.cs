using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
using RLMatchResultConsole.Models;
using Terminal.Gui;

namespace RLMatchResultConsole.Views
{
    internal class FiltersView : AbstractView
    {

        private readonly IViewRegister _viewRegister;
        private readonly DataFilter _dataFilter;

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
            FrameView gameModeFrame = new FrameView("Allowed game modes:") { X = 1, Y = 2, Width = Dim.Fill(), Height = 13 };

            for (int i = 0; i < _shownGameModeBoxes.Length; i++)
            {
                GameMode gm = _shownGameModeBoxes[i];

                _checkBoxes[i] = new CheckBox(2, i+1, gm.ToViewString());
                _checkBoxes[i].Toggled += (isChecked) => BoxToggled(isChecked, gm);
                gameModeFrame.Add(_checkBoxes[i]);
            }

            FrameView globalSettingsFrame = new FrameView("Global settings:") { X = 1, Y = 15, Width = Dim.Fill(), Height = 7 };

            _cbRankedOnly = new CheckBox(2, 1, "Ranked only");
            _cbRankedOnly.Toggled += (isChecked) =>
            {
                // the status at the moment of clicking is given. we have to reverse that
                _dataFilter.RankedOnly = !isChecked;
            };
            globalSettingsFrame.Add(_cbRankedOnly);

            _cbDisableFilters = new CheckBox(2, 3, "Disable all filters");
            _cbDisableFilters.Toggled += (isChecked) =>
            {
                _dataFilter.DisableFilters = !isChecked;
                UpdateEnableStatus();
            };
            globalSettingsFrame.Add(_cbDisableFilters);

            Update();

            content.Add(l, gameModeFrame, globalSettingsFrame);

        }

        public override void Update()
        {
            for (int i = 0; i < _shownGameModeBoxes.Length; i++)
            {
                _checkBoxes[i].Checked = _dataFilter.IsGameModeShown(_shownGameModeBoxes[i]);
            }
            _cbRankedOnly.Checked = _dataFilter.RankedOnly;
            _cbDisableFilters.Checked = _dataFilter.DisableFilters;
            UpdateEnableStatus();
        }

        public void UpdateEnableStatus()
        {
            for (int i = 0; i < _shownGameModeBoxes.Length; i++)
            {
                _checkBoxes[i].Enabled = !_dataFilter.DisableFilters;
            }
            _cbRankedOnly.Enabled = !_dataFilter.DisableFilters;
        }

        private void BoxToggled(bool isChecked, GameMode gm)
        {
            isChecked = !isChecked;     // the status at the moment of clicking is given. we have to reverse that
            _dataFilter.SetGameModeFilter(gm, isChecked);
        }
    }
}
