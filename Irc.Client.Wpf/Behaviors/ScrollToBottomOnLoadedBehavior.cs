using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Behaviors;

public class ScrollToBottomOnLoadedBehavior : Behavior<ListBox>
{

    protected override void OnAttached()
    {
        AssociatedObject.Loaded += AssociatedObject_Loaded;
    }

    private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var itemCount = AssociatedObject.Items?.Count ?? 0;

        if (itemCount > 0)
        {
            AssociatedObject.ScrollIntoView(AssociatedObject.Items[itemCount - 1]);
        }
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= AssociatedObject_Loaded;
    }
}
