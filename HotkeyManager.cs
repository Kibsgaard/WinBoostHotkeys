using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinBoostHotkeys
{
    public class HotkeyManager : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _windowHandle;
        private int _nextId = 1;
        private int _hotkeyOnId = -1;
        private int _hotkeyOffId = -1;

        public event EventHandler? HotkeyOnPressed;
        public event EventHandler? HotkeyOffPressed;

        public HotkeyManager(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public bool RegisterHotkeyOn(HotkeyConfig? hotkey)
        {
            if (_hotkeyOnId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyOnId);
                _hotkeyOnId = -1;
            }

            if (hotkey == null) return true;

            int modifiers = 0;
            if (hotkey.Ctrl) modifiers |= MOD_CONTROL;
            if (hotkey.Alt) modifiers |= MOD_ALT;
            if (hotkey.Shift) modifiers |= MOD_SHIFT;
            if (hotkey.Win) modifiers |= MOD_WIN;

            _hotkeyOnId = _nextId++;
            return RegisterHotKey(_windowHandle, _hotkeyOnId, modifiers, hotkey.KeyCode);
        }

        public bool RegisterHotkeyOff(HotkeyConfig? hotkey)
        {
            if (_hotkeyOffId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyOffId);
                _hotkeyOffId = -1;
            }

            if (hotkey == null) return true;

            int modifiers = 0;
            if (hotkey.Ctrl) modifiers |= MOD_CONTROL;
            if (hotkey.Alt) modifiers |= MOD_ALT;
            if (hotkey.Shift) modifiers |= MOD_SHIFT;
            if (hotkey.Win) modifiers |= MOD_WIN;

            _hotkeyOffId = _nextId++;
            return RegisterHotKey(_windowHandle, _hotkeyOffId, modifiers, hotkey.KeyCode);
        }

        public void ProcessMessage(Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == _hotkeyOnId)
                {
                    HotkeyOnPressed?.Invoke(this, EventArgs.Empty);
                }
                else if (id == _hotkeyOffId)
                {
                    HotkeyOffPressed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Dispose()
        {
            if (_hotkeyOnId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyOnId);
                _hotkeyOnId = -1;
            }

            if (_hotkeyOffId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyOffId);
                _hotkeyOffId = -1;
            }
        }
    }
}
