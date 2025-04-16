using StretchReminderApp.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace StretchReminderApp.Core
{
    public class NotificationManager
    {
        private DispatcherTimer _notificationTimer;
        private readonly DatabaseManager _dbManager;
        private readonly AppSettings _settings;
        private Window _notificationWindow;

        public NotificationManager(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            _settings = AppSettings.Load();
        }

        public void StartNotificationTimer()
        {
            try
            {
                // Use DispatcherTimer instead of System.Timers.Timer for WPF
                // This runs on the UI thread and avoids cross-thread issues
                _notificationTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMinutes(_settings.NotificationIntervalMinutes)
                };

                _notificationTimer.Tick += (s, e) => ShowNotification();
                _notificationTimer.Start();

                // Show first notification after a short delay
                DispatcherTimer startupTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(10)
                };

                startupTimer.Tick += (s, e) =>
                {
                    ShowNotification();
                    startupTimer.Stop(); // One-time timer
                };

                startupTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting notification: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowNotification()
        {
            try
            {
                // If previous notification is still open, close it
                if (_notificationWindow != null && _notificationWindow.IsVisible)
                {
                    _notificationWindow.Close();
                    _notificationWindow = null;
                }

                // Get current streak from database
                int currentStreak = _dbManager.GetCurrentStreak();

                // Create simple notification window
                _notificationWindow = new Window
                {
                    Title = "Time to Stretch!",
                    Width = 350,
                    Height = 200,
                    WindowStyle = WindowStyle.ToolWindow,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true,
                    ShowInTaskbar = true
                };

                // Create the content
                Grid mainGrid = new Grid();

                // Create a stack panel for the content
                StackPanel contentPanel = new StackPanel
                {
                    Margin = new Thickness(20)
                };

                // Title
                TextBlock titleText = new TextBlock
                {
                    Text = "Time to stretch!",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                // Message
                TextBlock messageText = new TextBlock
                {
                    Text = $"Maintain your {currentStreak}-day streak by completing today's stretch.",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 20)
                };

                // Buttons
                StackPanel buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                Button skipButton = new Button
                {
                    Content = "Skip",
                    Padding = new Thickness(15, 5, 15, 5),
                    Margin = new Thickness(0, 0, 10, 0)
                };

                Button startButton = new Button
                {
                    Content = "Start Stretching",
                    Padding = new Thickness(15, 5, 15, 5),
                    Background = new SolidColorBrush(Color.FromRgb(46, 125, 50)),
                    Foreground = new SolidColorBrush(Colors.White)
                };

                // Button event handlers
                skipButton.Click += (s, e) => _notificationWindow.Close();

                startButton.Click += (s, e) =>
                {
                    try
                    {
                        var stretchWindow = new StretchWindow(_dbManager);
                        stretchWindow.Show();
                        _notificationWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening stretch window: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        _notificationWindow.Close();
                    }
                };

                // Assemble the UI
                buttonPanel.Children.Add(skipButton);
                buttonPanel.Children.Add(startButton);

                contentPanel.Children.Add(titleText);
                contentPanel.Children.Add(messageText);
                contentPanel.Children.Add(buttonPanel);

                mainGrid.Children.Add(contentPanel);
                _notificationWindow.Content = mainGrid;

                // Play a sound if enabled
                if (_settings.PlaySoundOnCompletion)
                {
                    try
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    catch
                    {
                        // Ignore sound errors - not critical
                    }
                }

                // Show the window
                _notificationWindow.Show();

                // Auto-close after 60 seconds
                DispatcherTimer closeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(60)
                };

                closeTimer.Tick += (s, e) =>
                {
                    if (_notificationWindow != null && _notificationWindow.IsVisible)
                    {
                        _notificationWindow.Close();
                        _notificationWindow = null;
                    }
                    closeTimer.Stop();
                };

                closeTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing notification: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateNotificationSchedule(int intervalMinutes)
        {
            try
            {
                if (_notificationTimer != null)
                {
                    _notificationTimer.Stop();
                    _notificationTimer.Interval = TimeSpan.FromMinutes(intervalMinutes);
                    _notificationTimer.Start();
                }

                _settings.NotificationIntervalMinutes = intervalMinutes;
                _settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating schedule: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CleanUp()
        {
            try
            {
                // Stop timer
                if (_notificationTimer != null)
                {
                    _notificationTimer.Stop();
                    _notificationTimer = null;
                }

                // Close any open notification windows
                if (_notificationWindow != null && _notificationWindow.IsVisible)
                {
                    _notificationWindow.Close();
                    _notificationWindow = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during cleanup: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}