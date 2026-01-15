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
        private Settings _settings;
        private HotkeyManager? _hotkeyManager;
        private NetworkMonitor? _networkMonitor;

        public MainForm()
        {
            _powerPlanManager = new PowerPlanManager();
            _settings = SettingsManager.Load();
            InitializeComponent();
            InitializeTrayIcon();
            InitializeHotkeys();
            InitializeNetworkMonitor();
            ApplyLaunchState();
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

        private void InitializeHotkeys()
        {
            // Force handle creation
            _ = this.Handle;
            
            _hotkeyManager = new HotkeyManager(this.Handle);
            _hotkeyManager.HotkeyOnPressed += HotkeyManager_HotkeyOnPressed;
            _hotkeyManager.HotkeyOffPressed += HotkeyManager_HotkeyOffPressed;

            // Register hotkeys from settings
            _hotkeyManager.RegisterHotkeyOn(_settings.HotkeyOn);
            _hotkeyManager.RegisterHotkeyOff(_settings.HotkeyOff);
        }

        private void InitializeNetworkMonitor()
        {
            _networkMonitor = new NetworkMonitor();
            _networkMonitor.NetworkChanged += NetworkMonitor_NetworkChanged;
            _networkMonitor.StartMonitoring();
        }

        private void NetworkMonitor_NetworkChanged(object? sender, NetworkChangedEventArgs e)
        {
            // Invoke on UI thread if needed
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => NetworkMonitor_NetworkChanged(sender, e)));
                return;
            }

            // Check network rules and apply matching boost mode
            foreach (var rule in _settings.NetworkRules)
            {
                bool matches = false;

                if (rule.IsEthernet && e.IsEthernetConnected)
                {
                    matches = true;
                }
                else if (!rule.IsEthernet && !string.IsNullOrEmpty(rule.Ssid))
                {
                    matches = e.WifiSsid == rule.Ssid;
                }

                if (matches)
                {
                    // Apply the boost mode from the rule
                    SetBoostMode(rule.BoostMode);
                    break; // Apply first matching rule
                }
            }
        }

        private void HotkeyManager_HotkeyOnPressed(object? sender, EventArgs e)
        {
            SetBoostMode(BoostMode.Aggressive);
        }

        private void HotkeyManager_HotkeyOffPressed(object? sender, EventArgs e)
        {
            SetBoostMode(BoostMode.Disabled);
        }

        private void SetBoostMode(BoostMode mode)
        {
            if (_powerPlanManager.SetBoostMode(mode))
            {
                _currentBoostMode = mode;
                _settings.PreviousBoostMode = mode;
                SettingsManager.Save(_settings);
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

            SetBoostMode(newMode);
        }

        private void SettingsMenuItem_Click(object? sender, EventArgs e)
        {
            _settings = SettingsManager.Load();
            using var settingsForm = new SettingsForm
            {
                Settings = _settings
            };

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                SettingsManager.Save(_settings);
                // Re-register hotkeys with new settings
                _hotkeyManager?.RegisterHotkeyOn(_settings.HotkeyOn);
                _hotkeyManager?.RegisterHotkeyOff(_settings.HotkeyOff);
                // Network rules are automatically checked on next network change
            }
        }

        private void ApplyLaunchState()
        {
            switch (_settings.LaunchState)
            {
                case LaunchState.On:
                    _powerPlanManager.SetBoostMode(BoostMode.Aggressive);
                    break;
                case LaunchState.Off:
                    _powerPlanManager.SetBoostMode(BoostMode.Disabled);
                    break;
                case LaunchState.Previous:
                    if (_settings.PreviousBoostMode.HasValue)
                    {
                        _powerPlanManager.SetBoostMode(_settings.PreviousBoostMode.Value);
                    }
                    break;
            }
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

        protected override void WndProc(ref Message m)
        {
            _hotkeyManager?.ProcessMessage(m);
            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _networkMonitor?.Dispose();
                _hotkeyManager?.Dispose();
                _trayIcon?.Icon?.Dispose();
                _trayIcon?.Dispose();
                _contextMenu?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
