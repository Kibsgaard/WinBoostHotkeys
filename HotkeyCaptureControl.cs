using System;
using System.Windows.Forms;
using WinBoostHotkeys.Resources;

namespace WinBoostHotkeys
{
    public class HotkeyCaptureControl : TextBox
    {
        private bool _capturing = false;
        private HotkeyConfig? _currentHotkey;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public HotkeyConfig? Hotkey
        {
            get => _currentHotkey;
            set
            {
                _currentHotkey = value;
                UpdateText();
            }
        }

        public HotkeyCaptureControl()
        {
            ReadOnly = true;
            Cursor = Cursors.Hand;
            Click += HotkeyCaptureControl_Click;
            KeyDown += HotkeyCaptureControl_KeyDown;
            KeyUp += HotkeyCaptureControl_KeyUp;
            LostFocus += HotkeyCaptureControl_LostFocus;
        }

        private void HotkeyCaptureControl_Click(object? sender, EventArgs e)
        {
            StartCapture();
        }

        private void StartCapture()
        {
            _capturing = true;
            Text = Strings.HotkeyPressKeyCombination;
            BackColor = System.Drawing.Color.LightYellow;
            Focus();
        }

        private void HotkeyCaptureControl_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!_capturing) return;

            // Ignore modifier keys alone
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            // Get modifier states
            bool ctrl = e.Control;
            bool alt = e.Alt;
            bool shift = e.Shift;
            bool win = (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin) ||
                       ((Control.ModifierKeys & (Keys.LWin | Keys.RWin)) != 0);

            // Require at least one modifier
            if (!ctrl && !alt && !shift && !win)
            {
                return;
            }

            // Create hotkey config
            _currentHotkey = new HotkeyConfig(ctrl, alt, shift, win, (int)e.KeyCode);
            UpdateText();
            StopCapture();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void HotkeyCaptureControl_KeyUp(object? sender, KeyEventArgs e)
        {
            if (!_capturing) return;
            e.Handled = true;
        }

        private void HotkeyCaptureControl_LostFocus(object? sender, EventArgs e)
        {
            StopCapture();
        }

        private void StopCapture()
        {
            _capturing = false;
            BackColor = System.Drawing.SystemColors.Window;
        }

        private void UpdateText()
        {
            if (_currentHotkey == null)
            {
                Text = Strings.HotkeyNone;
                return;
            }

            var parts = new System.Collections.Generic.List<string>();
            if (_currentHotkey.Ctrl) parts.Add("Ctrl");
            if (_currentHotkey.Alt) parts.Add("Alt");
            if (_currentHotkey.Shift) parts.Add("Shift");
            if (_currentHotkey.Win) parts.Add("Win");

            string keyName = ((Keys)_currentHotkey.KeyCode).ToString();
            parts.Add(keyName);

            Text = string.Join(" + ", parts);
        }
    }
}
