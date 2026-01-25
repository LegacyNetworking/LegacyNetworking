using Riptide;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public interface INetworkPrefabPool
    {
        public GameObject Instantiate(object key, Vector3 position = default, Quaternion rotation = default) => Instantiate(key, position, rotation, Network.Client.Id);
        public GameObject Instantiate(object key, Vector3 position = default, Quaternion rotation = default, int owner = (int)NetworkHeaders.ServerId);
        public void Destroy(object instance, float timer = 0);
        public void OnNewScene(Scene scene);
        public void OnEnable();
        public void OnDisable();
        public void InstantiateToConnection(ushort connection, Message spawnMessage);
    }
}
