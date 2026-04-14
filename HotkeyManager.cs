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
        private int _hotkeyToggleId = -1;

        public event EventHandler? HotkeyOnPressed;
        public event EventHandler? HotkeyOffPressed;
        public event EventHandler? HotkeyTogglePressed;

        public HotkeyManager(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public bool RegisterHotkeys(HotkeyConfig? on, HotkeyConfig? off)
        {
            UnregisterAll();
            bool success = true;

            if (on != null && off != null && on.Equals(off))
            {
                _hotkeyToggleId = _nextId++;
                success &= RegisterHotKey(_windowHandle, _hotkeyToggleId, GetModifiers(on), on.KeyCode);
            }
            else
            {
                if (on != null)
                {
                    _hotkeyOnId = _nextId++;
                    success &= RegisterHotKey(_windowHandle, _hotkeyOnId, GetModifiers(on), on.KeyCode);
                }

                if (off != null)
                {
                    _hotkeyOffId = _nextId++;
                    success &= RegisterHotKey(_windowHandle, _hotkeyOffId, GetModifiers(off), off.KeyCode);
                }
            }
            return success;
        }

        private int GetModifiers(HotkeyConfig config)
        {
            int modifiers = 0;
            if (config.Ctrl) modifiers |= MOD_CONTROL;
            if (config.Alt) modifiers |= MOD_ALT;
            if (config.Shift) modifiers |= MOD_SHIFT;
            if (config.Win) modifiers |= MOD_WIN;
            return modifiers;
        }

        public void UnregisterAll()
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
            if (_hotkeyToggleId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyToggleId);
                _hotkeyToggleId = -1;
            }
        }

        public bool RegisterHotkeyOn(HotkeyConfig? hotkey)
        {
            if (_hotkeyOnId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyOnId);
                _hotkeyOnId = -1;
            }

            if (hotkey == null) return true;

            _hotkeyOnId = _nextId++;
            return RegisterHotKey(_windowHandle, _hotkeyOnId, GetModifiers(hotkey), hotkey.KeyCode);
        }

        public bool RegisterHotkeyOff(HotkeyConfig? hotkey)
        {
            if (_hotkeyOffId != -1)
            {
                UnregisterHotKey(_windowHandle, _hotkeyOffId);
                _hotkeyOffId = -1;
            }

            if (hotkey == null) return true;

            _hotkeyOffId = _nextId++;
            return RegisterHotKey(_windowHandle, _hotkeyOffId, GetModifiers(hotkey), hotkey.KeyCode);
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
                else if (id == _hotkeyToggleId)
                {
                    HotkeyTogglePressed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Dispose()
        {
            UnregisterAll();
        }
    }
}
