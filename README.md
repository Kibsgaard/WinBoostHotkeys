# WinBoostHotkeys Project Summary

WinBoostHotkeys is a Windows tray application developed in .NET 10 C# to manage Processor Performance Boost Mode. It offers hotkey-based control and network-based automation.

## Key Features Implemented:

*   **Power Plan Management**: The `PowerPlanManager` class handles toggling Processor Performance Boost Mode (Aggressive/Disabled) using `powercfg.exe` commands.
*   **Tray Icon & UI**: A `NotifyIcon` provides a context menu for user interaction, including "Boost" (with state indication), "Settings", and "Exit" options.
*   **Settings & Persistence**: The `Settings` class defines configurable properties like hotkeys, launch state, and network rules, which are managed (loaded/saved to AppData) by the `SettingsManager` class. A `SettingsForm` provides the user interface for these settings.
*   **Global Hotkeys**: The `HotkeyManager` class utilizes the Win32 `RegisterHotKey` API to enable global hotkeys for toggling Boost Mode.
*   **Network Monitoring**: The `NetworkMonitor` class utilizes network information APIs to detect WiFi SSID and Ethernet connection changes, applying power boost settings based on predefined rules.

All core functionalities, settings, hotkey integration, and network-based automation, as outlined in the original plan, have been successfully implemented.