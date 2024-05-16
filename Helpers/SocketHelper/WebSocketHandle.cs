using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Reddit_App.Helpers.SocketHelper
{
    public abstract class WebSocketHandle
    {
        protected ConnectionManager ConnectionManager { get; set; }

        public WebSocketHandle(ConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket, string username)
        {
            string connectionError = ValidateUsername(username);

            if (!string.IsNullOrEmpty(connectionError))
            {
                await ConnectionManager.RemoveSocket(socket, connectionError);
            }
            else
            {
                ConnectionManager.AddSocket(socket);
                ConnectionManager.AddUser(socket, username);

                //ServerMessage connectMessage = new ServerMessage(username, false, GetAllUsers());
                //await BroadcastMessage(JsonSerializer.Serialize(connectMessage));
            }
        }

        public string ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return $"Username must not be empty";
            }

            if (ConnectionManager.UsernameAlreadyExists(username))
            {
                return $"User {username} already exists";
            }

            return null;
        }

        public virtual async Task<string> OnDisconnected(WebSocket socket)
        {
            string socketId = ConnectionManager.GetID(socket);
            await ConnectionManager.RemoveSocket(socket);

            string username = ConnectionManager.GetUserNameBySocketID(socketId);
            ConnectionManager.RemoveUser(username);

            return username;
        }

        public async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        public async Task Broadcast(int userID, int postID, List<string> username, WebSocket sockets)
        {
            foreach (var item in username)
            {
                if (ConnectionManager.TryGetValueFromUser(item, out var receiverSocketID) && ConnectionManager.TryGetValueFromListConnect(receiverSocketID, out var receiverSocket))
                {

                    var messageObject = new
                    {
                        UserID = userID,
                        PostID = postID,
                        Message = $"Hello {item}!"
                    };
                    var json = JsonConvert.SerializeObject(messageObject);
                    var bytes = Encoding.UTF8.GetBytes(json);
                    await receiverSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public string ReceiveString(WebSocketReceiveResult result, byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
    }
}
