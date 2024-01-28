using GenerationRegionLogsAnalyzer.Enums;
using GenerationRegionLogsAnalyzer.LogModels;
using System.Numerics;

namespace DrawMapFromLog
{
    partial class DrawMapForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawMapFromFile(e.Graphics);
        }

        LogGroup GenerateLogGroupFromFilename(string finename)
        {
            try
            {
                string[] tokens = finename.Split(new char[] { '-' });
                return new LogGroup(tokens[0], ulong.Parse(tokens[2]), tokens[1] == "S" ? LogType.ServerSide : LogType.ClientSide);
            }
            catch
            {
                return null;
            }
        }

        private void DrawMapFromFile(Graphics g)
        {
            var file = _filesToDraw[_fileIndex];
            _fillerToDraw = new();
            _cellsToDraw = new();

            LogGroup logGroup = GenerateLogGroupFromFilename(Path.GetFileNameWithoutExtension(file));

            if (logGroup == null)
                return;

            foreach (var line in File.ReadAllLines(file))
            {
                AddingCell addCellLog = new AddingCell();

                if (addCellLog.TryMatch(line))
                {
                    if (line.ToLower().Contains("filler"))
                        _fillerToDraw.Add(addCellLog);
                    else
                        _cellsToDraw.Add(addCellLog);
                }
            }

            this.Text = Path.GetFileNameWithoutExtension(file);
            DrawMap(g);
        }

        private void DrawMap(Graphics g)
        {
            _toolTip = new ToolTip();
            _allLabelCells = new();

            _cellSize = _cellsToDraw.Where(k => k.CellPos.X != 0).Min(k => Math.Abs(k.CellPos.X));

            _areaBound = new Vector4(
                MathF.Min(_cellsToDraw.Min(k => k.CellPos.X), _fillerToDraw.Min(k => k.CellPos.X)),
                MathF.Min(_cellsToDraw.Min(k => k.CellPos.Y), _fillerToDraw.Min(k => k.CellPos.Y)),
                MathF.Max(_cellsToDraw.Max(k => k.CellPos.X), _fillerToDraw.Max(k => k.CellPos.X)),
                MathF.Max(_cellsToDraw.Max(k => k.CellPos.Y), _fillerToDraw.Max(k => k.CellPos.Y)));

            _scaleToForm = _dezoom * MathF.Max((_areaWidth + _cellSize) / ClientSize.Width, _areaHeight / ClientSize.Height);
            _borderToCenterMap = new Vector2((ClientSize.Width - _areaWidth / _scaleToForm) / 2, ((ClientSize.Height - _cellFormSize) - (_areaHeight / _scaleToForm)) / 2);
            _offset = new Vector2(-MathF.Min(0, _areaBound.X), -MathF.Min(0, _areaBound.Y));

            DrawXYAxis(g);

            if (_fillerCellsEnabled)
                DrawFillers(g);

            if (_regularCellsEnabled)
            {
                foreach (var cellToDraw in _cellsToDraw)
                    DrawCell(new Vector2(cellToDraw.CellPos.X, cellToDraw.CellPos.Y), cellToDraw);
            }
        }

        private List<AddingCell> _fillerToDraw;
        private List<AddingCell> _cellsToDraw;
        private List<Label> _allLabelCells;

        private Vector4 _areaBound;
        private float _cellSize;
        private float _areaWidth => MathF.Abs(_areaBound.Z - _areaBound.X);
        private float _areaHeight => MathF.Abs(_areaBound.W - _areaBound.Y);

        private Vector2 _offset;
        private float _scaleToForm;
        private float _cellFormSize => _cellSize / _scaleToForm - 1;

        private Vector2 _borderToCenterMap;
        private float _dezoom = 1.2f;
        private ToolTip _toolTip;

        private void DrawFillers(Graphics g)
        {
            foreach (var log in _fillerToDraw)
            {
                Vector2 pos = AdaptCoordinatesToForm(new Vector2(log.CellPos.X, log.CellPos.Y));
                AddLabelWithToolTip(pos.X, pos.Y, log, true);
            }
        }

        private Vector2 AdaptCoordinatesToForm(Vector2 pos)
            => new Vector2(ClientSize.Width - ((pos.X + _offset.X) / _scaleToForm + _borderToCenterMap.X), (ClientSize.Height - _cellFormSize) - ((_offset.Y + pos.Y) / _scaleToForm + _borderToCenterMap.Y));

        private void DrawCell(Vector2 pos, AddingCell log)
        {
            pos = AdaptCoordinatesToForm(pos);
            AddLabelWithToolTip(pos.X, pos.Y, log);
        }

        private void AddLabelWithToolTip(float x, float y, AddingCell log, bool isFiller = false)
        {
            if (_cellFormSize <= 0)
                return;

            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ForeColor = Color.White;
            label.BackColor = isFiller ? Color.LightGray : Color.Black;
            label.Font = new Font("Arial", _cellFormSize / 3);
            label.Size = new((int)_cellFormSize, (int)_cellFormSize);
            label.Location = new((int)x, (int)y);
            label.Text = log.CellId.ToString();
            Controls.Add(label);
            if (!isFiller)
                label.BringToFront();

            _toolTip.SetToolTip(label, log.CellName + " " + log.CellPos);
        }

        private void DrawXYAxis(Graphics g)
        {
            Vector2 origin = new Vector2((ClientSize.Width + _cellFormSize / 2) - (_offset.X / _scaleToForm + _borderToCenterMap.X), (ClientSize.Height - _cellFormSize / 2) - (_offset.Y / _scaleToForm + _borderToCenterMap.Y));
            g.DrawLine(Pens.Green, 0, origin.Y, ClientSize.Width, origin.Y);
            g.DrawLine(Pens.Red, origin.X, 0, origin.X, ClientSize.Height);
        }
    }
}