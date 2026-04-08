# WinBoostHotkeys Project Summary

WinBoostHotkeys is a utility designed to keep laptop fans quiet by strictly controlling the Processor Performance Boost Mode.

Windows 11 often enables aggressive CPU boosting even for minor tasks like opening a browser tab or a Word document, causing cooling fans to spin up unnecessarily. This fan noise can be a significant nuisance, especially in quiet environments like classrooms, libraries, or meetings.

WinBoostHotkeys allows you to quickly toggle this boost mode on and off via global hotkeys or the system tray icon, ensuring silence when you need it and performance when you want it.

**Tray Icon Status:**
| Boost Disabled (Quiet) | Boost Enabled (Aggressive) |
| :---: | :---: |
| ![Boost Disabled](assets/tray_blue.png) | ![Boost Enabled](assets/tray_green.png) |

**Network Automation:**
The application can also automatically switch boost modes based on your network connection. You can define rules for specific WiFi networks or Ethernet connections (e.g., enable Boost when connected to power/docking station via Ethernet, disable when on school WiFi). The first matching rule from the top will be applied.

## Implementation Details

WinBoostHotkeys is a Windows tray application developed in .NET 10 C# to manage Processor Performance Boost Mode. It offers hotkey-based control and network-based automation.

### Key Features:

*   **Power Plan Management**: The `PowerPlanManager` class handles toggling Processor Performance Boost Mode (Aggressive/Disabled) using `powercfg.exe` commands.
*   **Tray Icon & UI**: A `NotifyIcon` provides a context menu for user interaction, including "Boost" (with state indication), "Settings", and "Exit" options.
*   **Settings & Persistence**: The `Settings` class defines configurable properties like hotkeys, launch state, and network rules, which are managed (loaded/saved to AppData) by the `SettingsManager` class. A `SettingsForm` provides the user interface for these settings.
*   **Global Hotkeys**: The `HotkeyManager` class utilizes the Win32 `RegisterHotKey` API to enable global hotkeys for toggling Boost Mode.
*   **Network Monitoring**: The `NetworkMonitor` class utilizes network information APIs to detect WiFi SSID and Ethernet connection changes, applying power boost settings based on predefined rules.
*   **Auto Launch**: The `AutoLaunchManager` class manages a Windows Task Scheduler task to automatically launch the application with administrative privileges on user login.

All core functionalities, settings, hotkey integration, network-based automation, and auto-launch capabilities have been successfully implemented.