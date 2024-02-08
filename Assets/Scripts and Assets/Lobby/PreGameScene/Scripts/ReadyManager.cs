using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Michsky;
using Michsky.MUIP;

public class ReadyManager : NetworkBehaviour 
{
    public static ReadyManager Instance { get; private set; }
    
    public NetworkVariable<int> playerReadyCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> maxPlayerReadyCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    
    private NetworkVariable<float> mainTimer = new NetworkVariable<float>(
            value: 60f,
            NetworkVariableReadPermission.Everyone);
    
    private NetworkVariable<float> initialTimer = new NetworkVariable<float>(
                value: 10f,
                NetworkVariableReadPermission.Everyone);
    
    [SerializeField] private float mainTimerMax;
    [SerializeField] private float initialTimerMax;
    
    [SerializeField] private TMP_Text playerReadyCountText;
    [SerializeField] private TMP_Text mainTimerText;
    
    private bool mainTimerIsRunning = false;
    private bool initialTimerIsRunning = false;
    
    private bool hasReadyed = false;
    
    [SerializeField] private ButtonManager readyButton;
    [SerializeField] private Button leaveLobbyButton;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallBack;

        readyButton.onClick.AddListener(Vote);
        leaveLobbyButton.onClick.AddListener(Leave);
    }

    public IEnumerator SetUpGame()
    {
        yield return new WaitForSeconds(2f);

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Host");

            if (!NetworkObject.IsSpawned)
                NetworkObject.Spawn();

            mainTimer.Value = mainTimerMax;
            initialTimer.Value = initialTimerMax;

            mainTimerIsRunning = true;
            initialTimerIsRunning = true;

            maxPlayerReadyCount.Value = LobbyManager.Instance.playerCount;
        }
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
            SendReadyServerRpc();
            
            hasReadyed = true;
            
            Debug.Log("Ready");
            
            readyButton.Interactable(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        playerReadyCount.Value += 1;
        
        Debug.Log("Player Ready");
    }

    public void StartGame()
    {
        mainTimerIsRunning = false;
        initialTimerIsRunning = false;
        
        Debug.Log("Starting Game");
        
        RoleSelectManager.Instance.StartSelectionProcess();
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (mainTimerIsRunning)
            {
                mainTimer.Value -= Time.deltaTime;
    
                if (mainTimer.Value <= 0f)
                {
                    StartGame();
                }
            }
            
            if (initialTimerIsRunning)
            {
                initialTimer.Value -= Time.deltaTime;
    
                if (initialTimer.Value <= 0f)
                {
                    if (playerReadyCount.Value == maxPlayerReadyCount.Value)
                    {
                        StartGame();
                    }    
                }
                
            }
        }
        
        if (mainTimer.Value != 1)
        {
            mainTimerText.text = mainTimer.Value.ToString("F0");
        }
        else
        {
            mainTimerText.text = "Starting!";
        }
        
        playerReadyCountText.text = playerReadyCount.Value.ToString("F0") + "/" + maxPlayerReadyCount.Value.ToString("F0");
    }
    
    private void OnClientDisconnectedCallBack(ulong clientID)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallBack;

            Loader.Load("LobbyScene");
            
            return;
        }
        
        if (maxPlayerReadyCount != null)
        {
            maxPlayerReadyCount.Value -= 1;
        }
        
        Debug.Log("Player Disconnected");

        if (maxPlayerReadyCount.Value == 1)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallBack;

            Loader.Load("LobbyScene");
        }    
    }
}
