using System.IO;
using System.Text.Json;

namespace StretchReminderApp.Core
{
    public class AppSettings
    {
        // Notification settings
        public int NotificationIntervalMinutes { get; set; } = 120; // Default: 2 hours

        // Application settings
        public bool StartWithWindows { get; set; } = false;

        // Stretching session settings
        public int RequiredStretchCount { get; set; } = 5; // Default: 5 stretches required
        public int MinimumSessionDuration { get; set; } = 30; // Default: minimum 30 seconds

        // UI settings
        public bool ShowMotivationalMessages { get; set; } = true;
        public bool PlaySoundOnCompletion { get; set; } = true;

        // Model settings
        public float PoseDetectionThreshold { get; set; } = 0.7f;

        // Path to settings file
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StretchReminderApp",
            "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);

                    return settings ?? new AppSettings();
                }
            }
            catch (Exception)
            {
                // If loading fails, return default settings
            }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directoryPath) && directoryPath != null)
                    Directory.CreateDirectory(directoryPath);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception)
            {
                // Handle saving error if needed
            }
        }

        public void ResetToDefaults()
        {
            NotificationIntervalMinutes = 120;
            StartWithWindows = false;
            RequiredStretchCount = 5;
            MinimumSessionDuration = 30;
            ShowMotivationalMessages = true;
            PlaySoundOnCompletion = true;
            PoseDetectionThreshold = 0.7f;

            Save();
        }
    }
}