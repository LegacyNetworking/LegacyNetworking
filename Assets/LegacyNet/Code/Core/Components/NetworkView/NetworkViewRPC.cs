using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LegacyNetworking
{
    public delegate void RPCDelegate(short sender, Message data);
    public partial class NetworkView : MonoBehaviour
    {
        public Dictionary<byte, RPCDelegate> RpcMethods { get; set; } = new();
        private byte nextRpcId = 0;
        public void RegisterRPC(RPCDelegate method) {
            if (RpcMethods.ContainsValue(method)) {
                Debug.LogWarning($"[NetworkView] RPC method {method.Method.Name} is already registered on NetworkView attached to GameObject {gameObject.name}");
                return;
            }
            RpcMethods.Add(nextRpcId, method);
            nextRpcId += 1;
        }
        public byte GetRpcId(RPCDelegate method) => RpcMethods.FirstOrDefault(x => x.Value == method).Key;
        public void UnregisterRPC(RPCDelegate method) {
            if(RpcMethods.ContainsValue(method)) {
                RpcMethods.Remove(GetRpcId(method));
            } else {
                Debug.LogWarning($"[NetworkView] RPC method {method.Method.Name} is not registered on NetworkView attached to GameObject {gameObject.name}");
            }
        }
        public void InvokeRPC(RPCDelegate method, NetworkTarget target, bool buffered, Message data, MessageSendMode channel) {
            var rpcMessage = Message.Create(MessageSendMode.Reliable, NetworkMessages.RpcMessage);
            RPCInfo info = new();
            info.ViewId = GetRpcId(method);
            info.RpcId = GetRpcId(method);
            info.Target = target;
            info.Buffered = buffered;
            info.Data = data;
            rpcMessage.Add(info);
            Network.Send(rpcMessage);
        }
        /*
        public void InvokeRPC(RPCDelegate method, short target, bool buffered, Message data, MessageSendMode channel) {
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

    internal struct RPCInfo : IMessageSerializable {
        public int ViewId {
            get; set;
        }
        public byte RpcId {
            get; set;
        } 
        public NetworkTarget Target {
            get; set;
        }
        public short Player {
            get; set;
        }
        public bool Buffered {
            get; set;
        }
        public Message Data {
            get; set;
        }
        public void Deserialize(Message message) {
            ViewId = message.GetInt();
            RpcId = message.GetByte();
            Target = (NetworkTarget)message.GetByte();
            Player = message.GetShort();
            Buffered = message.GetBool();
            Data = message.GetMessage();
        }

        public void Serialize(Message message) {
            message.Add(ViewId);
            message.Add(RpcId);
            message.Add((byte)Target);
            message.Add(Player);
            message.Add(Buffered);
            message.Add(Data);
        }
    }
}
