using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyNetworking.Extras
{
    public class BallsStateGUI : MonoBehaviour, IObservable
    {
        private string stateText = "No State";
        public List<Transform> balls = new();
        private void Update() {
            if (!Network.isServer) {
                return;
            }
            stateText = "Balls State:\n";
            foreach (Transform t in balls) {
                stateText += $"{t.name}: {t.position}, {t.rotation}\n";
            }
        }
        private void OnGUI() {
            GUILayout.Box("Balls State Example");
            GUILayout.Box("This example shows how to sync the state of moving balls.");
            GUILayout.Box("Open multiple instances to see the synchronization in action.");
            GUILayout.Box(stateText);
        }

        public void OnSerialize(Message stream) {
            stream.Add(stateText);
        }

        public void OnDeserialize(Message stream) {
            stateText = stream.GetString();
        }
    }
}
