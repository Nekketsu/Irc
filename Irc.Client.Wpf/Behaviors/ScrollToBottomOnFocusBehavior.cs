using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Behaviors
{
    public class ScrollToBottomOnFocusBehavior : Behavior<ListBox>
    {

        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
        }

        private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var itemCount = AssociatedObject.Items?.Count ?? 0;

            if (itemCount > 0)
            {
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[itemCount - 1]);
            }
        }
    }
}
