using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace ST10450557_POE
{
    public class ResponseDisplayer
    {
        private readonly TextBlock _textBlock;
        private readonly Dispatcher _dispatcher;

        public ResponseDisplayer(TextBlock textBlock, Dispatcher dispatcher)
        {
            _textBlock = textBlock;
            _dispatcher = dispatcher;
        }

        public async void Display(string message, Color color)
        {
            await _dispatcher.InvokeAsync(async () =>
            {
                Run run = new Run();
                _textBlock.Inlines.Add(run);
                _textBlock.Inlines.Add(new Run("\n"));

                foreach (char c in message)
                {
                    run.Text += c;
                    run.Foreground = new SolidColorBrush(color);
                    await Task.Delay(30);
                }

                ScrollViewer scrollViewer = _textBlock.Parent as ScrollViewer;
                scrollViewer?.ScrollToBottom();
            });
        }
    }
}