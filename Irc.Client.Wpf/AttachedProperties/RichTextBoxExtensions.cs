using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Irc.Client.Wpf.AttachedProperties
{
    public class RichTextBoxExtensions
    {
        public static ObservableCollection<MessageViewModel> GetMessages(DependencyObject obj)
        {
            return (ObservableCollection<MessageViewModel>)obj.GetValue(MessagesProperty);
        }

        public static void SetMessages(DependencyObject obj, ObservableCollection<MessageViewModel> value)
        {
            obj.SetValue(MessagesProperty, value);
        }

        // Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.RegisterAttached("Messages", typeof(ObservableCollection<MessageViewModel>), typeof(RichTextBoxExtensions), new PropertyMetadata(null, OnMessagesChanged));

        private static void OnMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox richTextBox || richTextBox.Document.Blocks.FirstOrDefault() is not Paragraph paragraph)
            {
                return;
            }

            var messagesCollectionChanged = (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                OnCollectionChanged(richTextBox, e);
            };

            var handler = new NotifyCollectionChangedEventHandler(messagesCollectionChanged);
            if (e.OldValue is ObservableCollection<MessageViewModel> oldMessages)
            {
                oldMessages.CollectionChanged -= handler;

                paragraph.Inlines.Clear();
            }
            if (e.NewValue is ObservableCollection<MessageViewModel> newMessages)
            {
                AddMessages(paragraph, newMessages);
                richTextBox.ScrollToEnd();

                newMessages.CollectionChanged += handler;
            }
        }

        private static void OnCollectionChanged(RichTextBox richTextBox, NotifyCollectionChangedEventArgs e)
        {
            if (richTextBox.Document.Blocks.FirstOrDefault() is not Paragraph paragraph)
            {
                return;
            }

            var messages = e.NewItems.Cast<MessageViewModel>();
            AddMessages(paragraph, messages);

            richTextBox.ScrollToEnd();
        }

        private static void AddMessages(Paragraph paragraph, IEnumerable<MessageViewModel> messages)
        {
            foreach (var message in messages)
            {
                AddMessage(paragraph, message);
            }
        }

        private static void AddMessage(Paragraph paragraph, MessageViewModel message)
        {
            var text = message switch
            {
                ChatMessageViewModel chatMessage => $"[{chatMessage.DateTime:hh:mm}] <{chatMessage.Nickname}> {chatMessage.Message}",
                _ => $"[{message.DateTime:hh:mm}] {message.Message}"
            };

            paragraph.Inlines.Add(new Run(text));
            paragraph.Inlines.Add(new LineBreak());
        }
    }
}
