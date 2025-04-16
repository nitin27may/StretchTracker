using Microsoft.Win32;
using StretchReminderApp.Core;
using System.Windows;
using System.Windows.Controls;

namespace StretchReminderApp.UI
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;

        public SettingsWindow()
        {
            InitializeComponent();

            // Load current settings
            _settings = AppSettings.Load();

            // Initialize UI with current settings
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Initialize reminder interval controls
            DaysTextBox.Text = _settings.IntervalDays.ToString();
            HoursTextBox.Text = _settings.IntervalHours.ToString();
            MinutesTextBox.Text = _settings.IntervalMinutes.ToString();

            // Initialize dev mode controls
            DevModeCheckbox.IsChecked = _settings.EnableDevMode;
            DevModeMinutesTextBox.Text = _settings.DevModeIntervalMinutes.ToString();
            DevModeMinutesTextBox.IsEnabled = _settings.EnableDevMode;

            // Other settings
            StartWithWindowsCheckbox.IsChecked = _settings.StartWithWindows;
            SoundNotificationCheckbox.IsChecked = _settings.PlaySoundOnCompletion;
            StretchCountSlider.Value = _settings.RequiredStretchCount;
            ThresholdSlider.Value = _settings.PoseDetectionThreshold;
            MotivationCheckbox.IsChecked = _settings.ShowMotivationalMessages;

            // Update text displays
            UpdateStretchCountText();
            UpdateThresholdText();

            // Update effective interval text
            UpdateEffectiveIntervalText();
        }

        private void UpdateStretchCountText()
        {
            int count = (int)StretchCountSlider.Value;
            StretchCountText.Text = $"{count} stretches";
        }

        private void UpdateThresholdText()
        {
            double threshold = ThresholdSlider.Value;
            string description;

            if (threshold < 0.6)
                description = "Low (more detections)";
            else if (threshold < 0.75)
                description = "Medium";
            else
                description = "High (fewer detections)";

            ThresholdText.Text = description;
        }

        private void UpdateEffectiveIntervalText()
        {
            try
            {
                // Parse interval components
                if (!int.TryParse(DaysTextBox.Text, out int days))
                    days = 0;

                if (!int.TryParse(HoursTextBox.Text, out int hours))
                    hours = 0;

                if (!int.TryParse(MinutesTextBox.Text, out int minutes))
                    minutes = 0;

                // Parse dev mode interval
                if (!int.TryParse(DevModeMinutesTextBox.Text, out int devMinutes))
                    devMinutes = 5;

                // Calculate total minutes
                int totalMinutes = (days * 24 * 60) + (hours * 60) + minutes;

                // Ensure at least 1 minute
                if (totalMinutes < 1)
                    totalMinutes = 1;

                // Format description
                string description;

                if (DevModeCheckbox.IsChecked == true)
                {
                    description = $"{devMinutes} minutes (Development Mode)";
                    EffectiveIntervalText.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Colors.Blue);
                }
                else
                {
                    var parts = new System.Collections.Generic.List<string>();

                    if (days > 0)
                        parts.Add($"{days} day{(days > 1 ? "s" : "")}");

                    if (hours > 0)
                        parts.Add($"{hours} hour{(hours > 1 ? "s" : "")}");

                    if (minutes > 0)
                        parts.Add($"{minutes} minute{(minutes > 1 ? "s" : "")}");

                    description = parts.Count > 0 ? string.Join(", ", parts) : "1 minute";
                    EffectiveIntervalText.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Colors.Black);
                }

                EffectiveIntervalText.Text = $"Reminder every: {description}";
            }
            catch (Exception)
            {
                EffectiveIntervalText.Text = "Invalid reminder interval";
            }
        }

        private void StretchCountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateStretchCountText();
        }

        private void ThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateThresholdText();
        }

        private void IntervalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEffectiveIntervalText();
        }

        private void DevModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            DevModeMinutesTextBox.IsEnabled = DevModeCheckbox.IsChecked == true;
            UpdateEffectiveIntervalText();
        }

        private void DevModeMinutesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEffectiveIntervalText();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "TensorFlow Models (*.pb)|*.pb|All files (*.*)|*.*",
                InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models")
            };

            if (dialog.ShowDialog() == true)
            {
                ModelPathTextBox.Text = dialog.FileName;
            }
        }

        private void TestNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get app instance
                var app = (App)Application.Current;

                // Trigger a notification
                app.NotificationManager.ShowNotification();

                MessageBox.Show("Notification test sent successfully.",
                    "Test Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send test notification: {ex.Message}",
                    "Test Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Confirm reset
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to reset all settings to defaults?",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Reset settings
                _settings.ResetToDefaults();

                // Update UI
                InitializeUI();

                MessageBox.Show("Settings have been reset to defaults.",
                    "Reset Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update settings from UI
            try
            {
                // Parse interval components
                if (int.TryParse(DaysTextBox.Text, out int days))
                    _settings.IntervalDays = Math.Max(0, days);
                else
                    _settings.IntervalDays = 0;

                if (int.TryParse(HoursTextBox.Text, out int hours))
                    _settings.IntervalHours = Math.Max(0, hours);
                else
                    _settings.IntervalHours = 0;

                if (int.TryParse(MinutesTextBox.Text, out int minutes))
                    _settings.IntervalMinutes = Math.Max(0, minutes);
                else
                    _settings.IntervalMinutes = 0;

                // Update total minutes
                _settings.UpdateTotalMinutes();

                // Ensure at least 1 minute interval
                if (_settings.NotificationIntervalMinutes < 1)
                {
                    _settings.IntervalMinutes = 1;
                    _settings.NotificationIntervalMinutes = 1;
                }

                // Dev mode settings
                _settings.EnableDevMode = DevModeCheckbox.IsChecked == true;

                if (int.TryParse(DevModeMinutesTextBox.Text, out int devMinutes))
                    _settings.DevModeIntervalMinutes = Math.Max(1, devMinutes);
                else
                    _settings.DevModeIntervalMinutes = 5;

                // Other settings
                _settings.StartWithWindows = StartWithWindowsCheckbox.IsChecked ?? false;
                _settings.PlaySoundOnCompletion = SoundNotificationCheckbox.IsChecked ?? true;
                _settings.RequiredStretchCount = (int)StretchCountSlider.Value;
                _settings.PoseDetectionThreshold = (float)ThresholdSlider.Value;
                _settings.ShowMotivationalMessages = MotivationCheckbox.IsChecked ?? true;

                // Save settings to file
                _settings.Save();

                // Update notification interval in the app
                var app = (App)Application.Current;
                app.NotificationManager.UpdateNotificationSchedule();

                // Update startup registry
                if (_settings.StartWithWindows)
                    AddApplicationToStartup();
                else
                    RemoveApplicationFromStartup();

                // Show confirmation and close
                MessageBox.Show("Settings saved successfully!", "Settings Saved",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close without saving
            DialogResult = false;
            Close();
        }

        private void AddApplicationToStartup()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (key != null)
                    {
                        key.SetValue("StretchReminderApp",
                            System.Reflection.Assembly.GetExecutingAssembly().Location);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add to startup: {ex.Message}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveApplicationFromStartup()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (key != null && key.GetValue("StretchReminderApp") != null)
                        key.DeleteValue("StretchReminderApp");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove from startup: {ex.Message}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Input validation methods
        private void NumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Only allow digits
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}