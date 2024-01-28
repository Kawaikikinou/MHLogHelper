namespace DrawMapFromLog
{
    public partial class DrawMapForm : Form
    {
        public DrawMapForm()
        {
            InitializeComponent();
            Resize += MainForm_Resize;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}