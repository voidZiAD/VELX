using System;
using System.IO;
using System.Text.Json;

namespace VELX.Settings
{
    public class UserSettings
    {
        public string MicOption { get; set; } = "On"; // Default value
    }

    public static class SettingsManager
    {
        private static readonly string appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VELX", "settings");

        private static readonly string settingsFilePath = Path.Combine(appDataFolder, "options.json");

        public static UserSettings LoadSettings()
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                if (!File.Exists(settingsFilePath))
                {
                    var defaultSettings = new UserSettings();
                    SaveSettings(defaultSettings);
                    return defaultSettings;
                }

                string json = File.ReadAllText(settingsFilePath);
                return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
            }
            catch
            {
                return new UserSettings();
            }
        }

        public static void SaveSettings(UserSettings settings)
        {
            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }
    }
}
