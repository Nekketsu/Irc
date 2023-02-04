using Microsoft.AspNetCore.Components;

namespace Irc.Client.Maui.Blazor.Components
{
    public partial class UserList
    {
        [Parameter]
        public UserCollection Users { get; set; }
    }
}
