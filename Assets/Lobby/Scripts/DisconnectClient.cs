using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectClient : MonoBehaviour
{
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
