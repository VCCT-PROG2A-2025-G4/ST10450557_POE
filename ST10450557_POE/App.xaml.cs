using System;
using System.Windows;
using System.IO;

namespace ST10450557_POE
{
    public partial class App : Application
    {
        private readonly ActivityLogger _logger = new ActivityLogger();

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                await _logger.LogActionAsync("Application startup initiated");
                MainWindow = new MainWindow();
                await ((MainWindow)MainWindow).InitializeAsync();
                MainWindow.Show();
                await _logger.LogActionAsync("MainWindow shown");
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Startup error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Console.WriteLine($"Startup error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Failed to start application: {ex.Message}\nStackTrace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}