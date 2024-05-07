using System.Net.WebSockets;
using System.Text;

namespace Reddit_App.Websockets
{
    public class WebSocketHandler
    {
        private List<WebsocketConnection> _connections = new List<WebsocketConnection>();

        public async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            var connection = new WebsocketConnection { webSocket = webSocket };
            _connections.Add(connection);
            try
            {
                await HandleConnectionAsync(connection);
            }
            finally
            {
                _connections.Remove(connection);
            }
        }

        private async Task HandleConnectionAsync(WebsocketConnection connection)
        {
            var buffer = new byte[1024 * 4];
            while(connection.webSocket.State == WebSocketState.Open)
            {
                var result = await connection.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                //xử lý thông điệp nhận được

                // gửi thông điệp tới tất cả các kết nối websocket

                foreach(var conn in _connections)
                {
                    //if(conn.webSocket.State == WebSocketState.Open)
                    //{
                    //    await conn.webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    //}
                    if (IsTrangUser(conn))
                    {
                        //await conn.webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                        await conn.webSocket.SendAsync(Encoding.UTF8.GetBytes("New post created"), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }

        private bool IsTrangUser(WebsocketConnection connection)
        {
            var connect = "http://127.0.0.1:5500/pages/Tranguser.html";

            if(connect.EndsWith("/pages/Tranguser.html", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }    
            return false;
        }
    }
}
