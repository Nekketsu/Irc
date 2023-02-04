using Irc.Client.Maui.Blazor.Models;
using Microsoft.AspNetCore.Components;

namespace Irc.Client.Maui.Blazor.Components
{
    public partial class Chat
    {
        [Parameter]
        public IEnumerable<ChatMessage> Messages { get; set; }
    }
}
