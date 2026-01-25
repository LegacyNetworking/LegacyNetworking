using UnityEngine;

namespace LegacyNetworking
{
    public class NetworkMono : MonoBehaviour
    {
        private void Update() {
            Network.Client.Update();
            if(Network.Server.IsRunning) Network.Server.Update();
        }
    }
}
