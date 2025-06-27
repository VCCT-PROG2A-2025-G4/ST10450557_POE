using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace ST10450557_POE
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _userInput;
        private string _userName = "Cyber Explorer";
        private string _favoriteTopic;
        private int _totalPoints;
        private string _currentTheme = "dark";
        private string _lastCyberTopic;
        private readonly ResponseGenerator _responseGenerator;
        private readonly ActivityLogger _logger;
        private readonly List<string> _userQuestions = new List<string>();
        private readonly ObservableCollection<Reminder> _reminders = new ObservableCollection<Reminder>();
        private string _pendingReminderCommand;

        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                OnPropertyChanged(nameof(UserInput));
            }
        }

        public string StatusMessage => $"Welcome, {_userName} | Time: {DateTime.Now:HH:mm} SAST | Points: {_totalPoints} | Theme: {_currentTheme}";

        public MainWindow()
        {
            _responseGenerator = new ResponseGenerator();
            _logger = new ActivityLogger();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _logger.LogActionAsync("MainWindow initialization started");
                InitializeComponent();
                await _logger.LogActionAsync("InitializeComponent completed");
                DataContext = this;
                await LoadRemindersAsync();
                DisplayAsciiArt();
                await _logger.LogActionAsync("DisplayAsciiArt completed");
                ChatMessages.Add(new ChatMessage($"Welcome, {_userName}! I'm your CyberSafe Chatbot. Type 'help' for commands.", Colors.Cyan));
                await _logger.LogActionAsync("MainWindow initialized");
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal;
                this.Activate();
                await _logger.LogActionAsync("MainWindow set to visible and activated");
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"MainWindow initialization error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Failed to initialize MainWindow: {ex.Message}\nStackTrace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInputAsync();
        }

        private async void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ProcessUserInputAsync();
                e.Handled = true;
            }
        }

        private async Task ProcessUserInputAsync()
        {
            if (string.IsNullOrWhiteSpace(UserInput))
            {
                ChatMessages.Add(new ChatMessage("❌ Please enter a valid question.", Colors.Red));
                await _logger.LogActionAsync("User attempted to send empty message");
                ScrollToBottom();
                return;
            }

            var input = UserInput.Trim();
            var inputLower = input.ToLower();
            ChatMessages.Add(new ChatMessage($"{_userName}: {input}", Colors.Green));
            await _logger.LogActionAsync($"User input: {input}");
            UserInput = string.Empty;
            ScrollToBottom();

            if (!NamePromptTextBlock.Visibility.Equals(Visibility.Collapsed))
            {
                _userName = string.IsNullOrWhiteSpace(input) ? "Cyber Explorer" : input;
                NamePromptTextBlock.Visibility = Visibility.Collapsed;
                ChatMessages.Add(new ChatMessage($"Welcome, {_userName}! I'm your CyberSafe Chatbot. Type 'help' for commands.", Colors.Cyan));
                OnPropertyChanged(nameof(StatusMessage));
                await _logger.LogActionAsync($"User set name: {_userName}");
                ScrollToBottom();
                return;
            }

            if (!string.IsNullOrEmpty(_pendingReminderCommand))
            {
                await ProcessReminderCommandAsync(input);
                return;
            }

            if (inputLower == "exit")
            {
                string recall = _favoriteTopic != null ? $"I remember you were interested in {_favoriteTopic}. " : "";
                ChatMessages.Add(new ChatMessage($"{recall}Are you sure you want to exit? (Type 'yes' to confirm)", Colors.Yellow));
                await _logger.LogActionAsync("User requested exit");
                ScrollToBottom();
                return;
            }
            else if (inputLower == "yes")
            {
                await SaveUserDataAsync();
                await SaveRemindersAsync();
                ChatMessages.Add(new ChatMessage($"Goodbye, {_userName}! Stay cyber aware! Total Points: {_totalPoints}", Colors.Yellow));
                await _logger.LogActionAsync("User exited application");
                ScrollToBottom();
                Application.Current.Shutdown();
                return;
            }
            else if (inputLower == "help")
            {
                DisplayHelpMenu();
                await _logger.LogActionAsync("User requested help menu");
                ScrollToBottom();
            }
            else if (inputLower == "quiz")
            {
                await StartQuiz();
                await _logger.LogActionAsync("User started quiz");
            }
            else if (inputLower == "history")
            {
                DisplayQuestionHistory();
                await _logger.LogActionAsync("User requested question history");
                ScrollToBottom();
            }
            else if (inputLower.StartsWith("reminders"))
            {
                await HandleRemindersCommandAsync(inputLower);
            }
            else
            {
                string sentiment = DetectSentiment(inputLower);
                if (!string.IsNullOrEmpty(sentiment))
                {
                    ChatMessages.Add(new ChatMessage(sentiment, Colors.Magenta));
                    await _logger.LogActionAsync($"Detected sentiment: {sentiment}");
                    ScrollToBottom();
                }

                _userQuestions.Add(input);
                if (_userQuestions.Count > 10) _userQuestions.RemoveAt(0);

                if (inputLower.Contains("more") && !string.IsNullOrEmpty(_lastCyberTopic))
                {
                    string response = _responseGenerator.GenerateMore(_lastCyberTopic);
                    await DisplayResponseAsync($"Chatbot: {response}", Colors.Cyan);
                    await _logger.LogActionAsync($"User requested more info on {_lastCyberTopic}");
                }
                else
                {
                    string response = _responseGenerator.Generate(inputLower);
                    if (!response.Contains("rephrase"))
                    {
                        _lastCyberTopic = ExtractCyberTopic(inputLower);
                        if (_favoriteTopic == null || _userQuestions.Count(q => q.ToLower().Contains(_lastCyberTopic)) > _userQuestions.Count(q => q.ToLower().Contains(_favoriteTopic)))
                        {
                            _favoriteTopic = _lastCyberTopic;
                        }
                    }
                    await DisplayResponseAsync($"Chatbot: {response}", Colors.Cyan);
                    await _logger.LogActionAsync($"Generated response: {response}");
                }
            }
        }

        private async Task HandleRemindersCommandAsync(string inputLower)
        {
            try
            {
                var parts = inputLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1) // "reminders"
                {
                    DisplayReminders();
                    await _logger.LogActionAsync("User requested reminder list");
                }
                else if (parts.Length > 1)
                {
                    var action = parts[1].ToLower();
                    if (action == "add")
                    {
                        _pendingReminderCommand = "add_name";
                        ChatMessages.Add(new ChatMessage("Please enter the name of the task.", Colors.Cyan));
                        await _logger.LogActionAsync("User started adding reminder");
                    }
                    else if (action == "complete" && parts.Length > 2)
                    {
                        int index;
                        if (int.TryParse(parts[2], out index) && index > 0 && index <= _reminders.Count)
                        {
                            _reminders[index - 1].Status = "Complete";
                            ChatMessages.Add(new ChatMessage($"Reminder '{_reminders[index - 1].Name}' marked as complete.", Colors.Green));
                            await SaveRemindersAsync();
                            await _logger.LogActionAsync($"User marked reminder {index} as complete");
                        }
                        else
                        {
                            ChatMessages.Add(new ChatMessage("Invalid reminder index.", Colors.Red));
                            await _logger.LogActionAsync("User provided invalid reminder index for complete");
                        }
                    }
                    else if (action == "pending" && parts.Length > 2)
                    {
                        int index;
                        if (int.TryParse(parts[2], out index) && index > 0 && index <= _reminders.Count)
                        {
                            _reminders[index - 1].Status = "Pending";
                            ChatMessages.Add(new ChatMessage($"Reminder '{_reminders[index - 1].Name}' marked as pending.", Colors.Green));
                            await SaveRemindersAsync();
                            await _logger.LogActionAsync($"User marked reminder {index} as pending");
                        }
                        else
                        {
                            ChatMessages.Add(new ChatMessage("Invalid reminder index.", Colors.Red));
                            await _logger.LogActionAsync("User provided invalid reminder index for pending");
                        }
                    }
                    else if (action == "delete" && parts.Length > 2)
                    {
                        int index;
                        if (int.TryParse(parts[2], out index) && index > 0 && index <= _reminders.Count)
                        {
                            var reminderName = _reminders[index - 1].Name;
                            _reminders.RemoveAt(index - 1);
                            ChatMessages.Add(new ChatMessage($"Reminder '{reminderName}' deleted.", Colors.Green));
                            await SaveRemindersAsync();
                            await _logger.LogActionAsync($"User deleted reminder {index}");
                        }
                        else
                        {
                            ChatMessages.Add(new ChatMessage("Invalid reminder index.", Colors.Red));
                            await _logger.LogActionAsync("User provided invalid reminder index for delete");
                        }
                    }
                    else
                    {
                        ChatMessages.Add(new ChatMessage("Invalid reminder command. Use: reminders [add | complete <index> | pending <index> | delete <index>]", Colors.Red));
                        await _logger.LogActionAsync("User provided invalid reminder command");
                    }
                }
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error processing reminder command: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ChatMessages.Add(new ChatMessage($"Error processing reminder: {ex.Message}", Colors.Red));
                ScrollToBottom();
            }
        }

        private async Task ProcessReminderCommandAsync(string input)
        {
            try
            {
                if (_pendingReminderCommand == "add_name")
                {
                    _pendingReminderCommand = "add_description";
                    _reminders.Add(new Reminder { Name = input, Status = "Pending" });
                    ChatMessages.Add(new ChatMessage("Please enter the task description.", Colors.Cyan));
                    await _logger.LogActionAsync($"User provided reminder name: {input}");
                }
                else if (_pendingReminderCommand == "add_description")
                {
                    _pendingReminderCommand = "add_date";
                    _reminders[_reminders.Count - 1].Description = input;
                    ChatMessages.Add(new ChatMessage("Please enter the due date (e.g., yyyy-MM-dd).", Colors.Cyan));
                    await _logger.LogActionAsync($"User provided reminder description: {input}");
                }
                else if (_pendingReminderCommand == "add_date")
                {
                    if (DateTime.TryParse(input, out DateTime dueDate))
                    {
                        _reminders[_reminders.Count - 1].DueDate = dueDate;
                        ChatMessages.Add(new ChatMessage($"Reminder '{_reminders[_reminders.Count - 1].Name}' added successfully.", Colors.Green));
                        await SaveRemindersAsync();
                        await _logger.LogActionAsync($"User added reminder with due date: {input}");
                        _pendingReminderCommand = null;
                    }
                    else
                    {
                        ChatMessages.Add(new ChatMessage("Invalid date format. Please use yyyy-MM-dd.", Colors.Red));
                        await _logger.LogActionAsync("User provided invalid reminder date format");
                    }
                }
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error processing reminder input: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ChatMessages.Add(new ChatMessage($"Error adding reminder: {ex.Message}", Colors.Red));
                _pendingReminderCommand = null;
                ScrollToBottom();
            }
        }

        private void DisplayReminders()
        {
            if (_reminders.Count == 0)
            {
                ChatMessages.Add(new ChatMessage("No reminders set.", Colors.Yellow));
                return;
            }

            string reminderText = "Reminders:\n" + string.Join("\n", _reminders.Select((r, i) =>
                $"{i + 1}. {r.Name} - {r.Description} (Due: {r.DueDate:yyyy-MM-dd}, Status: {r.Status})"));
            ChatMessages.Add(new ChatMessage(reminderText, Colors.Cyan));
        }

        private async Task LoadRemindersAsync()
        {
            try
            {
                if (File.Exists("reminders.txt"))
                {
                    var lines = await File.ReadAllLinesAsync("reminders.txt");
                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 4 && DateTime.TryParse(parts[2], out DateTime dueDate))
                        {
                            _reminders.Add(new Reminder
                            {
                                Name = parts[0],
                                Description = parts[1],
                                DueDate = dueDate,
                                Status = parts[3]
                            });
                        }
                    }
                    await _logger.LogActionAsync("Loaded reminders from file");
                }
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error loading reminders: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private async Task SaveRemindersAsync()
        {
            try
            {
                var data = string.Join("\n", _reminders.Select(r => $"{r.Name}|{r.Description}|{r.DueDate:yyyy-MM-dd}|{r.Status}"));
                await File.WriteAllTextAsync("reminders.txt", data);
                await _logger.LogActionAsync("Saved reminders to file");
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error saving reminders: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private async Task DisplayResponseAsync(string message, Color color)
        {
            var chatMessage = new ChatMessage("", color);
            ChatMessages.Add(chatMessage);

            foreach (char c in message)
            {
                chatMessage.Message += c;
                ChatMessages[ChatMessages.Count - 1] = new ChatMessage(chatMessage.Message, color);
                ScrollToBottom();
                await Task.Delay(30);
            }

            ChatMessages.Add(new ChatMessage("", color));
            ScrollToBottom();
        }

        private async Task StartQuiz()
        {
            try
            {
                await _logger.LogActionAsync("Starting quiz window");
                var quizWindow = new QuizWindow(_responseGenerator, _logger, ref _totalPoints);
                await quizWindow.InitializeAsync();
                quizWindow.Closed += (s, e) => OnPropertyChanged(nameof(StatusMessage));
                quizWindow.ShowDialog();
                await SaveUserDataAsync();
                await _logger.LogActionAsync("Quiz window closed and user data saved");
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error starting quiz: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ChatMessages.Add(new ChatMessage($"Error starting quiz: {ex.Message}", Colors.Red));
                ScrollToBottom();
            }
        }

        private void DisplayHelpMenu()
        {
            string helpText = @"Available Commands:
- help: Show this menu
- quiz: Start a cybersecurity quiz
- history: View recent questions
- reminders: View all reminders
- reminders add: Add a new reminder
- reminders complete <index>: Mark reminder as complete
- reminders pending <index>: Mark reminder as pending
- reminders delete <index>: Delete a reminder
- exit: Exit the application
- [cybersecurity topic]: Ask about topics like 'ransomware', '2fa', 'vpn', etc.
- more: Get more info on the last topic (e.g., '2fa more')";
            ChatMessages.Add(new ChatMessage(helpText, Colors.Cyan));
            ScrollToBottom();
        }

        private void DisplayQuestionHistory()
        {
            if (_userQuestions.Count == 0)
            {
                ChatMessages.Add(new ChatMessage("No questions asked yet.", Colors.Yellow));
                return;
            }
            string history = "Recent Questions:\n" + string.Join("\n", _userQuestions.Select((q, i) => $"{i + 1}. {q}"));
            ChatMessages.Add(new ChatMessage(history, Colors.Cyan));
            ScrollToBottom();
        }

        private string DetectSentiment(string input)
        {
            input = input.ToLower();
            if (input.Contains("worried") || input.Contains("anxious") || input.Contains("nervous") || input.Contains("uneasy") || input.Contains("concerned"))
                return $"I understand your concern, {_userName}! Let's go over some tips.";
            if (input.Contains("curious") || input.Contains("interested") || input.Contains("intrigued") || input.Contains("eager") || input.Contains("wondering"))
                return $"Great to see your curiosity, {_userName}! Let's explore that topic together.";
            if (input.Contains("frustrated") || input.Contains("annoyed") || input.Contains("irritated") || input.Contains("upset") || input.Contains("aggravated"))
                return $"Sorry you're frustrated, {_userName}. Let's break it down.";
            if (input.Contains("happy") || input.Contains("glad") || input.Contains("joyful") || input.Contains("excited") || input.Contains("pleased"))
                return $"Glad you're happy, {_userName}! Let's keep learning.";
            if (input.Contains("confused") || input.Contains("puzzled") || input.Contains("uncertain") || input.Contains("lost") || input.Contains("unclear"))
                return $"No worries, {_userName}, I'll clarify that for you.";
            if (input.Contains("angry") || input.Contains("mad") || input.Contains("furious") || input.Contains("irate") || input.Contains("enraged"))
                return $"Sorry you're angry, {_userName}. Let's address this calmly.";
            if (input.Contains("scared") || input.Contains("afraid") || input.Contains("frightened") || input.Contains("panicked") || input.Contains("terrified"))
                return $"It's okay to feel scared, {_userName}. I'm here to help.";
            return "";
        }

        private string ExtractCyberTopic(string input)
        {
            foreach (var keyword in _responseGenerator.Keywords)
            {
                if (input.Contains(keyword))
                    return keyword;
            }
            return null;
        }

        private async void PlayGreeting()
        {
            try
            {
                await new AudioPlayer().PlayGreeting();
                await _logger.LogActionAsync("Greeting audio played successfully");
            }
            catch (Exception ex)
            {
                ChatMessages.Add(new ChatMessage($"Error playing greeting: {ex.Message}", Colors.Red));
                await _logger.LogActionAsync($"Error playing greeting: {ex.Message}");
                ScrollToBottom();
            }
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
            ChatMessages.Add(new ChatMessage(asciiArt, Colors.Cyan));
            ScrollToBottom();
        }

        private async Task SaveUserDataAsync()
        {
            try
            {
                var data = $"Username: {_userName}\nFavoriteTopic: {_favoriteTopic}\nTotalPoints: {_totalPoints}\nQuestions: {string.Join(";", _userQuestions)}";
                await File.WriteAllTextAsync("user_data.txt", data);
                await _logger.LogActionAsync("Saved user data");
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error saving user data: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void ScrollToBottom()
        {
            if (ChatScrollViewer != null)
            {
                ChatScrollViewer.ScrollToBottom();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserInputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NamePromptTextBlock.Visibility == Visibility.Visible)
            {
                UserInputTextBox.Text = "";
            }
        }
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public SolidColorBrush Color { get; }

        public ChatMessage(string message, Color color)
        {
            Message = message;
            Color = new SolidColorBrush(color);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Reminder
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}