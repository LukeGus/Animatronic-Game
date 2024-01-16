using UnityEngine;
using Unity.Netcode;

public class NetworkDisabler : NetworkBehaviour
{
    [SerializeField] private GameObject cameraHolder;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        cameraHolder.SetActive(IsOwner);
    }
}
