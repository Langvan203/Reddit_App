using static Microsoft.AspNetCore.Http.WebSocketManager;

namespace Reddit_App.Helpers.SocketHelper
{
    public class SendNotiHandler : WebSocketHandle
    {
        public SendNotiHandler(ConnectionManager connectionManager) : base(connectionManager) 
        {
        }

     }
}
