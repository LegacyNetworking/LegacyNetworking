using Riptide;
using System.Net.Sockets;
using UnityEngine;

namespace LegacyNetworking
{
    public class NetworkRigidbodyView : NetworkTransformView
    {
        private Rigidbody m_Rigidbody;
        private void Awake() {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
