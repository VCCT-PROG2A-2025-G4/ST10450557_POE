using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace ST10450557_POE
{
    public class AudioPlayer
    {
        private readonly ActivityLogger _logger;

        public AudioPlayer()
        {
            _logger = new ActivityLogger();
        }

        public async Task PlayGreeting()
        {
            await PlayAudioAsync("Greeting(1).wav");
        }

        public async Task PlayCorrectAnswer()
        {
            await PlayAudioAsync("Correct.wav");
        }

        public async Task PlayIncorrectAnswer()
        {
            await PlayAudioAsync("Incorrect.wav");
        }

        private async Task PlayAudioAsync(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Greeting(1).wav");
            try
            {
                if (File.Exists(filePath))
                {
                    using (SoundPlayer player = new SoundPlayer(filePath))
                    {
                        player.Play();
                    }
                    await _logger.LogActionAsync($"Played audio: {fileName}");
                }
                else
                {
                    await _logger.LogActionAsync($"Audio file not found: {filePath}");
                }
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error playing audio {fileName}: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
    }
}