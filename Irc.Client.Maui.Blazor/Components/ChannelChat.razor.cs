using Irc.Client.Maui.Blazor.Models;
using Microsoft.AspNetCore.Components;

namespace Irc.Client.Maui.Blazor.Components
{
    public partial class ChannelChat
    {
        [Parameter]
        public IEnumerable<ChatMessage> Messages { get; set; }

        [Parameter]
        public UserCollection Users { get; set; }
    }
}
