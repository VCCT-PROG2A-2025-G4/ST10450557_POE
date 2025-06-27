CyberSafe Chatbot
Overview
CyberSafe Chatbot is a WPF application designed to educate users about cybersecurity through an interactive chat interface. It features a chatbot that responds to cybersecurity-related queries, a quiz to test knowledge, and a reminder system to manage tasks. The application logs user activity and persists user data and reminders to files.
Features

Chat Interface: Ask questions about cybersecurity topics (e.g., ransomware, 2FA, VPN) and receive informative responses with a typing effect.
Quiz: Test your cybersecurity knowledge with a 10-question quiz, earning points for correct answers.
Reminder System: Add, view, mark as complete/pending, or delete tasks with names, descriptions, and due dates.
Audio Feedback: Plays audio for greetings and quiz answers (requires .wav files).
Persistent Data: Saves user data (user_data.txt) and reminders (reminders.txt).
Activity Logging: Logs actions and errors to activity_log.txt for debugging.

Prerequisites

Windows OS with .NET Framework (version 4.7.2 or later recommended).
Visual Studio (2019 or later) for building and running the project.
Audio files (Greeting(1).wav, Correct.wav, Incorrect.wav) in the Resources folder.

Setup Instructions

Clone or Download the Project:

Copy the project files to your local machine.


Open in Visual Studio:

Open the .sln file (e.g., ST10450557_POE.sln) in Visual Studio.


Configure Audio Files:

Place Greeting(1).wav, Correct.wav, and Incorrect.wav in the Resources folder in the project.
In Solution Explorer, right-click each file:
Set Build Action to Content.
Set Copy to Output Directory to Copy always.


Ensure the files appear in bin\Debug\Resources or bin\Release\Resources after building.


Build the Solution:

Build the project in Visual Studio (Debug or Release mode).
Check for build errors and resolve any missing dependencies.


Run the Application:

Run the project from Visual Studio.
The main window should open, displaying an ASCII art banner and a welcome message.



Usage

Chat Interface:

Type questions or commands in the input box and press Enter or click Send.
Example: Ask "What is ransomware?" or type "help" for a command list.
Responses appear with a typing effect and scroll automatically.


Commands:

help: Displays available commands.
quiz: Starts a cybersecurity quiz.
history: Shows recent questions.
reminders: Lists all reminders.
reminders add: Adds a new reminder (prompts for name, description, due date).
reminders complete <index>: Marks a reminder as complete.
reminders pending <index>: Marks a reminder as pending.
reminders delete <index>: Deletes a reminder.
exit: Exits the application (confirm with "yes").


Quiz:

Type quiz to open the quiz window.
Answer multiple-choice questions by selecting an option and clicking Submit.
Earn 10 points per correct answer.
Audio feedback plays for correct/incorrect answers if files are configured.


Reminders:

Add a reminder with reminders add, then provide:
Name (e.g., "Study 2FA").
Description (e.g., "Read about two-factor authentication").
Due Date (e.g., "2025-06-30").


View reminders with reminders to see indexed tasks.
Manage tasks with reminders complete 1, reminders pending 1, or reminders delete 1.



File Structure

Source Files:

MainWindow.xaml/MainWindow.xaml.cs: Main chat interface.
QuizWindow.xaml/QuizWindow.xaml.cs: Quiz interface.
AudioPlayer.cs: Handles audio playback.
ActivityLogger.cs: Logs actions and errors.
ResponseGenerator.cs: Generates chatbot responses.
App.xaml/App.xaml.cs: Application entry point.


Data Files (generated in bin\Debug or bin\Release):

user_data.txt: Stores username, favorite topic, points, and questions.
reminders.txt: Stores reminders (format: Name|Description|DueDate|Status).
activity_log.txt: Logs application events and errors.


Resources:

Resources/Greeting(1).wav: Played on startup.
Resources/Correct.wav: Played for correct quiz answers.
Resources/Incorrect.wav: Played for incorrect quiz answers.



Troubleshooting

Audio Not Playing:

Verify audio files are in bin\Debug\Resources or bin\Release\Resources.
Check activity_log.txt for errors like "Failed to play audio".
Ensure files are set to Content and Copy always in the project.


Quiz Window Blank or Freezes:

Check activity_log.txt for "QuizWindow initialization error" or "Error loading question".
Run in Debug mode with breakpoints in QuizWindow.xaml.cs (InitializeAsync, LoadQuestionAsync).
Ensure QuizWindow.xaml matches the provided version.


Application Crashes:

Review activity_log.txt for stack traces.
Ensure file permissions allow writing to user_data.txt, reminders.txt, and activity_log.txt.



Notes

The application uses a dark theme for better readability.
Reminders and user data persist between sessions.
Audio files must be .wav format for compatibility with SoundPlayer.
For further customization, modify ResponseGenerator.cs to add more cybersecurity topics or responses.

License
This project is for educational purposes and not distributed under a specific license.
