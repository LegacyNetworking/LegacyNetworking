using Riptide;
using UnityEngine;

namespace LegacyNetworking
{
    public class NetworkTransformView : MonoBehaviour, IObservable
    {
        public bool syncScale = false;
        public float teleportDistance = 3;
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private Vector3 networkScale;
        private bool _isWriting;

        void Awake() {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
            networkScale = transform.localScale;
        }

        void Update() {
            if (_isWriting)
                return;
            transform.position = networkPosition;
            transform.rotation = networkRotation;
            if (syncScale)
                transform.localScale = networkScale;
        }

        public void OnSerialize(Message stream) {
            stream.Add(transform.position);
            stream.Add(transform.rotation);
            if (syncScale)
                stream.Add(transform.localScale);
        }

        public void OnDeserialize(Message stream) {
            networkPosition = stream.GetVector3();
            networkRotation = stream.GetQuaternion();
            if (syncScale)
                networkScale = stream.GetVector3();

            if (Vector3.Distance(transform.position, networkPosition) > teleportDistance) {
                transform.position = networkPosition;
                transform.rotation = networkRotation;
                if (syncScale)
                    transform.localScale = networkScale;
                return;
            }
        }
    }
}
