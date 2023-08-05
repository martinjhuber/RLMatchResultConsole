using RLMatchResultConsole.Common;
using RLMatchResultConsole.Data;
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

        public FiltersView(IViewRegister viewRegister, DataFilter dataFilter)
        {
            _viewRegister = viewRegister;
            _dataFilter = dataFilter;
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
            FrameView gameModeFrame = new FrameView("Allowed game modes:") { X = 1, Y = 1, Width = Dim.Fill(), Height = 16 };

            gameModeFrame.Add(cbDuel, cbDoubles, cbStandard, cbChaos, cbTournament, cbRankedOnly);
            cbDuel.Toggled += UpdateFilters;
            cbDoubles.Toggled += UpdateFilters;
            cbStandard.Toggled += UpdateFilters;
            cbTournament.Toggled += UpdateFilters;
            cbChaos.Toggled += UpdateFilters;
            cbRankedOnly.Toggled += UpdateFilters;

            Update();

            content.Add(l, gameModeFrame);

        }

        public override void Update()
        {
            cbDuel.Checked = _dataFilter.GameModeFilters.Contains(Models.GameMode.Duel);
            cbDoubles.Checked = _dataFilter.GameModeFilters.Contains(Models.GameMode.Doubles);
            cbStandard.Checked = _dataFilter.GameModeFilters.Contains(Models.GameMode.Standard);
            cbChaos.Checked = _dataFilter.GameModeFilters.Contains(Models.GameMode.Chaos);
            cbTournament.Checked = _dataFilter.GameModeFilters.Contains(Models.GameMode.Tournament);
            cbRankedOnly.Checked = _dataFilter.RankedOnly;
        }

        private void UpdateFilters(bool check)
        {
            _dataFilter.GameModeFilters.Clear();
            if (cbDuel.Checked) { _dataFilter.GameModeFilters.Add(Models.GameMode.Duel); }
            if (cbDoubles.Checked) { _dataFilter.GameModeFilters.Add(Models.GameMode.Doubles); }
            if (cbStandard.Checked) { _dataFilter.GameModeFilters.Add(Models.GameMode.Standard); }
            if (cbChaos.Checked) { _dataFilter.GameModeFilters.Add(Models.GameMode.Chaos); }
            if (cbTournament.Checked) { _dataFilter.GameModeFilters.Add(Models.GameMode.Tournament); }

            _dataFilter.SetRankedOnlyFilter(cbRankedOnly.Checked);

        }
    }
}
