using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Behaviors;

public class ScrollIntoViewBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        var items = AssociatedObject.Items as INotifyCollectionChanged;
        if (items is not null)
        {
            items.CollectionChanged += Items_CollectionChanged;
        }
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            AssociatedObject.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
        }
    }

    protected override void OnDetaching()
    {
        var items = AssociatedObject.Items as INotifyCollectionChanged;
        if (items is not null)
        {
            items.CollectionChanged -= Items_CollectionChanged;
        }
    }
}
