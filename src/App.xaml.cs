using StretchReminderApp.Core;
using StretchReminderApp.UI;
using System.Windows;

namespace StretchReminderApp
{
    public partial class App : Application
    {
        public NotificationManager NotificationManager { get; private set; }
        public DatabaseManager DatabaseManager { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                // Initialize database
                DatabaseManager = new DatabaseManager();
                DatabaseManager.InitializeDatabase();

                // Setup notification system
                NotificationManager = new NotificationManager(DatabaseManager);

                // Create a main window instead of using system tray icon
                MainWindow = new MainWindow();
                MainWindow.Show();

                // Start notification timer
                NotificationManager.StartNotificationTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application: {ex.Message}\n\n{ex.StackTrace}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        // Menu click handlers for the taskbar context menu
        private void StatsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Open stats window
            var statsWindow = new StatsWindow(DatabaseManager);
            statsWindow.Show();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Open settings window
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Exit the application
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Clean up notification system
                NotificationManager?.CleanUp();
            }
            catch
            {
                // Ignore errors during shutdown
            }
            base.OnExit(e);
        }
    }
}