using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinBoostHotkeys
{
    public partial class MainForm : Form
    {
        private NotifyIcon? _trayIcon;
        private ContextMenuStrip? _contextMenu;
        private ToolStripMenuItem? _toggleBoostMenuItem;
        private PowerPlanManager _powerPlanManager;
        private BoostMode? _currentBoostMode;

        public MainForm()
        {
            _powerPlanManager = new PowerPlanManager();
            InitializeComponent();
            InitializeTrayIcon();
            UpdateBoostState();
        }

        private void InitializeComponent()
        {
            // Hide form from taskbar and make it invisible
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(0, 0);
        }

        private void InitializeTrayIcon()
        {
            _contextMenu = new ContextMenuStrip();

            // Toggle Boost menu item
            _toggleBoostMenuItem = new ToolStripMenuItem("Toggle Boost");
            _toggleBoostMenuItem.Click += ToggleBoostMenuItem_Click;
            _contextMenu.Items.Add(_toggleBoostMenuItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            // Settings menu item (placeholder for now)
            var settingsMenuItem = new ToolStripMenuItem("Settings");
            settingsMenuItem.Click += SettingsMenuItem_Click;
            _contextMenu.Items.Add(settingsMenuItem);

            // Exit menu item
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;
            _contextMenu.Items.Add(exitMenuItem);

            // Create tray icon
            _trayIcon = new NotifyIcon
            {
                Icon = IconHelper.CreateBlueIcon(), // Default to OFF (blue)
                ContextMenuStrip = _contextMenu,
                Text = "WinBoostHotkeys",
                Visible = true
            };

            _trayIcon.DoubleClick += TrayIcon_DoubleClick;
        }

        private void UpdateBoostState()
        {
            _currentBoostMode = _powerPlanManager.GetCurrentBoostMode();
            UpdateIcon();
            UpdateMenuText();
        }

        private void UpdateIcon()
        {
            if (_trayIcon == null) return;

            Icon? oldIcon = _trayIcon.Icon;
            _trayIcon.Icon = _currentBoostMode == BoostMode.Aggressive
                ? IconHelper.CreateGreenIcon()
                : IconHelper.CreateBlueIcon();
            
            oldIcon?.Dispose();
        }

        private void UpdateMenuText()
        {
            if (_toggleBoostMenuItem == null) return;

            string state = _currentBoostMode == BoostMode.Aggressive ? "On" : "Off";
            _toggleBoostMenuItem.Text = $"Boost ({state})";
            _toggleBoostMenuItem.Checked = _currentBoostMode == BoostMode.Aggressive;
        }

        private void ToggleBoostMenuItem_Click(object? sender, EventArgs e)
        {
            ToggleBoost();
        }

        private void TrayIcon_DoubleClick(object? sender, EventArgs e)
        {
            ToggleBoost();
        }

        private void ToggleBoost()
        {
            BoostMode newMode = _currentBoostMode == BoostMode.Aggressive
                ? BoostMode.Disabled
                : BoostMode.Aggressive;

            if (_powerPlanManager.SetBoostMode(newMode))
            {
                _currentBoostMode = newMode;
                UpdateIcon();
                UpdateMenuText();
            }
            else
            {
                MessageBox.Show(
                    "Failed to change boost mode. Please ensure the application is running with administrator privileges.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SettingsMenuItem_Click(object? sender, EventArgs e)
        {
            // Placeholder for settings form (Task 5)
            MessageBox.Show("Settings will be implemented in Phase 2.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void SetVisibleCore(bool value)
        {
            // Keep form hidden
            base.SetVisibleCore(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _trayIcon?.Icon?.Dispose();
                _trayIcon?.Dispose();
                _contextMenu?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
