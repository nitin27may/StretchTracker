using StretchReminderApp.UI;
using System.Windows;

namespace StretchReminderApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StretchNowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get app instance to access the database manager
                var app = (App)Application.Current;

                // Open the stretch window
                var stretchWindow = new StretchWindow(app.DatabaseManager);
                stretchWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting stretch session: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewStatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get app instance to access the database manager
                var app = (App)Application.Current;

                // Open the stats window
                var statsWindow = new StatsWindow(app.DatabaseManager);
                statsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening stats: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the settings window
                var settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening settings: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}