using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WinBoostHotkeys
{
    public enum LaunchState
    {
        Previous,
        On,
        Off
    }

    public class HotkeyConfig
    {
        public bool Ctrl { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public bool Win { get; set; }
        public int KeyCode { get; set; } // Virtual key code

        public HotkeyConfig()
        {
        }

        public HotkeyConfig(bool ctrl, bool alt, bool shift, bool win, int keyCode)
        {
            Ctrl = ctrl;
            Alt = alt;
            Shift = shift;
            Win = win;
            KeyCode = keyCode;
        }
    }

    public class NetworkRule
    {
        public string? Ssid { get; set; } // Null for Ethernet
        public bool IsEthernet { get; set; }
        public BoostMode BoostMode { get; set; }
    }

    public class Settings
    {
        public HotkeyConfig? HotkeyOn { get; set; }
        public HotkeyConfig? HotkeyOff { get; set; }
        public LaunchState LaunchState { get; set; } = LaunchState.Previous;
        public List<NetworkRule> NetworkRules { get; set; } = new();
        public BoostMode? PreviousBoostMode { get; set; } // For "Previous" launch state

        public static Settings GetDefault()
        {
            return new Settings
            {
                LaunchState = LaunchState.Previous,
                NetworkRules = new List<NetworkRule>()
            };
        }
    }
}
