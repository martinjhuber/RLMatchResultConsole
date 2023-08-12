using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Components
{
    internal class RLTableComponent
    {
        private TableView _tableView;
        private DataTable _dataTable;

        public ColorScheme NormalNoFocus { get; } = new ColorScheme();
        public ColorScheme MatchWin { get; } = new ColorScheme();
        public ColorScheme MatchLoss { get; } = new ColorScheme();
        public ColorScheme RowHighlight { get; } = new ColorScheme();

        public RLTableComponent(Pos X, Pos Y, Dim Width, Dim Height, string dataTableName = "Table")
        {
            _dataTable = new DataTable(dataTableName);
            _tableView = new TableView(_dataTable) { X = X, Y = Y, Width = Width, Height = Height };

            var tableStyle = new TableView.TableStyle();
            tableStyle.AlwaysShowHeaders = true;
            tableStyle.ShowHorizontalHeaderOverline = false;
            _tableView.Style = tableStyle;

            InitColorSchemes();
        }

        private void InitColorSchemes()
        {
            NormalNoFocus.Focus = NormalNoFocus.HotFocus = NormalNoFocus.Normal = NormalNoFocus.HotNormal = Colors.Base.Normal;

            MatchWin.Normal = Application.Driver.MakeAttribute(Color.White, Color.BrightGreen);
            MatchWin.Focus = MatchWin.HotFocus = MatchWin.HotNormal = MatchWin.Normal;

            MatchLoss.Normal = Application.Driver.MakeAttribute(Color.White, Color.BrightRed);
            MatchLoss.Focus = MatchLoss.HotFocus = MatchLoss.HotNormal = MatchLoss.Normal;

            RowHighlight.Normal = Colors.Menu.Normal;
            RowHighlight.Focus = RowHighlight.HotFocus = RowHighlight.HotNormal = RowHighlight.Normal;
        }

        public DataColumn AddColumn(
            Type type,
            string name,
            int minWidth = 1,
            int maxWidth = 100,
            TextAlignment align = TextAlignment.Left,
            ColorScheme? scheme = null)
        {
            DataColumn column = new DataColumn() { DataType = type, ColumnName = name };
            _dataTable.Columns.Add(column);

            var style = _tableView.Style.GetOrCreateColumnStyle(column);
            style.MinWidth = minWidth;
            style.MaxWidth = maxWidth;
            style.Alignment = align;

            if (scheme is not null)
            {
                style.ColorGetter = (args) => scheme;
            }

            return column;
        }

        public void SetColumnColorsByValue(int colIndex, Dictionary<string, ColorScheme> colorValues)
        {
            if (_tableView.Table.Columns.Count > colIndex)
            {
                DataColumn column = _tableView.Table.Columns[colIndex];
                SetColumnColorsByValue(column, colorValues);
            }
        }

        public void SetColumnColorsByValue(DataColumn column, Dictionary<string, ColorScheme> colorValues)
        {
            var style = _tableView.Style.GetOrCreateColumnStyle(column);
            style.ColorGetter = (args) =>
            {
                if (colorValues.ContainsKey(args.CellValue.ToString() ?? ""))
                {
                    return colorValues[args.CellValue.ToString() ?? ""];
                }
                return null;
            };
        }

        public void SetRowColors(Dictionary<int, ColorScheme> colorRows)
        {
            _tableView.Style.RowColorGetter = (args) =>
            {
                if (colorRows.ContainsKey(args.RowIndex))
                {
                    return colorRows[args.RowIndex];
                }
                return null;
            };
        }

        public void AddToView(View view)
        {
            view.Add(_tableView);
        }

        public void ClearRows()
        {
            _dataTable.Rows.Clear();
        }

        public DataRow AddRow(params object?[] values)
        {
            return _dataTable.Rows.Add(values);
        }

        public void Update()
        {
            _tableView.Update();
        }
        public void Focus()
        {
            _tableView.FocusFirst();
        }

        public void SortBy(string columnName, bool descending = true)
        {
            _tableView.Table.DefaultView.Sort = columnName + " " + (descending ? "DESC" : "ASC");

            var sortedCopy = _tableView.Table.DefaultView.ToTable();
            _tableView.Table.Rows.Clear();
            foreach (DataRow r in sortedCopy.Rows)
            {
                _tableView.Table.ImportRow(r);
            }
        }

        public void SortBy(int columnIndex, bool descending = true)
        {
            SortBy(_dataTable.Columns[columnIndex].ColumnName, descending);
        }

        public bool FullRowSelect
        {
            get { return _tableView.FullRowSelect; }
            set { _tableView.FullRowSelect = value; }
        }

        public delegate void CellActionCallback(int columnId, int rowId);

        public void AddCellActionCallback(CellActionCallback cellActionCallback)
        {
            _tableView.MouseClick += (e) =>
            {
                Point? cellIndex = _tableView.ScreenToCell(e.MouseEvent.X, e.MouseEvent.Y);
                if (cellIndex.HasValue && e.MouseEvent.Flags.HasFlag(MouseFlags.Button1DoubleClicked))
                {
                    e.Handled = true;
                    cellActionCallback(cellIndex.Value.X, cellIndex.Value.Y);
                }
            };
            _tableView.KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter || e.KeyEvent.Key == Key.Space)
                {
                    e.Handled = true;
                    cellActionCallback(_tableView.SelectedColumn, _tableView.SelectedRow);
                }
            };
        }

    }
}
