using System;
using System.IO;
using System.Threading.Tasks;

namespace ST10450557_POE
{
    public class ActivityLogger
    {
        private readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "activity_log.txt");

        public async Task LogActionAsync(string action)
        {
            try
            {
                string logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} SAST - {action}{Environment.NewLine}";
                await File.AppendAllTextAsync(_logFilePath, logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging action: {ex.Message}");
            }
        }
    }
}