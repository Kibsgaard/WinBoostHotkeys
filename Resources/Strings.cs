using System.Resources;

namespace WinBoostHotkeys.Resources
{
    public static class Strings
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("WinBoostHotkeys.Resources.Strings", typeof(Strings).Assembly);

        public static string AppName => ResourceManager.GetString("AppName") ?? "WinBoostHotkeys";
        public static string MenuToggleBoost => ResourceManager.GetString("MenuToggleBoost") ?? "Boost";
        public static string MenuSettings => ResourceManager.GetString("MenuSettings") ?? "Settings";
        public static string MenuExit => ResourceManager.GetString("MenuExit") ?? "Exit";
        public static string MenuBoost => ResourceManager.GetString("MenuBoost") ?? "Boost";
        public static string BoostOn => ResourceManager.GetString("BoostOn") ?? "On";
        public static string BoostOff => ResourceManager.GetString("BoostOff") ?? "Off";
        public static string ErrorBoostModeFailed => ResourceManager.GetString("ErrorBoostModeFailed") ?? "Failed to change boost mode. Please ensure the application is running with administrator privileges.";
        public static string ErrorTitle => ResourceManager.GetString("ErrorTitle") ?? "Error";
        public static string MessageAlreadyRunning => ResourceManager.GetString("MessageAlreadyRunning") ?? "WinBoostHotkeys is already running.";
        public static string SettingsTitle => ResourceManager.GetString("SettingsTitle") ?? "Settings";
        public static string LabelHotkeyBoostOn => ResourceManager.GetString("LabelHotkeyBoostOn") ?? "Hotkey (Boost On):";
        public static string LabelHotkeyBoostOff => ResourceManager.GetString("LabelHotkeyBoostOff") ?? "Hotkey (Boost Off):";
        public static string LabelLaunchState => ResourceManager.GetString("LabelLaunchState") ?? "Launch State:";
        public static string LabelNetworkRules => ResourceManager.GetString("LabelNetworkRules") ?? "Network Rules:";
        public static string LaunchStatePrevious => ResourceManager.GetString("LaunchStatePrevious") ?? "Previous";
        public static string GridColumnType => ResourceManager.GetString("GridColumnType") ?? "Type";
        public static string GridColumnSSID => ResourceManager.GetString("GridColumnSSID") ?? "SSID";
        public static string GridColumnBoostMode => ResourceManager.GetString("GridColumnBoostMode") ?? "Boost Mode";
        public static string NetworkTypeWiFi => ResourceManager.GetString("NetworkTypeWiFi") ?? "WiFi";
        public static string NetworkTypeEthernet => ResourceManager.GetString("NetworkTypeEthernet") ?? "Ethernet";
        public static string ButtonAddRule => ResourceManager.GetString("ButtonAddRule") ?? "Add Rule";
        public static string ButtonRemoveRule => ResourceManager.GetString("ButtonRemoveRule") ?? "Remove Rule";
        public static string ButtonOK => ResourceManager.GetString("ButtonOK") ?? "OK";
        public static string ButtonCancel => ResourceManager.GetString("ButtonCancel") ?? "Cancel";
        public static string HotkeyPressKeyCombination => ResourceManager.GetString("HotkeyPressKeyCombination") ?? "Press a key combination...";
        public static string HotkeyNone => ResourceManager.GetString("HotkeyNone") ?? "None";
    }
}
