namespace SocialNetApp.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    namespace SocialNetApp.Hubs
    {
        public class ChatHub : Hub
        {
            public static string GetGroupName(string userId1, string userId2)
            {
                return userId1.CompareTo(userId2) < 0 ? $"{userId1}-{userId2}" : $"{userId2}-{userId1}";
            }

            public async Task JoinChat(string userId, string recipientId)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(userId, recipientId));
            }
        }
    }
}
