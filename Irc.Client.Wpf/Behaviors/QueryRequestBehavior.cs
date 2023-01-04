using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Controls;
using Irc.Client.Wpf.Messenger.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace Irc.Client.Wpf.Behaviors
{
    public class QueryRequestBehavior : Behavior<Conversation>
    {
        private readonly IMessenger messenger;

        public string Nickname
        {
            get { return (string)GetValue(NicknameProperty); }
            set { SetValue(NicknameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NickName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NicknameProperty =
            DependencyProperty.Register("Nickname", typeof(string), typeof(QueryRequestBehavior), new PropertyMetadata(null));



        public QueryRequestBehavior()
        {
            messenger = App.Current.Services.GetService<IMessenger>();
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Nickname is null)
            {
                return;
            }

            messenger.Send(new WhoisRequest(Nickname));
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
        }
    }
}
