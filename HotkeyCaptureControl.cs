using System;
using System.Windows.Forms;
using WinBoostHotkeys.Resources;

namespace WinBoostHotkeys
{
    public class HotkeyCaptureControl : Control
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
                Invalidate();
            }
        }

        public HotkeyCaptureControl()
        {
            SetStyle(ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.UserPaint, true);
            Cursor = Cursors.Hand;
            BackColor = System.Drawing.SystemColors.Window;
            Size = new System.Drawing.Size(120, 23);
        }

        public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
        {
            return new System.Drawing.Size(120, 23);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            StartCapture();
        }

        private void StartCapture()
        {
            _capturing = true;
            Focus();
            Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!_capturing) 
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Extract main key
            Keys keyCode = keyData & Keys.KeyCode;

            // Ignore modifier keys alone
            if (keyCode == Keys.ControlKey || keyCode == Keys.ShiftKey ||
                keyCode == Keys.Menu || keyCode == Keys.LWin || keyCode == Keys.RWin || keyCode == Keys.None)
            {
                return true; // Consume event while capturing
            }

            // Detect modifiers
            bool ctrl = (keyData & Keys.Control) == Keys.Control;
            bool alt = (keyData & Keys.Alt) == Keys.Alt;
            bool shift = (keyData & Keys.Shift) == Keys.Shift;
            bool win = (Control.ModifierKeys & Keys.LWin) != 0 || (Control.ModifierKeys & Keys.RWin) != 0;

            if (!ctrl && !alt && !shift && !win)
            {
                return true; // Require modifiers, consume event
            }

            // Create hotkey config
            _currentHotkey = new HotkeyConfig(ctrl, alt, shift, win, (int)keyCode);
            StopCapture();
            return true; // Event handled
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            StopCapture();
        }

        private void StopCapture()
        {
            if (!_capturing) return;
            _capturing = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            var rect = ClientRectangle;

            // Draw background
            using (var brush = new System.Drawing.SolidBrush(_capturing ? System.Drawing.Color.LightYellow : BackColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw border
            using (var pen = new System.Drawing.SolidBrush(Focused ? System.Drawing.SystemColors.Highlight : System.Drawing.SystemColors.ControlDark))
            {
                var borderRect = rect;
                borderRect.Width -= 1;
                borderRect.Height -= 1;
                g.DrawRectangle(new System.Drawing.Pen(pen), borderRect);
            }

            // Get text to display
            string textToShow;
            if (_capturing)
            {
                textToShow = Strings.HotkeyPressKeyCombination;
            }
            else if (_currentHotkey == null)
            {
                textToShow = Strings.HotkeyNone;
            }
            else
            {
                var parts = new System.Collections.Generic.List<string>();
                if (_currentHotkey.Ctrl) parts.Add("Ctrl");
                if (_currentHotkey.Alt) parts.Add("Alt");
                if (_currentHotkey.Shift) parts.Add("Shift");
                if (_currentHotkey.Win) parts.Add("Win");
                parts.Add(((Keys)_currentHotkey.KeyCode).ToString());
                textToShow = string.Join(" + ", parts);
            }

            // Draw text
            TextRenderer.DrawText(g, textToShow, Font, rect, System.Drawing.SystemColors.ControlText, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
