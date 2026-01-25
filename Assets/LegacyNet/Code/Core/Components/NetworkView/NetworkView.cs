using Riptide;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyNetworking {
    public partial class NetworkView : MonoBehaviour {
        public Dictionary<IObservable, Message> Observables {
            get; internal set;
        } = new();
        public object InstantiateKey { get; internal set; }
        public int ViewID { get; internal set; } = -1;
        public int Owner { get; internal set; } = -1;
        public int Controller { get; internal set; } = -1;
        public bool IsController{
            get {
                bool value = ownershipFlag.HasFlag(OwnershipFlag.Server) || Controller < 0 && Network.Server.IsRunning;
                if (value)
                    return true;
                value = Network.Client.IsConnected ? Controller == Network.Client.Id : true;
                return value;
            }
        }
        public bool IsMine {
            get {
                bool value = ownershipFlag.HasFlag(OwnershipFlag.Server) || Owner < 0 && Network.Server.IsRunning;
                if (value)
                    return true;
                value = Network.Client.IsConnected ? Owner == Network.Client.Id : true;
                return value;
            }
        }
        public MessageSendMode reliability;
        public OwnershipFlag ownershipFlag;
        public ControllerFlag controllerFlag;
        public bool autoFindObservables = true;
        public void FindObservables() {
            if (autoFindObservables) {
                var _observables = GetComponentsInChildren<IObservable>().ToList().FindAll(x=> (x as MonoBehaviour).GetComponentInParent<NetworkView>() == this);
                foreach (var _observable in _observables) {
                    Observables.Add(_observable, Message.Create(reliability, NetworkHeaders.StreamMessage));
                }
            }
        }

        private void Update() {
            UpdateObservables();
        }

        private void UpdateObservables() {
            var _observables = Observables;
            if (_observables != Observables) {
                int i = 0;
                foreach (var item in Observables) {
                    var _observable = item.Key;
                    var _stream = IsController ? Message.Create(reliability, NetworkHeaders.StreamMessage):item.Value;
                    _observable.OnSerializeView(ref _stream, IsMine);
                    Observables[_observable] = _stream;
                    i++;
                }
            }
        }

        private void SendStreamMessage(int i) {

        }
        public enum OwnershipFlag : byte {
            Fixed,
            Shared,
            Server,
        }
        public enum ControllerFlag : byte {
            Owner,
            Shared,
            Server,
        }
    }   
}
