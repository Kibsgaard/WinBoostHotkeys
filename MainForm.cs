using System;
using System.Windows.Forms;

namespace WinBoostHotkeys
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Hide form from taskbar and make it invisible
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new System.Drawing.Size(0, 0);
        }

        protected override void SetVisibleCore(bool value)
        {
            // Keep form hidden
            base.SetVisibleCore(false);
        }
    }
}
