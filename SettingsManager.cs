using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WinBoostHotkeys
{
    public class SettingsManager
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WinBoostHotkeys"
        );

        private static readonly string SettingsFilePath = Path.Combine(
            SettingsDirectory,
            "settings.json"
        );

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static Settings Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    return Settings.GetDefault();
                }

                string json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<Settings>(json, JsonOptions);

                return settings ?? Settings.GetDefault();
            }
            catch
            {
                // Return defaults on error
                return Settings.GetDefault();
            }
        }

        public static bool Save(Settings settings)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(SettingsDirectory))
                {
                    Directory.CreateDirectory(SettingsDirectory);
                }

                string json = JsonSerializer.Serialize(settings, JsonOptions);
                File.WriteAllText(SettingsFilePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetSettingsPath() => SettingsFilePath;
    }
}
