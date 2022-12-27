using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Behaviors
{
    public class ScrollIntoViewBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            var itemsSource = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (itemsSource != null)
            {
                itemsSource.CollectionChanged += ItemsSource_CollectionChanged;
            }
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AssociatedObject.ScrollIntoView(e.NewItems[0]);
            }
        }

        protected override void OnDetaching()
        {
            var itemsSource = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (itemsSource != null)
            {
                itemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
            }
        }
    }
}
