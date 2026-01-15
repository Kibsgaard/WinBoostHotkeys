# WinBoostHotkeys Implementation Plan

## Overview
Windows tray application (.NET 10 C#) to toggle Processor Performance Boost Mode with hotkeys and network-based automation.

## Architecture
- **Framework**: .NET 10, Windows Forms (NotifyIcon for tray)
- **Power Management**: `powercfg.exe` CLI or Win32 Power Management APIs
- **Settings Storage**: JSON file in AppData
- **Hotkeys**: Global hotkey registration via Win32 RegisterHotKey API
- **Network Detection**: System.Net.NetworkInformation for WiFi SSID and Ethernet detection
- **Localization**: .NET resource files (.resx) for multi-language support

## Implementation Tasks

### Phase 1: Core Functionality
1. **Project Setup**
   - Create .NET 10 Windows Forms application
   - Configure for single-instance (mutex)
   - Add required NuGet packages
   - Set up resource files (.resx) for localization (strings externalized from start)

2. **Power Plan Management**
   - Implement `PowerPlanManager` class
   - Methods: GetCurrentBoostMode(), SetBoostMode(Aggressive/Disabled)
   - Use `powercfg.exe /setacvalueindex`, `/setdcvalueindex` and `/setactive` commands
   - Handle GUID retrieval for current power plan

3. **Tray Icon & UI**
   - Create `NotifyIcon` with context menu
   - Menu items: "Boost (with state checkmark)", "Settings", "Exit"
   - Icon state management (green=ON, blue=OFF)
   - Update icon on state change

### Phase 2: Settings & Persistence
4. **Settings Model & Storage**
   - Create `Settings` class (JSON serializable)
   - Properties: HotkeyOn, HotkeyOff, LaunchState, NetworkRules
   - Implement `SettingsManager` for load/save to AppData

5. **Settings UI**
   - Create Settings form
   - Hotkey capture controls (key combination picker)
   - Launch state dropdown (Previous/On/Off)
   - Network rules table (SSID/Type → State)

### Phase 3: Hotkeys
6. **Global Hotkey Registration**
   - Implement `HotkeyManager` using RegisterHotKey Win32 API
   - Handle WM_HOTKEY messages in message loop
   - Map hotkeys to Boost On/Off actions
   - Load hotkeys from settings on startup

### Phase 4: Network Detection
7. **Network Monitoring**
   - Implement `NetworkMonitor` class
   - Detect WiFi SSID changes (NetworkInformation APIs)
   - Detect Ethernet connection changes
   - Match against settings rules and apply state

8. **Launch & Network Logic**
   - On startup: Apply launch state (Previous/On/Off)
   - Monitor network changes and apply matching rules
   - Persist "Previous" state on state change

## Technical Details

### Power Plan Commands
```bash
# Get current plan GUID
powercfg /getactivescheme

# Set boost mode (GUID = current plan, sub_GUID = processor boost)
powercfg /setacvalueindex <SCHEME_GUID> <SUB_GUID> <SETTING_GUID> <VALUE>
powercfg /setdcvalueindex <SCHEME_GUID> <SUB_GUID> <SETTING_GUID> <VALUE>
powercfg /setactive <SCHEME_GUID>
```

### Boost Mode Values
- Aggressive: 2
- Disabled: 0

### Dependencies
- System.Windows.Forms
- System.Text.Json (or Newtonsoft.Json)
- Win32 interop for hotkeys
- System.Resources (built-in, for localization)

## File Structure
```
WinBoostHotkeys/
├── Program.cs
├── MainForm.cs (hidden form for message loop)
├── PowerPlanManager.cs
├── HotkeyManager.cs
├── NetworkMonitor.cs
├── Settings.cs
├── SettingsManager.cs
├── SettingsForm.cs
├── Resources/
│   ├── Strings.resx (default/English)
│   ├── Strings.{locale}.resx (other languages)
│   └── Icons/ (tray icons)
```

## Priority Order
1. Phase 1 (Core) - Essential functionality
2. Phase 2 (Settings) - User configuration
3. Phase 3 (Hotkeys) - Quick access
4. Phase 4 (Network) - Automation feature
