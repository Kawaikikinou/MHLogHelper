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

        private void DrawMapFromFile(Graphics g)
        {
            var file = _filesToDraw[_fileIndex];

            List<AddingCell> logs = new();
            LogGroup logGroup = GenerateLogGroupFromFilename(Path.GetFileNameWithoutExtension(file));

            if (logGroup == null)
                return;

            foreach (var line in File.ReadAllLines(file))
            {
                if (line.ToLower().Contains("filler"))
                    continue;

                AddingCell addCellLog = new AddingCell();

                if (addCellLog.TryMatch(line))
                    logs.Add(addCellLog);
            }

            this.Text = Path.GetFileNameWithoutExtension(file);
            DrawMap(g, logs);
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

        private void DrawMap(Graphics g, List<AddingCell> logs)
        {
            _toolTip = new ToolTip();
            _cellSize = logs.Where(k => k.CellPos.X != 0).Min(k => Math.Abs(k.CellPos.X));

            _areaBound = new Vector4(
                logs.Min(k => k.CellPos.X),
                logs.Min(k => k.CellPos.Y),
                logs.Max(k => k.CellPos.X),
                logs.Max(k => k.CellPos.Y));

            _scaleToForm = _dezoom * MathF.Max((_areaWidth + _cellSize) / ClientSize.Height, _areaHeight / ClientSize.Width);
            _borderToCenterMap = new Vector2((ClientSize.Width - _areaHeight / _scaleToForm) / 2, ((ClientSize.Height - _cellFormSize) - (_areaWidth / _scaleToForm)) / 2);
            _offset = new Vector2(-MathF.Min(0, _areaBound.X), -MathF.Min(0, _areaBound.Y));

            DrawXYAxis(g);
            foreach (var log in logs)
                DrawCell(new Vector2(log.CellPos.X, log.CellPos.Y), log);
        }

        private Vector2 AdaptCoordinatesToForm(Vector2 pos)
            => new Vector2((_offset.Y + pos.Y) / _scaleToForm + _borderToCenterMap.X, (ClientSize.Height - _cellFormSize) - ((pos.X + _offset.X) / _scaleToForm + _borderToCenterMap.Y));

        private void DrawCell(Vector2 pos, AddingCell log)
        {
            pos = AdaptCoordinatesToForm(pos);
            AddLabelWithToolTip(pos.X, pos.Y, log);
        }

        private void AddLabelWithToolTip(float x, float y, AddingCell log)
        {
            if (_cellFormSize <= 0)
                return;

            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ForeColor = Color.White;
            label.BackColor = Color.Black;
            label.Font = new Font("Arial", _cellFormSize / 3);
            label.Size = new((int)_cellFormSize, (int)_cellFormSize);
            label.Location = new((int)x, (int)y);
            label.Text = log.CellId.ToString();
            Controls.Add(label);
            _toolTip.SetToolTip(label, log.CellName + " " + log.CellPos);
        }

        private void DrawXYAxis(Graphics g)
        {
            Vector2 origin = new Vector2(_offset.Y / _scaleToForm + _borderToCenterMap.X, ClientSize.Height - (_offset.X / _scaleToForm + _borderToCenterMap.Y));
            g.DrawLine(Pens.Red, 0, origin.Y, ClientSize.Width, origin.Y);
            g.DrawLine(Pens.Green, origin.X, 0, origin.X, ClientSize.Height);
        }
    }
}