using Microsoft.AspNetCore.SignalR;
using Reddit_App.Request;
using Reddit_App.Services;

namespace SignalRChat.Hubs
{
    public class DetectNewEvent : Hub
    {
        private readonly NotificationServices _notificationServices;

        public DetectNewEvent(NotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
        }

        public async Task SendNotification(string Message)
        {
            //await Clients.All.SendAsync("SendNoti", UserID, Content);
            //Console.WriteLine("Add new record to database");
            await Clients.All.SendAsync("ReceiveNotification", Message);
        }
    }
}
