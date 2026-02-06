using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LegacyNetworking
{
    public delegate void RPCDelegate(RPCInfo rpc);
    public partial class NetworkView : MonoBehaviour
    {
        private Dictionary<byte, RPCDelegate> RpcMethods { get; set; } = new();
        private byte nextRpcId = 0;
        public byte RegisterRPC(RPCDelegate method) {
            if (RpcMethods.ContainsValue(method)) {
                Debug.LogWarning($"[NetworkView] RPC method {method.Method.Name} is already registered on NetworkView attached to GameObject {gameObject.name}");
                return GetRpcId(method);
            }
            var id = nextRpcId;
            RpcMethods.Add(id, method);
            nextRpcId += 1;
            return id;
        }
        public byte GetRpcId(RPCDelegate method) => RpcMethods.FirstOrDefault(x => x.Value == method).Key;
        public void UnregisterRPC(RPCDelegate method) {
            if(RpcMethods.ContainsValue(method)) {
                RpcMethods.Remove(GetRpcId(method));
            } else {
                Debug.LogWarning($"[NetworkView] RPC method {method.Method.Name} is not registered on NetworkView attached to GameObject {gameObject.name}");
            }
        }
        public void SendRPC(byte rpcId, NetworkTarget target, short playerTarget = -1, bool buffered = false, MessageSendMode channel = MessageSendMode.Unreliable, params object[] args) {
            var message = Message.Create(MessageSendMode.Reliable, NetworkMessages.RpcMessage);
            RPCMessage rpcMessage = new();
            rpcMessage.Info = new() {
                Sender = (ushort)Network.localPlayer,
                Data = args
            };
            rpcMessage.ViewId = viewId;
            rpcMessage.RpcId = rpcId;
            rpcMessage.Target = target;
            rpcMessage.PlayerTarget = playerTarget;
            rpcMessage.Buffered = buffered;            
            message.Add(rpcMessage);
            Network.Send(message);
        }

        [MessageHandler((ushort)NetworkMessages.RpcMessage)]
        public static void Server_Rpc(ushort sender, Message received) {
            var cache = Message.Create().AddMessage(received);
            var message = cache.GetSerializable<RPCMessage>();
            switch (message.Target) {
                case NetworkTarget.All | NetworkTarget.Others:
                    bool others = message.Target == NetworkTarget.Others;
                    foreach (var client in Network.localServer.Clients) {
                        if (others)
                            Network.SendToAll(received, (short)client.Id);
                        else
                            Network.SendToAll(received);
                    }
                    break;
                case NetworkTarget.Targeted:
                    Network.Send(received, (ushort)message.PlayerTarget);
                    break;
                case NetworkTarget.Server:
                    HandleRpc(message);
                    break;
            }
        }

        [MessageHandler((ushort)NetworkMessages.RpcMessage)]
        public static void Client_Rpc(Message received) {
            HandleRpc(received.GetSerializable<RPCMessage>());
        }

        private static void HandleRpc(RPCMessage rpcMessage) {
            var view = Network.Views[(ushort)rpcMessage.ViewId];
            view.RpcMethods[rpcMessage.RpcId].Invoke(rpcMessage.Info);
        }
        /*
        public void SendRPC(RPCDelegate method, short target, bool buffered, Message data, MessageSendMode channel) {
            var rpcMessage = Message.Create(MessageSendMode.Reliable, NetworkMessages.RpcMessage);
            RPCInfo info = new();
            info.ViewId = GetRpcId(method);
            info.RpcId = GetRpcId(method);
            info.Target = target;
            info.Buffered = buffered;
            info.Data = data;
            rpcMessage.Add(info);
            Network.Send(rpcMessage);
        }*/
    }
}
