using System.Threading.Tasks;
using Reddit_App.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit_App.Models;
using Fleck;
namespace Reddit_App.Services
{
   
    public class SendNotificationServices : SendNotificationRepository
    {
        private readonly List<IWebSocketConnection> _allSockets = new List<IWebSocketConnection>();

        public async Task SendNotificationAsync(string username, int postID, List<string> lUsernames)
        {
            var notification = $"{username} vừa đăng một bài viết {postID}";

            foreach (var socket in _allSockets)
            {
                if (username.Contains(socket.ConnectionInfo.ClientIpAddress))
                {
                    await socket.Send(notification);
                }
            }
        }

        
    }
}
