using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinBoostHotkeys.Resources;

namespace WinBoostHotkeys
{
    public partial class SettingsForm : Form
    {
        private Settings _settings;
        private HotkeyCaptureControl _hotkeyOnControl = null!;
        private HotkeyCaptureControl _hotkeyOffControl = null!;
        private ComboBox _launchStateComboBox = null!;
        private CheckBox _autoLaunchCheckBox = null!;
        private DataGridView _networkRulesGrid = null!;
        private Button _okButton = null!;
        private Button _cancelButton = null!;
        private Button _addRuleButton = null!;
        private Button _removeRuleButton = null!;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                LoadSettings();
            }
        }

        public SettingsForm()
        {
            _settings = Settings.GetDefault();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var version = Application.ProductVersion.Split('+')[0];
            this.Text = $"{Application.ProductName} v{version} - {Strings.SettingsTitle}";
            this.ClientSize = new Size(600, 540);
            this.MinimumSize = new Size(550, 450);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 9,
                Padding = new Padding(15)
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Hotkey On
            var hotkeyOnLabel = new Label
            {
                Text = Strings.LabelHotkeyBoostOn,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 10)
            };
            _hotkeyOnControl = new HotkeyCaptureControl
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Hotkey Off
            var hotkeyOffLabel = new Label
            {
                Text = Strings.LabelHotkeyBoostOff,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 10)
            };
            _hotkeyOffControl = new HotkeyCaptureControl
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Launch State
            var launchStateLabel = new Label
            {
                Text = Strings.LabelLaunchState,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 10)
            };
            _launchStateComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 0, 0, 10)
            };
            _launchStateComboBox.Items.AddRange(new object[] { Strings.LaunchStatePrevious, Strings.BoostOn, Strings.BoostOff });

            // Auto Launch
            _autoLaunchCheckBox = new CheckBox
            {
                Text = Strings.LabelStartWithWindows,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };

            // Network Rules Label
            var networkRulesLabel = new Label
            {
                Text = Strings.LabelNetworkRules,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };

            // Network Rules Info Label
            var networkRulesInfoLabel = new Label
            {
                Text = Strings.LabelNetworkRulesInfo,
                AutoSize = true,
                UseMnemonic = false,
                ForeColor = SystemColors.GrayText,
                Margin = new Padding(0, 0, 0, 5)
            };

            // Grid
            _networkRulesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Margin = new Padding(0, 0, 0, 10),
                BackgroundColor = SystemColors.Window
            };

            _networkRulesGrid.Columns.Add(Strings.GridColumnType, Strings.GridColumnType);
            _networkRulesGrid.Columns.Add(Strings.GridColumnSSID, Strings.GridColumnSSID);
            _networkRulesGrid.Columns.Add(Strings.GridColumnBoostMode, Strings.GridColumnBoostMode);

            _networkRulesGrid.Columns[0].ReadOnly = true;
            _networkRulesGrid.Columns[1].ReadOnly = false;
            _networkRulesGrid.Columns[2].ReadOnly = false;
            
            _networkRulesGrid.Columns[1].FillWeight = 50;

            var typeColumn = new DataGridViewComboBoxColumn
            {
                Name = Strings.GridColumnType,
                HeaderText = Strings.GridColumnType,
                MinimumWidth = 100,
                FillWeight = 20
            };
            typeColumn.Items.AddRange(new object[] { Strings.NetworkTypeWiFi, Strings.NetworkTypeEthernet });
            _networkRulesGrid.Columns.RemoveAt(0);
            _networkRulesGrid.Columns.Insert(0, typeColumn);

            var boostModeColumn = new DataGridViewComboBoxColumn
            {
                Name = "BoostMode",
                HeaderText = Strings.GridColumnBoostMode,
                MinimumWidth = 100,
                FillWeight = 30
            };
            boostModeColumn.Items.AddRange(new object[] { Strings.BoostOn, Strings.BoostOff });
            _networkRulesGrid.Columns.RemoveAt(2);
            _networkRulesGrid.Columns.Insert(2, boostModeColumn);

            _networkRulesGrid.CellValueChanged += NetworkRulesGrid_CellValueChanged;

            // Rules Buttons
            var rulesButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };
            _addRuleButton = new Button
            {
                Text = Strings.ButtonAddRule,
                AutoSize = true,
                MinimumSize = new Size(100, 30),
                Margin = new Padding(0)
            };
            _addRuleButton.Click += AddRuleButton_Click;
            
            _removeRuleButton = new Button
            {
                Text = Strings.ButtonRemoveRule,
                AutoSize = true,
                MinimumSize = new Size(100, 30),
                Margin = new Padding(10, 0, 0, 0)
            };
            _removeRuleButton.Click += RemoveRuleButton_Click;
            
            rulesButtonsPanel.Controls.Add(_addRuleButton);
            rulesButtonsPanel.Controls.Add(_removeRuleButton);

            // OK/Cancel Buttons
            var okCancelPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0)
            };
            _cancelButton = new Button
            {
                Text = Strings.ButtonCancel,
                AutoSize = true,
                MinimumSize = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(10, 0, 0, 0)
            };
            _okButton = new Button
            {
                Text = Strings.ButtonOK,
                AutoSize = true,
                MinimumSize = new Size(80, 30),
                DialogResult = DialogResult.OK,
                Margin = new Padding(0)
            };
            _okButton.Click += OkButton_Click;
            
            okCancelPanel.Controls.Add(_cancelButton);
            okCancelPanel.Controls.Add(_okButton);

            mainPanel.Controls.Add(hotkeyOnLabel, 0, 0);
            mainPanel.Controls.Add(_hotkeyOnControl, 1, 0);
            
            mainPanel.Controls.Add(hotkeyOffLabel, 0, 1);
            mainPanel.Controls.Add(_hotkeyOffControl, 1, 1);
            
            mainPanel.Controls.Add(launchStateLabel, 0, 2);
            mainPanel.Controls.Add(_launchStateComboBox, 1, 2);

            mainPanel.Controls.Add(_autoLaunchCheckBox, 0, 3);
            mainPanel.SetColumnSpan(_autoLaunchCheckBox, 2);

            mainPanel.Controls.Add(networkRulesLabel, 0, 4);
            mainPanel.SetColumnSpan(networkRulesLabel, 2);

            mainPanel.Controls.Add(networkRulesInfoLabel, 0, 5);
            mainPanel.SetColumnSpan(networkRulesInfoLabel, 2);

            mainPanel.Controls.Add(_networkRulesGrid, 0, 6);
            mainPanel.SetColumnSpan(_networkRulesGrid, 2);

            mainPanel.Controls.Add(rulesButtonsPanel, 0, 7);
            mainPanel.SetColumnSpan(rulesButtonsPanel, 2);

            mainPanel.Controls.Add(okCancelPanel, 0, 8);
            mainPanel.SetColumnSpan(okCancelPanel, 2);

            this.Controls.Add(mainPanel);

            this.AcceptButton = _okButton;
            this.CancelButton = _cancelButton;
        }

        private void LoadSettings()
        {
            _hotkeyOnControl.Hotkey = _settings.HotkeyOn;
            _hotkeyOffControl.Hotkey = _settings.HotkeyOff;
            _launchStateComboBox.SelectedIndex = (int)_settings.LaunchState;
            _autoLaunchCheckBox.Checked = _settings.AutoLaunch;

            _networkRulesGrid.Rows.Clear();
            foreach (var rule in _settings.NetworkRules)
            {
                int rowIndex = _networkRulesGrid.Rows.Add();
                _networkRulesGrid.Rows[rowIndex].Cells[0].Value = rule.IsEthernet ? Strings.NetworkTypeEthernet : Strings.NetworkTypeWiFi;
                _networkRulesGrid.Rows[rowIndex].Cells[1].Value = rule.Ssid ?? "";
                _networkRulesGrid.Rows[rowIndex].Cells[2].Value = rule.BoostMode == BoostMode.Aggressive ? Strings.BoostOn : Strings.BoostOff;
            }
        }

        private void SaveSettings()
        {
            _settings.HotkeyOn = _hotkeyOnControl.Hotkey;
            _settings.HotkeyOff = _hotkeyOffControl.Hotkey;
            _settings.LaunchState = (LaunchState)_launchStateComboBox.SelectedIndex;
            _settings.AutoLaunch = _autoLaunchCheckBox.Checked;

            try
            {
                AutoLaunchManager.SetAutoLaunch(_settings.AutoLaunch);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set auto-launch: {ex.Message}", Strings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _settings.NetworkRules.Clear();
            foreach (DataGridViewRow row in _networkRulesGrid.Rows)
            {
                if (row.IsNewRow) continue;

                string type = row.Cells[0].Value?.ToString() ?? "";
                string ssid = row.Cells[1].Value?.ToString() ?? "";
                string boostMode = row.Cells[2].Value?.ToString() ?? "";

                var rule = new NetworkRule
                {
                    IsEthernet = type == Strings.NetworkTypeEthernet,
                    Ssid = type == Strings.NetworkTypeWiFi ? ssid : null,
                    BoostMode = boostMode == Strings.BoostOn ? BoostMode.Aggressive : BoostMode.Disabled
                };
                _settings.NetworkRules.Add(rule);
            }
        }

        private void AddRuleButton_Click(object? sender, EventArgs e)
        {
            int rowIndex = _networkRulesGrid.Rows.Add();
            _networkRulesGrid.Rows[rowIndex].Cells[0].Value = Strings.NetworkTypeWiFi;
            _networkRulesGrid.Rows[rowIndex].Cells[1].Value = "";
            _networkRulesGrid.Rows[rowIndex].Cells[2].Value = Strings.BoostOn;
        }

        private void RemoveRuleButton_Click(object? sender, EventArgs e)
        {
            if (_networkRulesGrid.SelectedRows.Count > 0)
            {
                _networkRulesGrid.Rows.RemoveAt(_networkRulesGrid.SelectedRows[0].Index);
            }
        }

        private void NetworkRulesGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            // If Type column changed to Ethernet, clear SSID
            if (e.ColumnIndex == 0 && e.RowIndex >= 0 && e.RowIndex < _networkRulesGrid.Rows.Count)
            {
                var row = _networkRulesGrid.Rows[e.RowIndex];
                if (row.Cells[0].Value?.ToString() == "Ethernet")
                {
                    row.Cells[1].Value = "";
                }
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            SaveSettings();
        }
    }
}
