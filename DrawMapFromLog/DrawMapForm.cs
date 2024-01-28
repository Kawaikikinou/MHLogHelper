namespace DrawMapFromLog
{
    public partial class DrawMapForm : Form
    {
        private string[] _filesToDraw;
        private int _fileIndex;
        public DrawMapForm()
        {
            InitializeMapList();
            InitializeComponent();
            InitializeFileMenu();
            Resize += MainForm_Resize;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Controls.Clear();
            InitializeFileMenu();
            Invalidate();
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

            previousMapMenuItem.ShortcutKeys = Keys.Control | Keys.Left;
            nextMapMenuItem.ShortcutKeys = Keys.Control | Keys.Right;
            saveScreenshotMenuItem.ShortcutKeys = Keys.Control | Keys.S;

            previousMapMenuItem.Click += PreviousMapMenuItem_Click;
            nextMapMenuItem.Click += NextMapMenuItem_Click;
            saveScreenshotMenuItem.Click += SaveScreenshotMenuItem_Click;


            fileMenu.DropDownItems.Add(previousMapMenuItem);
            fileMenu.DropDownItems.Add(nextMapMenuItem);
            fileMenu.DropDownItems.Add(saveScreenshotMenuItem);

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(fileMenu);

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

            Controls.Clear();
            InitializeFileMenu();
            Invalidate();
        }

        private void SaveScreenshotMenuItem_Click(object sender, EventArgs e)
        {
            CaptureAndSaveScreenshot();
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
    }
}