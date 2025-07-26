using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using SimpleJSON;

namespace H3Status.Server
{

    internal static class ServerManager
    {
        private static WebSocket _client;
        private static WebSocketServer _server;

        public static void Start()
        {
            if (Config.Connection.useExternalAddress.Value)
            {
                string address = "wss://" + Config.Connection.webSocketAddress.Value;
                _client = new WebSocket(address);
                _client.EnableRedirection = true;

                _client.OnOpen += (sender, e) => {
                    Plugin.Logger.LogInfo($"Connected to address {address}");
                };
                _client.OnClose += (sender, e) => {
                    Plugin.Logger.LogInfo($"Connection to {address} closed");
                    Plugin.Logger.LogInfo(e.Code);
                    Plugin.Logger.LogInfo(e.Reason);
                };
                _client.OnError += (sender, e) => {
                    Plugin.Logger.LogError($"Failed to connect to address {address}");
                };

                _client.Connect();

                if (_client.IsAlive)
                {
                    var eventJSON = new JSONObject();
                    eventJSON["type"] = "hello";
                    eventJSON["status"] = Patches.VersionHandler.GetVersionInfo();

                    _client.SendAsync(eventJSON.ToString(), null);
                }
            }

            else
            {
                int port = Config.Connection.webSocketPort.Value;
                _server = new WebSocketServer(port);
                _server.AddWebSocketService<Server.ServerBehavior>("/");
                _server.Start();

                Plugin.Logger.LogInfo($"Server started on port {port}");
            }

        }

        public static void Stop()
        {
            if (_client != null){
                _client.Close();
                _client = null;
            }
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }

        public static void Send(JSONObject json)
        {
            if (_client != null && _client.IsAlive)
            {
                _client.SendAsync(json.ToString(), null);
            }

            if (_server != null && _server.IsListening)
            {
                ServerBehavior.SendMessage(json);
            }
        }
    }

    internal class ServerBehavior : WebSocketBehavior
    {
        private static List<ServerBehavior> _instances = new List<ServerBehavior>();

        protected override void OnOpen()
        {
            base.OnOpen();
            _instances.Add(this);

            var eventJSON = new JSONObject();
            eventJSON["type"] = "hello";
            eventJSON["status"] = Patches.VersionHandler.GetVersionInfo();

            this.SendAsync(eventJSON.ToString(), null);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _instances.Remove(this);
            base.OnClose(e);
        }

        public static void SendMessage(JSONObject json) {
            foreach (var instance in _instances) {
                instance.SendAsync(json.ToString(), null);
            }
        }
    }

}
