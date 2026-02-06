using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyNetworking.Extras
{
    public class BallsStateGUI : MonoBehaviour, IObservable
    {
        public NetworkView NetworkView;
        private string stateText = "No State";
        public List<Transform> balls = new();
        public string ballKey;
        public Transform spawn;
        byte spawnRpc;
        void Awake() {
            spawnRpc = NetworkView.RegisterRPC(SpawnBall);
        }

        private void SpawnBall(RPCInfo rpc) {
            if (!Network.isServer)
                return;
            Debug.Log($"Spawning Ball from Player:{rpc.Sender}");
            Network.Instantiate(ballKey, spawn.position, spawn.rotation);
        }

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
            if (GUILayout.Button("Spawn Ball")) {
                NetworkView.SendRPC(spawnRpc, NetworkTarget.Server, channel: MessageSendMode.Reliable);
            }
        }

        public void OnSerialize(Message stream) {
            stream.Add(stateText);
        }

        public void OnDeserialize(Message stream) {
            stateText = stream.GetString();
        }
    }
}
