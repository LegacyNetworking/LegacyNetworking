using Riptide;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LegacyNetworking
{
    public struct RPCMessage : IMessageSerializable {
        public int ViewId {
            get; set;
        }
        public byte RpcId {
            get; set;
        }
        public NetworkTarget Target {
            get; set;
        }
        public short PlayerTarget {
            get; set;
        }
        public bool Buffered {
            get; set;
        }
        public RPCInfo Info {
            get; set;
        }
        public void Deserialize(Message message) {
            ViewId = message.GetInt();
            RpcId = message.GetByte();
            Target = (NetworkTarget)message.GetByte();
            PlayerTarget = message.GetShort();
            Buffered = message.GetBool();
            Info = message.GetSerializable<RPCInfo>();
        }

        public void Serialize(Message message) {
            message.Add(ViewId);
            message.Add(RpcId);
            message.Add((byte)Target);
            message.Add(PlayerTarget);
            message.Add(Buffered);
            message.Add(Info);
        }
    }
    public struct RPCInfo : IMessageSerializable {
        public ushort Sender {
            get; internal set;
        }
        public object[] Data {
            get; internal set;
        }
        public void Serialize(Message message) {
            message.Add(Sender);
            message.Add(Data);
        }
        public void Deserialize(Message message) {
            Sender = message.GetUShort();
            Data = message.GetObjectArray();
        }
    }
}
