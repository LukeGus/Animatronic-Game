using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ClientReady : MonoBehaviour 
{
    private bool hasReadyed = false;
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button leaveLobbyButton;
    
    private void Start()
    {
        readyButton.onClick.AddListener(Vote);
        leaveLobbyButton.onClick.AddListener(Leave);
        
        ReadyManager.Instance.SendPlayerConnectedServerRpc();
    }
    
    public void Leave()
    {
        LobbyManager.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        
        Loader.Load("LobbyScene");
    }

    public void Vote()
    {
        if (!hasReadyed)
        {
            ReadyManager.Instance.SendReadyServerRpc();
            
            hasReadyed = true;
            
            Debug.Log("Ready");
        }
    }
}