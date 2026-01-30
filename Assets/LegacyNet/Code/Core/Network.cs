using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyNetworking {
    public static partial class Network {
        [RuntimeInitializeOnLoadMethod]
        public static void Start() {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            localServer = new("LEGACYNET_SERVER");
            localServer.ClientConnected += Server_ClientConnected;
            localClient = new("LEGACYNET_CLIENT");
            NetworkMono.DontDestroyOnLoad(new GameObject("Network [Mono]", typeof(NetworkMono)));

            SceneManager = new LevelNetSceneManager();
            PrefabPool = new ResourceNetworkPrefabPool();
        }

        private static void Server_ClientConnected(object sender, ServerConnectedEventArgs e) {
            var connection = e.Client.Id;
            foreach(var message in bufferedMessages) {
                localServer.Send(message, connection);
            }
        }

        private static INetworkPrefabPool prefabPool;
        public static INetworkPrefabPool PrefabPool {
            get => prefabPool; set {
                prefabPool?.OnDisable();
                prefabPool = value;
                prefabPool?.OnEnable();
            }
        }
        private static INetSceneManager sceneManager;
        public static INetSceneManager SceneManager {
            get => sceneManager; set {
                sceneManager = value;
            }
        }
        public static bool isServer {
            get {
                return localServer != null ? localServer.IsRunning : false;
            }
        }
        public static bool isClient {
            get {
                return localClient != null ? localClient.IsConnected : false;
            }
        }
        public static short localPlayer {
            get{
                return (short)(isClient ? localClient.Id : -99);
            }
        }
        public static Server localServer {
            get; private set;
        }
        public static Client localClient {
            get; private set;
        }
        public static void NetShutdown() {
            localClient.Disconnect();
            localServer.Stop();
        }
        public static void NetStart(ushort port = 8778, ushort maxClients = 16) {
            localServer.Start(port, maxClients);
        }
        public static bool NetStartHost(ushort port = 8778, ushort maxClients = 16) {
            NetStart(port, maxClients);
            bool started = NetConnect("127.0.0.1", port);
            if (!started)
                started = NetConnect("127.0.0.1");
            return started;
        }
        public static bool NetConnect(ushort port, string address, ushort maxConnectionAttempts = 5) {
            return NetConnect($"{address}:{port}", maxConnectionAttempts);
        }
        public static bool NetConnect(string address, ushort maxConnectionAttempts = 5) {
            return localClient.Connect(address, maxConnectionAttempts);
        }
        public static List<Message> bufferedMessages = new();
        /// <summary>
        /// Send message from server to clients
        /// </summary>
        public static void SendToAll(Message message, short exceptPlayer = -1, bool asHost = false, bool buffered = false) {
            if (!isServer)
                return;
            if (asHost) {
                foreach (var connection in localServer.Clients) {
                    if(connection.Id == localPlayer || connection.Id == exceptPlayer)
                        continue;
                    localServer.Send(message, connection);
                }
            }
            else {
                if(exceptPlayer < 0)
                    localServer.SendToAll(message);
                else
                    localServer.SendToAll(message, (ushort)exceptPlayer);
            }
            if(!bufferedMessages.Contains(message)) bufferedMessages.Add(message);
        }
        /// <summary>
        /// Send message from server to a client
        /// </summary>
        public static void Send(Message message, ushort target) {
            if (!isServer)
                return;
            localServer.Send(message, target);
        }
        /// <summary>
        /// Send message from client to server
        /// </summary>
        public static void Send(Message message) {
            if(!isClient)
                return;
            localClient.Send(message);
        }
    }
}