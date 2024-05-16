using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Reddit_App.Helpers.SocketHelper
{
    public class WebSocketMiddlewareCustom
    {
        private readonly RequestDelegate _next;
        private WebSocketHandle _webSocketHandle { get; set; }


        public WebSocketMiddlewareCustom(RequestDelegate next, WebSocketHandle webSocketHandle)
        {
            _next = next;
            _webSocketHandle = webSocketHandle;
        }

        public async Task Invoke(HttpContext context)
        {
            if(!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }
            string username = context.Request.Query["username"];
            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();

            await _webSocketHandle.OnConnected(socket, username);
            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string msg = _webSocketHandle.ReceiveString(result, buffer);
                    JObject jsonMessage = JObject.Parse(msg);
                    int UserID = (int)jsonMessage["UserID"];
                    int PostID = (int)jsonMessage["PostID"];
                    JArray jsonUserName = (JArray)jsonMessage.SelectToken("listUserName");
                    List<string> userNames = jsonUserName.Select(x => x.ToString()).ToList();
                    await _webSocketHandle.Broadcast(UserID, PostID, userNames, socket);
                }
                else if (result.MessageType == WebSocketMessageType.Close || socket.State == WebSocketState.Aborted)
                {
                    await HandleDisconnect(socket);
                    return;
                }
            });
        }


        
        private async Task HandleDisconnect(WebSocket socket)
        {
            await _webSocketHandle.OnDisconnected(socket);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                           cancellationToken: CancellationToken.None);

                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await HandleDisconnect(socket);
            }
        }
    }
               
}
