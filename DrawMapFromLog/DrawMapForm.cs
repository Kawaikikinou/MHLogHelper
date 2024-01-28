namespace DrawMapFromLog
{
    public partial class DrawMapForm : Form
    {
        private string[] _filesToDraw;
        private int _fileIndex;
        private bool _regularCellsEnabled = true;
        private bool _fillerCellsEnabled = true;
        private ToolStripMenuItem _regularCellsMenuItem;
        private ToolStripMenuItem _fillerCellsMenuItem;

        public DrawMapForm()
        {
            InitializeMapList();
            InitializeComponent();
            InitializeFileMenu();
            Resize += MainForm_Resize;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void InitializeMapList()
        {
            string logsFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../../LogsToDraw");
            _filesToDraw = Directory.GetFiles(logsFolder);
        }

        private void InitializeFileMenu()
        {
            ToolStripMenuItem fileMenu = new("File");
            ToolStripMenuItem previousMapMenuItem = new("Previous map");
            ToolStripMenuItem nextMapMenuItem = new("Next map");
            ToolStripMenuItem saveScreenshotMenuItem = new("Save screenshot");

            ToolStripMenuItem filtersMenu = new ToolStripMenuItem("Filters");
            _regularCellsMenuItem = new ToolStripMenuItem("Regular cells");
            _fillerCellsMenuItem = new ToolStripMenuItem("Filler cells");

            previousMapMenuItem.ShortcutKeys = Keys.Control | Keys.Left;
            nextMapMenuItem.ShortcutKeys = Keys.Control | Keys.Right;
            saveScreenshotMenuItem.ShortcutKeys = Keys.Control | Keys.S;

            _regularCellsMenuItem.ShortcutKeys |= Keys.Control | Keys.R;
            _fillerCellsMenuItem.ShortcutKeys |= Keys.Control | Keys.F;

            previousMapMenuItem.Click += PreviousMapMenuItem_Click;
            nextMapMenuItem.Click += NextMapMenuItem_Click;
            saveScreenshotMenuItem.Click += SaveScreenshotMenuItem_Click;

            _regularCellsMenuItem.Checked = _regularCellsEnabled;
            _regularCellsMenuItem.Click += RegularCellsMenuItem_Click;

            _fillerCellsMenuItem.Checked = _fillerCellsEnabled;
            _fillerCellsMenuItem.Click += FillerCellsMenuItem_Click;

            fileMenu.DropDownItems.Add(previousMapMenuItem);
            fileMenu.DropDownItems.Add(nextMapMenuItem);
            fileMenu.DropDownItems.Add(saveScreenshotMenuItem);

            filtersMenu.DropDownItems.Add(_regularCellsMenuItem);
            filtersMenu.DropDownItems.Add(_fillerCellsMenuItem);

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(filtersMenu);

            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);
        }

        private void PreviousMapMenuItem_Click(object sender, EventArgs e) => SelectMap(--_fileIndex);

        private void NextMapMenuItem_Click(object sender, EventArgs e) => SelectMap(++_fileIndex);

        private void SelectMap(int i)
        {
            if (i < 0)
                _fileIndex = _filesToDraw.Length - 1;

            if (i >= _filesToDraw.Length)
                _fileIndex = 0;

            Refresh();
        }

        private void SaveScreenshotMenuItem_Click(object sender, EventArgs e)
        {
            CaptureAndSaveScreenshot();
        }

        private void RegularCellsMenuItem_Click(object sender, EventArgs e)
        {
            _regularCellsEnabled = !_regularCellsEnabled;
            _regularCellsMenuItem.Checked = _regularCellsEnabled;
            Refresh();
        }

        private void FillerCellsMenuItem_Click(object sender, EventArgs e)
        {
            _fillerCellsEnabled = !_fillerCellsEnabled;
            _fillerCellsMenuItem.Checked = _fillerCellsEnabled;
            Refresh();
        }

        private void CaptureAndSaveScreenshot()
        {
            int width = this.Width;
            int height = this.Height;

            using (Bitmap bitmap = new(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, new Size(width, height));
                }

                string filePath = this.Text;
                bitmap.Save(filePath + ".png");
                MessageBox.Show($"Screenshot of {filePath} saved in bin folder");
            }
        }

        private void Refresh()
        {
            Controls.Clear();
            InitializeFileMenu();
            Invalidate();
        }
    }
}