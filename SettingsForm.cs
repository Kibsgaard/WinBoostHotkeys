using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinBoostHotkeys
{
    public partial class SettingsForm : Form
    {
        private Settings _settings;
        private HotkeyCaptureControl _hotkeyOnControl = null!;
        private HotkeyCaptureControl _hotkeyOffControl = null!;
        private ComboBox _launchStateComboBox = null!;
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
            this.Text = "Settings";
            this.Size = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            int yPos = 20;
            int labelWidth = 150;
            int controlX = 180;
            int controlWidth = 300;

            // Hotkey On
            var hotkeyOnLabel = new Label
            {
                Text = "Hotkey (Boost On):",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 23)
            };
            this.Controls.Add(hotkeyOnLabel);

            _hotkeyOnControl = new HotkeyCaptureControl
            {
                Location = new Point(controlX, yPos),
                Size = new Size(controlWidth, 23)
            };
            this.Controls.Add(_hotkeyOnControl);

            yPos += 35;

            // Hotkey Off
            var hotkeyOffLabel = new Label
            {
                Text = "Hotkey (Boost Off):",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 23)
            };
            this.Controls.Add(hotkeyOffLabel);

            _hotkeyOffControl = new HotkeyCaptureControl
            {
                Location = new Point(controlX, yPos),
                Size = new Size(controlWidth, 23)
            };
            this.Controls.Add(_hotkeyOffControl);

            yPos += 35;

            // Launch State
            var launchStateLabel = new Label
            {
                Text = "Launch State:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 23)
            };
            this.Controls.Add(launchStateLabel);

            _launchStateComboBox = new ComboBox
            {
                Location = new Point(controlX, yPos),
                Size = new Size(controlWidth, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _launchStateComboBox.Items.AddRange(new[] { "Previous", "On", "Off" });
            this.Controls.Add(_launchStateComboBox);

            yPos += 40;

            // Network Rules
            var networkRulesLabel = new Label
            {
                Text = "Network Rules:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 23)
            };
            this.Controls.Add(networkRulesLabel);

            yPos += 25;

            _networkRulesGrid = new DataGridView
            {
                Location = new Point(20, yPos),
                Size = new Size(540, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            _networkRulesGrid.Columns.Add("Type", "Type");
            _networkRulesGrid.Columns.Add("SSID", "SSID");
            _networkRulesGrid.Columns.Add("Boost Mode", "Boost Mode");

            _networkRulesGrid.Columns[0].Width = 100;
            _networkRulesGrid.Columns[1].Width = 200;
            _networkRulesGrid.Columns[2].Width = 150;

            _networkRulesGrid.Columns[0].ReadOnly = true;
            _networkRulesGrid.Columns[1].ReadOnly = false;
            _networkRulesGrid.Columns[2].ReadOnly = false;

            // Make Type column a combo box
            var typeColumn = new DataGridViewComboBoxColumn
            {
                Name = "Type",
                HeaderText = "Type",
                Width = 100
            };
            typeColumn.Items.AddRange("WiFi", "Ethernet");
            _networkRulesGrid.Columns.RemoveAt(0);
            _networkRulesGrid.Columns.Insert(0, typeColumn);

            // Make Boost Mode column a combo box
            var boostModeColumn = new DataGridViewComboBoxColumn
            {
                Name = "BoostMode",
                HeaderText = "Boost Mode",
                Width = 150
            };
            boostModeColumn.Items.AddRange("On", "Off");
            _networkRulesGrid.Columns.RemoveAt(2);
            _networkRulesGrid.Columns.Insert(2, boostModeColumn);

            // Handle Type change to clear SSID for Ethernet
            _networkRulesGrid.CellValueChanged += NetworkRulesGrid_CellValueChanged;

            this.Controls.Add(_networkRulesGrid);

            yPos += 210;

            // Buttons for network rules
            _addRuleButton = new Button
            {
                Text = "Add Rule",
                Location = new Point(20, yPos),
                Size = new Size(100, 30)
            };
            _addRuleButton.Click += AddRuleButton_Click;
            this.Controls.Add(_addRuleButton);

            _removeRuleButton = new Button
            {
                Text = "Remove Rule",
                Location = new Point(130, yPos),
                Size = new Size(100, 30)
            };
            _removeRuleButton.Click += RemoveRuleButton_Click;
            this.Controls.Add(_removeRuleButton);

            yPos += 50;

            // OK/Cancel buttons
            _okButton = new Button
            {
                Text = "OK",
                Location = new Point(400, yPos),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };
            _okButton.Click += OkButton_Click;
            this.Controls.Add(_okButton);

            _cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(485, yPos),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(_cancelButton);

            this.AcceptButton = _okButton;
            this.CancelButton = _cancelButton;
        }

        private void LoadSettings()
        {
            _hotkeyOnControl.Hotkey = _settings.HotkeyOn;
            _hotkeyOffControl.Hotkey = _settings.HotkeyOff;
            _launchStateComboBox.SelectedIndex = (int)_settings.LaunchState;

            _networkRulesGrid.Rows.Clear();
            foreach (var rule in _settings.NetworkRules)
            {
                int rowIndex = _networkRulesGrid.Rows.Add();
                _networkRulesGrid.Rows[rowIndex].Cells[0].Value = rule.IsEthernet ? "Ethernet" : "WiFi";
                _networkRulesGrid.Rows[rowIndex].Cells[1].Value = rule.Ssid ?? "";
                _networkRulesGrid.Rows[rowIndex].Cells[2].Value = rule.BoostMode == BoostMode.Aggressive ? "On" : "Off";
            }
        }

        private void SaveSettings()
        {
            _settings.HotkeyOn = _hotkeyOnControl.Hotkey;
            _settings.HotkeyOff = _hotkeyOffControl.Hotkey;
            _settings.LaunchState = (LaunchState)_launchStateComboBox.SelectedIndex;

            _settings.NetworkRules.Clear();
            foreach (DataGridViewRow row in _networkRulesGrid.Rows)
            {
                if (row.IsNewRow) continue;

                string type = row.Cells[0].Value?.ToString() ?? "";
                string ssid = row.Cells[1].Value?.ToString() ?? "";
                string boostMode = row.Cells[2].Value?.ToString() ?? "";

                var rule = new NetworkRule
                {
                    IsEthernet = type == "Ethernet",
                    Ssid = type == "WiFi" ? ssid : null,
                    BoostMode = boostMode == "On" ? BoostMode.Aggressive : BoostMode.Disabled
                };
                _settings.NetworkRules.Add(rule);
            }
        }

        private void AddRuleButton_Click(object? sender, EventArgs e)
        {
            int rowIndex = _networkRulesGrid.Rows.Add();
            _networkRulesGrid.Rows[rowIndex].Cells[0].Value = "WiFi";
            _networkRulesGrid.Rows[rowIndex].Cells[1].Value = "";
            _networkRulesGrid.Rows[rowIndex].Cells[2].Value = "On";
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
