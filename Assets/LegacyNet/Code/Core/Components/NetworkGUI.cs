using UnityEngine;
using Network = LegacyNetworking.Network;

public class NetworkGUI : MonoBehaviour
{
    private string _address = "127.0.0.1";
    private string _port = "7777";
    private void OnGUI() {
        if(GUILayout.Button("Start"))
            Network.NetStart(ushort.Parse(_port));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect"))
            Network.NetConnect(ushort.Parse(_port), _address);
        _address = GUILayout.TextField(_address);
        _port = GUILayout.TextField(_port);
        GUILayout.EndHorizontal();
        if(GUILayout.Button("Disconnect"))
            Network.NetShutdown();
        GUILayout.Box($"Is Server: {Network.isServer}");
        GUILayout.Box($"Is Client: {Network.isClient}");
    }
}
