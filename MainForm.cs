using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinBoostHotkeys.Resources;

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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

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
            _toggleBoostMenuItem = new ToolStripMenuItem(Strings.MenuBoost);
            _toggleBoostMenuItem.Click += ToggleBoostMenuItem_Click;
            _contextMenu.Items.Add(_toggleBoostMenuItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            // Settings menu item
            var settingsMenuItem = new ToolStripMenuItem(Strings.MenuSettings);
            settingsMenuItem.Click += SettingsMenuItem_Click;
            _contextMenu.Items.Add(settingsMenuItem);

            // Exit menu item
            var exitMenuItem = new ToolStripMenuItem(Strings.MenuExit);
            exitMenuItem.Click += ExitMenuItem_Click;
            _contextMenu.Items.Add(exitMenuItem);

            // Create tray icon
            _trayIcon = new NotifyIcon
            {
                Icon = IconHelper.CreateBlueIcon(), // Default to OFF (blue)
                Text = Strings.AppName,
                Visible = true
            };

            _trayIcon.MouseClick += TrayIcon_MouseClick;
        }

        private void InitializeHotkeys()
        {
            // Force handle creation
            _ = this.Handle;
            
            _hotkeyManager = new HotkeyManager(this.Handle);
            _hotkeyManager.HotkeyOnPressed += HotkeyManager_HotkeyOnPressed;
            _hotkeyManager.HotkeyOffPressed += HotkeyManager_HotkeyOffPressed;
            _hotkeyManager.HotkeyTogglePressed += HotkeyManager_HotkeyTogglePressed;
 
            // Register hotkeys from settings
            if (!_hotkeyManager.RegisterHotkeys(_settings.HotkeyOn, _settings.HotkeyOff))
            {
                MessageBox.Show(Strings.ErrorHotkeyRegistrationFailed, Strings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                else if (!rule.IsEthernet)
                {
                    if(string.IsNullOrEmpty(rule.Ssid))
                    {
                        matches = true;
                    } else
                    {
                        matches = e.WifiSsid == rule.Ssid;
                    }   
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

        private void HotkeyManager_HotkeyTogglePressed(object? sender, EventArgs e)
        {
            ToggleBoost();
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
                    Strings.ErrorBoostModeFailed,
                    Strings.ErrorTitle,
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

            _toggleBoostMenuItem.Text = Strings.MenuBoost;
            _toggleBoostMenuItem.Checked = _currentBoostMode == BoostMode.Aggressive;
        }

        private void ToggleBoostMenuItem_Click(object? sender, EventArgs e)
        {
            ToggleBoost();
        }

        private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleBoost();
            }
            else if (e.Button == MouseButtons.Right)
            {
                SetForegroundWindow(this.Handle);
                _contextMenu?.Show(Cursor.Position);
            }
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
            // Unregister hotkeys temporarily so SettingsForm can capture them if they happen to be pressed during assignment
            _hotkeyManager?.UnregisterAll();

            _settings = SettingsManager.Load();
            using var settingsForm = new SettingsForm
            {
                Settings = _settings
            };

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                SettingsManager.Save(_settings);
                // Re-register hotkeys with new settings
                if (!(_hotkeyManager?.RegisterHotkeys(_settings.HotkeyOn, _settings.HotkeyOff) ?? true))
                {
                    MessageBox.Show(Strings.ErrorHotkeyRegistrationFailed, Strings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                // Network rules are automatically checked on next network change
            }
            else
            {
                // Re-register old hotkeys if cancelled
                _hotkeyManager?.RegisterHotkeys(_settings.HotkeyOn, _settings.HotkeyOff);
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
