using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ST10450557_POE
{
    public partial class QuizWindow : Window, INotifyPropertyChanged
    {
        private readonly ResponseGenerator _responseGenerator;
        private readonly ActivityLogger _logger;
        private readonly AudioPlayer _audioPlayer;
        private readonly List<Question> _questions;
        private int _currentQuestionIndex;
        private int _score;
        private int _totalPoints;

        public string CurrentQuestionText => _questions != null && _currentQuestionIndex < _questions.Count
            ? $"Question {_currentQuestionIndex + 1}: {_questions[_currentQuestionIndex].Text}"
            : "Loading question...";
        public string ScoreText => $"Score: {_score}/{_questions?.Count ?? 0}";

        public event PropertyChangedEventHandler PropertyChanged;

        public QuizWindow(ResponseGenerator responseGenerator, ActivityLogger logger, ref int totalPoints)
        {
            _responseGenerator = responseGenerator ?? throw new ArgumentNullException(nameof(responseGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _audioPlayer = new AudioPlayer();
            _totalPoints = totalPoints;
            _questions = GenerateQuestions();
            _currentQuestionIndex = 0;
            _score = 0;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _logger.LogActionAsync("QuizWindow initialization started");
                InitializeComponent();
                await _logger.LogActionAsync("InitializeComponent completed");
                DataContext = this;
                await LoadQuestionAsync();
                await _logger.LogActionAsync("Quiz window opened and first question loaded");
                this.Visibility = Visibility.Visible;
                this.Activate();
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"QuizWindow initialization error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Failed to initialize QuizWindow: {ex.Message}\nStackTrace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private List<Question> GenerateQuestions()
        {
            return new List<Question>
            {
                new Question("What is the minimum length for a strong password?", new[] { "8 characters", "10 characters", "12 characters", "16 characters" }, 2, "A strong password should be at least 12 characters long, including uppercase, lowercase, numbers, and special characters."),
                new Question("What does 2FA stand for?", new[] { "Two-Factor Analysis", "Two-Factor Authentication", "Twin-Factor Access", "Two-Factor Approval" }, 1, "2FA stands for Two-Factor Authentication, adding an extra security layer like a phone code."),
                new Question("What is ransomware?", new[] { "A firewall", "Malware that encrypts files", "A VPN", "An antivirus" }, 1, "Ransomware is malware that encrypts files and demands a ransom for decryption."),
                new Question("What does a firewall do?", new[] { "Encrypts data", "Stores passwords", "Monitors network traffic", "Scans for viruses" }, 2, "A firewall monitors and controls incoming and outgoing network traffic to prevent unauthorized access."),
                new Question("What is a VPN used for?", new[] { "Password generation", "File storage", "Network encryption", "Virus removal" }, 2, "A VPN encrypts your internet connection, ensuring privacy on public Wi-Fi."),
                new Question("What is social engineering?", new[] { "Software development", "Network hacking", "Data encryption", "Manipulating people" }, 3, "Social engineering manipulates people into revealing confidential information."),
                new Question("What is a zero-day attack?", new[] { "A virus", "A worm", "An unknown vulnerability exploit", "A known exploit" }, 2, "A zero-day attack exploits vulnerabilities before developers can patch them."),
                new Question("What is the dark web?", new[] { "A secure browser", "A hidden internet part", "A public network", "An antivirus tool" }, 1, "The dark web is a hidden internet part accessible only via special software, often used for illegal activities."),
                new Question("What is spear phishing?", new[] { "Mass phishing", "Data encryption", "Targeted phishing", "Network attack" }, 2, "Spear phishing is a targeted phishing attack using personalized information to trick victims."),
                new Question("What prevents SQL injection?", new[] { "VPNs", "Antivirus", "Firewalls", "Parameterized queries" }, 3, "Parameterized queries prevent SQL injection by separating SQL code from user input.")
            };
        }

        private async Task LoadQuestionAsync()
        {
            try
            {
                OnPropertyChanged(nameof(CurrentQuestionText));
                OnPropertyChanged(nameof(ScoreText));
                OptionsStackPanel.Children.Clear();
                var question = _questions[_currentQuestionIndex];
                for (int i = 0; i < question.Options.Length; i++)
                {
                    var radio = new RadioButton
                    {
                        Content = question.Options[i],
                        Foreground = new SolidColorBrush(Colors.White),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 5),
                        Tag = i
                    };
                    OptionsStackPanel.Children.Add(radio);
                }
                SubmitButton.IsEnabled = true;
                NextButton.Visibility = Visibility.Hidden;
                await _logger.LogActionAsync($"Loaded question {_currentQuestionIndex + 1}: {question.Text}");
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error loading question: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Failed to load question: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRadio = OptionsStackPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);
                if (selectedRadio == null)
                {
                    MessageBox.Show("Please select an answer.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    await _logger.LogActionAsync("User attempted to submit without selecting an answer");
                    return;
                }

                int selectedIndex = (int)selectedRadio.Tag;
                var question = _questions[_currentQuestionIndex];
                bool isCorrect = selectedIndex == question.CorrectAnswerIndex;

                if (isCorrect)
                {
                    _score++;
                    _totalPoints += 10;
                    await _audioPlayer.PlayCorrectAnswer();
                    MessageBox.Show($"Correct! {question.Explanation}", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _logger.LogActionAsync($"Correct answer for question {_currentQuestionIndex + 1}: {selectedRadio.Content}");
                }
                else
                {
                    await _audioPlayer.PlayIncorrectAnswer();
                    MessageBox.Show($"Incorrect. {question.Explanation}", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
                    await _logger.LogActionAsync($"Incorrect answer for question {_currentQuestionIndex + 1}: {selectedRadio.Content}");
                }

                SubmitButton.IsEnabled = false;
                NextButton.Visibility = Visibility.Visible;
                OnPropertyChanged(nameof(ScoreText));
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error processing submit: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error processing answer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentQuestionIndex++;
                if (_currentQuestionIndex < _questions.Count)
                {
                    await LoadQuestionAsync();
                }
                else
                {
                    MessageBox.Show($"Quiz completed! Your score: {_score}/{_questions}.Count. Total Points: {_totalPoints}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _logger.LogActionAsync($"Quiz completed. Score: {_score}/{_questions}.Count, Total Points: {_totalPoints}");
                    Close();
                }
            }
            catch (Exception ex)
            {
                await _logger.LogActionAsync($"Error navigating to next. question: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show("Failed to load next question: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Question
    {
        public string Text { get; }
        public string[] Options { get; }
        public int CorrectAnswerIndex { get; }
        public string Explanation { get; }

        public Question(string text, string[] options, int correctAnswerIndex, string explanation)
        {
            Text = text;
            Options = options;
            CorrectAnswerIndex = correctAnswerIndex;
            Explanation = explanation;
        }
    }
}