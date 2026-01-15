using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WinBoostHotkeys
{
    public enum BoostMode
    {
        Disabled = 0,
        Aggressive = 2
    }

    public class PowerPlanManager
    {
        // GUIDs for processor performance boost mode
        private const string ProcessorBoostSubGuid = "54533251-82be-4824-96c1-47b60b740d00";
        private const string ProcessorBoostSettingGuid = "be337238-0d82-4146-a960-4f3749d470c7";
        private static readonly Regex GuidPattern = new(@"([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BoostModeValuePattern = new(@"Current AC Power Setting Index:\s*0x([0-9a-f]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets the current active power scheme GUID
        /// </summary>
        private string? GetActiveSchemeGuid()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "/getactivescheme",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processStartInfo);
                if (process == null) return null;

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Extract GUID from output: "Power Scheme GUID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx  (Balanced)"
                var match = GuidPattern.Match(output);
                return match.Success ? match.Groups[1].Value : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the current processor performance boost mode
        /// </summary>
        public BoostMode? GetCurrentBoostMode()
        {
            try
            {
                string? schemeGuid = GetActiveSchemeGuid();
                if (string.IsNullOrEmpty(schemeGuid)) return null;

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = $"/query {schemeGuid} {ProcessorBoostSubGuid} {ProcessorBoostSettingGuid}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processStartInfo);
                if (process == null) return null;

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Extract value from output: "Current AC Power Setting Index: 0x00000002"
                var match = BoostModeValuePattern.Match(output);
                if (match.Success)
                {
                    int value = Convert.ToInt32(match.Groups[1].Value, 16);
                    return (BoostMode)value;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the processor performance boost mode
        /// </summary>
        public bool SetBoostMode(BoostMode mode)
        {
            try
            {
                string? schemeGuid = GetActiveSchemeGuid();
                if (string.IsNullOrEmpty(schemeGuid)) return false;

                int value = (int)mode;

                // Set AC power setting
                var acProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = $"/setacvalueindex {schemeGuid} {ProcessorBoostSubGuid} {ProcessorBoostSettingGuid} {value}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var acProcess = Process.Start(acProcessStartInfo);
                if (acProcess == null) return false;
                acProcess.WaitForExit();
                if (acProcess.ExitCode != 0) return false;

                // Set DC power setting
                var dcProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = $"/setdcvalueindex {schemeGuid} {ProcessorBoostSubGuid} {ProcessorBoostSettingGuid} {value}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var dcProcess = Process.Start(dcProcessStartInfo);
                if (dcProcess == null) return false;
                dcProcess.WaitForExit();
                if (dcProcess.ExitCode != 0) return false;

                // Activate the scheme
                var activateProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = $"/setactive {schemeGuid}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var activateProcess = Process.Start(activateProcessStartInfo);
                if (activateProcess == null) return false;
                activateProcess.WaitForExit();

                return activateProcess.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
