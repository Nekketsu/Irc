using Irc.Client.Wpf.ViewModels.Tabs;
using System.Windows;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Views.Tabs
{
    /// <summary>
    /// Interaction logic for StatusView.xaml
    /// </summary>
    public partial class StatusView : UserControl
    {
        public StatusView()
        {
            InitializeComponent();
        }


        public StatusViewModel Status
        {
            get { return (StatusViewModel)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StatusViewModel), typeof(StatusView), new PropertyMetadata(null));
    }
}
