using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class NetworkDisabler : NetworkBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject cameraHolder;
    
    public override void OnNetworkSpawn()
    {
        cameraHolder.SetActive(IsOwner);
        base.OnNetworkSpawn();
        
        if (!IsOwner)
        {
            playerInput.enabled = false;
            inputSystemUIInputModule.enabled = false;
            eventSystem.enabled = false;
        }    
    }
}
