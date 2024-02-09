using UnityEngine;
using Unity.Netcode;

public class NetworkDisabler : NetworkBehaviour
{
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private EventSystem eventSystem;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        cameraHolder.SetActive(IsOwner);
        
        if (!IsOwner)
        {
            Destroy(eventSystem);
        }
    }
}
