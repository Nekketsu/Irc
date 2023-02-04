using Microsoft.JSInterop;

namespace Irc.Client.Maui.Blazor.Models
{
    public class Chat
    {
        protected readonly Pages.Index index;

        public string Id { get; set; }
        public string Name { get; set; }
        public List<ChatMessage> Log { get; set; }

        public Chat(Pages.Index index)
        {
            Log = new();
            this.index = index;
        }

        public Chat(Pages.Index index, string name) : this(index)
        {
            Id = name;
            Name = name;
        }

        public async Task Speak(Nickname nickname, string text)
        {
            var message = new ChatMessage(nickname, text);
            Log.Add(message);

            if (index.CurrentChat.Id == Id)
            {
                index.OnStateHasChanged();
            }

            await index.JSRuntime.InvokeVoidAsync("scrollToBottom", Id);
        }
    }
}
