using System.Threading.Tasks;

namespace Reddit_App.Services
{
    public interface SendNotificationRepository
    {
        Task SendNotificationAsync(string username, int postID, List<string> lUsernames);

    }
}
