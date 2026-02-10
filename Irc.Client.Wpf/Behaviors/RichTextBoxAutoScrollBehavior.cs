using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Behaviors;

public class RichTextBoxAutoScrollBehavior : Behavior<RichTextBox>
{
    protected override void OnAttached()
    {
        AssociatedObject.TextChanged += AssociatedObject_TextChanged;
    }

    private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
    {
        AssociatedObject.ScrollToEnd();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
    }
}
