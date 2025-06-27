using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ST10450557_POE
{
    public class ChatManager
    {
        private readonly ResponseGenerator _responseGenerator;
        private readonly ResponseDisplayer _responseDisplayer;
        private readonly AudioPlayer _audioPlayer;
        private readonly ActivityLogger _logger;
        private readonly TextBlock _chatHistoryTextBlock;
        private readonly Dispatcher _dispatcher;
        private string _userName;

        public ChatManager(TextBlock chatHistoryTextBlock, Dispatcher dispatcher)
        {
            _responseGenerator = new ResponseGenerator();
            _responseDisplayer = new ResponseDisplayer(chatHistoryTextBlock, dispatcher);
            _audioPlayer = new AudioPlayer();
            _logger = new ActivityLogger();
            _chatHistoryTextBlock = chatHistoryTextBlock;
            _dispatcher = dispatcher;
            _userName = "Cyber Explorer";
        }

        public void Start()
        {
            _audioPlayer.PlayGreeting();
            DisplayAsciiArt();
            DisplayResponse($"Welcome, {_userName}! I'm your CyberSafe Chatbot. Type 'help' for commands.", Colors.Cyan);
        }

        public async void SetUserName(string userName)
        {
            _userName = string.IsNullOrWhiteSpace(userName) ? "Cyber Explorer" : userName;
            DisplayResponse($"Welcome, {_userName}! I'm your CyberSafe Chatbot. Type 'help' for commands.", Colors.Cyan);
            await _logger.LogActionAsync($"User set name: {_userName}");
        }

        public async void ProcessInput(string userInput)
        {
            if (userInput.ToLower() == "exit")
            {
                DisplayResponse("Goodbye! Stay cyber aware!", Colors.Yellow);
                await _logger.LogActionAsync("User requested exit via ChatManager");
                Application.Current.Shutdown();
                return;
            }

            string response = _responseGenerator.Generate(userInput.ToLower());
            DisplayResponse(response, Colors.Cyan);
            await _logger.LogActionAsync($"Processed input: {userInput}, Response: {response}");
        }

        public void DisplayResponse(string message, Color color)
        {
            _responseDisplayer.Display(message, color);
        }

        private void DisplayAsciiArt()
        {
            string asciiArt = @"
    ____  _            _             _      _          
 | __ )| | __ _  ___| | _____ _ __(_) ___| |__   ___ 
 |  _ \| |/ _` |/ __| |/ / _ \ `__| |/ __| '_ \ / _ \
 | |__) | | (_| | (__|   <  __/ |  | | (__| | | |  __/
 |____/|_|__,_,_|____|_|_|____|_|  |_|____|_| |_|____|
";
            _responseDisplayer.Display(asciiArt, Colors.Cyan);
        }
    }
}