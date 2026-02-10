using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Irc.Client.Wpf.Controls;

public class Conversation : RichTextBox
{
    NotifyCollectionChangedEventHandler collectionChangedHandler;

    public Conversation()
    {
        Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
        Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
        BorderBrush = new SolidColorBrush(Color.FromRgb(0x3F, 0x3F, 0x46));
        Unloaded += Conversation_Unloaded;
    }

    private void Conversation_Unloaded(object sender, RoutedEventArgs e)
    {
        if (Messages is not null && collectionChangedHandler is not null)
        {
            Messages.CollectionChanged -= collectionChangedHandler;
        }
    }

    public ObservableCollection<MessageViewModel> Messages
    {
        get { return (ObservableCollection<MessageViewModel>)GetValue(MessagesProperty); }
        set { SetValue(MessagesProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MessagesProperty =
        DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageViewModel>), typeof(Conversation), new UIPropertyMetadata(null, OnMessagesChanged));

    private static void OnMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Conversation conversationControl || conversationControl.Document.Blocks.FirstOrDefault() is not Paragraph paragraph)
        {
            return;
        }

        var messagesCollectionChanged = (object sender, NotifyCollectionChangedEventArgs e) =>
        {
            conversationControl.OnCollectionChanged(conversationControl, e);
        };

        if (e.OldValue is ObservableCollection<MessageViewModel> oldMessages && conversationControl.collectionChangedHandler is not null)
        {
            oldMessages.CollectionChanged -= conversationControl.collectionChangedHandler;

            paragraph.Inlines.Clear();
        }
        if (e.NewValue is ObservableCollection<MessageViewModel> newMessages)
        {
            conversationControl.collectionChangedHandler = new NotifyCollectionChangedEventHandler(messagesCollectionChanged);

            conversationControl.AddMessages(paragraph, newMessages);
            conversationControl.ScrollToEnd();

            newMessages.CollectionChanged += conversationControl.collectionChangedHandler;
        }
    }

    private void OnCollectionChanged(Conversation conversationControl, NotifyCollectionChangedEventArgs e)
    {
        if (conversationControl.Document.Blocks.FirstOrDefault() is not Paragraph paragraph)
        {
            return;
        }

        var messages = e.NewItems.Cast<MessageViewModel>();
        AddMessages(paragraph, messages);

        conversationControl.ScrollToEnd();
    }

    private void AddMessages(Paragraph paragraph, IEnumerable<MessageViewModel> messages)
    {
        foreach (var message in messages)
        {
            AddMessage(paragraph, message);
        }
    }

    private void AddMessage(Paragraph paragraph, MessageViewModel message)
    {
        var text = message switch
        {
            ChatMessageViewModel chatMessage => $"[{chatMessage.DateTime:hh:mm}] <{chatMessage.Nickname}> {chatMessage.Message}",
            _ => $"[{message.DateTime:hh:mm}] {message.Message}"
        };


        var foreground = message.MessageKind switch
        {
            MessageKind.Action => ActionTextBrush,
            MessageKind.Ctcp => CtcpTextBrush,
            MessageKind.Highlight => HighlightTextBrush,
            MessageKind.Info => InfoTextBrush,
            MessageKind.Info2 => Info2TextBrush,
            MessageKind.Invite => InviteTextBrush,
            MessageKind.Join => JoinTextBrush,
            MessageKind.Kick => KickTextBrush,
            MessageKind.Mode => ModeTextBrush,
            MessageKind.Nick => NickTextBrush,
            MessageKind.Normal => NormalTextBrush,
            MessageKind.Notice => NoticeTextBrush,
            MessageKind.Notify => NotifyTextBrush,
            MessageKind.Other => OtherTextBrush,
            MessageKind.Own => OwnTextBrush,
            MessageKind.Part => PartTextBrush,
            MessageKind.Quit => QuitTextBrush,
            MessageKind.Topic => TopicTextBrush,
            MessageKind.Wallops => WallopsTextBrush,
            MessageKind.Whois => WhoisTextBrush,
            _ => NormalTextBrush
        };

        var run = new Run(text) { Foreground = foreground };
        paragraph.Inlines.Add(run);
        paragraph.Inlines.Add(new LineBreak());
    }

    public Brush ActionTextBrush
    {
        get { return (Brush)GetValue(ActionTextBrushProperty); }
        set { SetValue(ActionTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ActionTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ActionTextBrushProperty =
        DependencyProperty.Register("ActionTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(156, 0, 156))));


    public Brush CtcpTextBrush
    {
        get { return (Brush)GetValue(CtcpTextBrushProperty); }
        set { SetValue(CtcpTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CtcpTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CtcpTextBrushProperty =
        DependencyProperty.Register("CtcpTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0))));


    public Brush HighlightTextBrush
    {
        get { return (Brush)GetValue(HighlightTextBrushProperty); }
        set { SetValue(HighlightTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HighlightTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HighlightTextBrushProperty =
        DependencyProperty.Register("HighlightTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(127, 0, 0))));


    public Brush InfoTextBrush
    {
        get { return (Brush)GetValue(InfoTextBrushProperty); }
        set { SetValue(InfoTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for InfoTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty InfoTextBrushProperty =
        DependencyProperty.Register("InfoTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 127))));


    public Brush Info2TextBrush
    {
        get { return (Brush)GetValue(Info2TextBrushProperty); }
        set { SetValue(Info2TextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Info2TextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty Info2TextBrushProperty =
        DependencyProperty.Register("Info2TextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush InviteTextBrush
    {
        get { return (Brush)GetValue(InviteTextBrushProperty); }
        set { SetValue(InviteTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for InviteTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty InviteTextBrushProperty =
        DependencyProperty.Register("InviteTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush JoinTextBrush
    {
        get { return (Brush)GetValue(JoinTextBrushProperty); }
        set { SetValue(JoinTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for JoinTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty JoinTextBrushProperty =
        DependencyProperty.Register("JoinTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush KickTextBrush
    {
        get { return (Brush)GetValue(KickTextBrushProperty); }
        set { SetValue(KickTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for KickTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty KickTextBrushProperty =
        DependencyProperty.Register("KickTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush ModeTextBrush
    {
        get { return (Brush)GetValue(ModeTextBrushProperty); }
        set { SetValue(ModeTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ModeTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ModeTextBrushProperty =
        DependencyProperty.Register("ModeTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush NickTextBrush
    {
        get { return (Brush)GetValue(NickTextBrushProperty); }
        set { SetValue(NickTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NickTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NickTextBrushProperty =
        DependencyProperty.Register("NickTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush NormalTextBrush
    {
        get { return (Brush)GetValue(NormalTextBrushProperty); }
        set { SetValue(NormalTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NormalTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NormalTextBrushProperty =
        DependencyProperty.Register("NormalTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(SystemColors.HighlightTextBrush));


    public Brush NoticeTextBrush
    {
        get { return (Brush)GetValue(NoticeTextBrushProperty); }
        set { SetValue(NoticeTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NoticeTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NoticeTextBrushProperty =
        DependencyProperty.Register("NoticeTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(127, 0, 0))));


    public Brush NotifyTextBrush
    {
        get { return (Brush)GetValue(NotifyTextBrushProperty); }
        set { SetValue(NotifyTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NotifyTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NotifyTextBrushProperty =
        DependencyProperty.Register("NotifyTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(252, 127, 0))));


    public Brush OtherTextBrush
    {
        get { return (Brush)GetValue(OtherTextBrushProperty); }
        set { SetValue(OtherTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OtherTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OtherTextBrushProperty =
        DependencyProperty.Register("OtherTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(156, 0, 156))));


    public Brush OwnTextBrush
    {
        get { return (Brush)GetValue(OwnTextBrushProperty); }
        set { SetValue(OwnTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OwnTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OwnTextBrushProperty =
        DependencyProperty.Register("OwnTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(Brushes.Black));


    public Brush PartTextBrush
    {
        get { return (Brush)GetValue(PartTextBrushProperty); }
        set { SetValue(PartTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PartTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PartTextBrushProperty =
        DependencyProperty.Register("PartTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush QuitTextBrush
    {
        get { return (Brush)GetValue(QuitTextBrushProperty); }
        set { SetValue(QuitTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for QuitTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty QuitTextBrushProperty =
        DependencyProperty.Register("QuitTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 127))));


    public Brush TopicTextBrush
    {
        get { return (Brush)GetValue(TopicTextBrushProperty); }
        set { SetValue(TopicTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TopicTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TopicTextBrushProperty =
        DependencyProperty.Register("TopicTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 147, 0))));


    public Brush WallopsTextBrush
    {
        get { return (Brush)GetValue(WallopsTextBrushProperty); }
        set { SetValue(WallopsTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for WallopsTextBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WallopsTextBrushProperty =
        DependencyProperty.Register("WallopsTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(127, 0, 0))));


    public Brush WhoisTextBrush
    {
        get { return (Brush)GetValue(WhoisTextBrushProperty); }
        set { SetValue(WhoisTextBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for WhoisBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WhoisTextBrushProperty =
        DependencyProperty.Register("WhoisTextBrush", typeof(Brush), typeof(Conversation), new PropertyMetadata(Brushes.Black));
}
