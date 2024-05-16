using AutoMapper.Execution;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Reddit_App.Helpers.SocketHelper
{
    public class ConnectionManager
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private static ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

        public WebSocket GetSocketByID(string id)
        {
            return _sockets.FirstOrDefault(s => s.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets() 
        {
            return _sockets;
        }

        public List<string> GetAllUserName()
        {
            return _users.Select(p => p.Key).ToList();
        }
        public string GetID(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public string GetUserNameBySocketID(string socketID)
        {
            return _users.FirstOrDefault(p => p.Value == socketID).Key;
        }

        public string GetuernameBySocket(WebSocket socket)
        {
            string socketID = GetID(socket);
            return GetUserNameBySocketID(socketID);
        }

        public void AddSocket(WebSocket socket)
        {
            string socketID = CreateConnectionID();
            _sockets.TryAdd(socketID, socket);
        }

        public void AddUser(WebSocket socket, string username)
        {
            string socketid = GetID(socket);
            _users.TryAdd(username, socketid);
        }

        public async Task RemoveSocket(WebSocket socket, string description = "Connection closed")
        {
            string id = GetID(socket);
            if (!string.IsNullOrEmpty(id))
            {
                _sockets.TryRemove(id, out _);
            }

            if (socket.State != WebSocketState.Aborted)
            {
                await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, statusDescription: description, cancellationToken: CancellationToken.None);
            }
        }

        public void RemoveUser(string username)
        {
            _users.TryRemove(username, out _);
        }

        public bool UsernameAlreadyExists(string username)
        {
            return _users.ContainsKey(username);
        }
       
        private string CreateConnectionID()
        {
            return Guid.NewGuid().ToString();
        }

        // get value

        public bool TryGetValue<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public bool TryGetValueFromListConnect(string key, out WebSocket value)
        {
            return _sockets.TryGetValue(key, out value);
        }

        public bool TryGetValueFromUser(string key, out string value)
        {
            return _users.TryGetValue(key, out value);
        }
    }
}
