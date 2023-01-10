using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace Irc.Client.Wpf.Behaviors
{
    public class TextBoxHistoryBehavior : Behavior<TextBox>
    {
        public int MaxHistoryLength { get; set; }

        List<string> history;
        int historyIndex;

        public TextBoxHistoryBehavior()
        {
            MaxHistoryLength = 100;
            history = new List<string>();
            historyIndex = 0;
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (history.Count >= MaxHistoryLength)
                    {
                        history.RemoveRange(0, MaxHistoryLength - history.Count + 1);
                    }
                    history.Add(AssociatedObject.Text);
                    historyIndex = history.Count;
                    break;
                case Key.Up:
                    if (history.Any())
                    {
                        historyIndex = Math.Max(historyIndex - 1, 0);
                        AssociatedObject.Text = history[historyIndex];
                        AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
                    }
                    break;
                case Key.Down:
                    if (history.Any())
                    {
                        historyIndex = Math.Min(historyIndex + 1, history.Count);
                        AssociatedObject.Text = historyIndex < history.Count
                            ? history[historyIndex]
                            : string.Empty;
                        AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
                    }
                    break;
                case Key.Escape:
                    AssociatedObject.Text = null;
                    break;
            }
        }
    }
}
