using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class ReadyManager : NetworkBehaviour 
{
    public static ReadyManager Instance { get; private set; }
    
    private NetworkVariable<float> playerReadyCount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> maxPlayerReadyCount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    
    private NetworkVariable<float> mainTimer = new NetworkVariable<float>(
            value: 60f,
            NetworkVariableReadPermission.Everyone);
    
    private float secondaryTimer;
    
    [SerializeField] private float mainTimerMax;
    [SerializeField] private float secondaryTimerMax;
    
    [SerializeField] private TMP_Text playerReadyCountText;
    [SerializeField] private TMP_Text mainTimerText;
        
    private bool isRunning = false;
    private bool everyoneIsReadyRunning = false;
    
    private bool hasReadyed = false;
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button leaveLobbyButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallBack;
        
        readyButton.onClick.AddListener(Vote);
        leaveLobbyButton.onClick.AddListener(Leave);
        
        SendPlayerConnectedServerRpc();
        
        this.NetworkObject.Spawn();
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
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        playerReadyCount.Value += 1;
        
        Debug.Log("Player ready");
             
        if (!isRunning)
        {
            isRunning = true;
            mainTimer.Value = mainTimerMax;
        }
        
        if (!everyoneIsReadyRunning)
        {
            everyoneIsReadyRunning = true;
            secondaryTimer = secondaryTimerMax;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SendPlayerConnectedServerRpc(ServerRpcParams rpcParams = default)
    {
        maxPlayerReadyCount.Value += 1;
        
        Debug.Log("Player connected");
    }

    public void StartGame()
    {
        isRunning = false;
        everyoneIsReadyRunning = false;
        
        Debug.Log("Starting game");
        
        Loader.LoadNetwork("GameScene");
    }

    private void Update()
    {
        if (IsHost)
        {
            if (isRunning)
            {
                mainTimer.Value -= Time.deltaTime;
    
                if (mainTimer.Value <= 0f)
                {
                    StartGame();
                }
            }
            
            if (everyoneIsReadyRunning)
            {
                secondaryTimer -= Time.deltaTime;
    
                if (secondaryTimer <= 0f)
                {
                    if (playerReadyCount.Value == maxPlayerReadyCount.Value)
                    {
                        StartGame();
                    }    
                }
                
            }
        }
        
        if (mainTimer.Value != 0)
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
        if (!IsHost)
        {
            return;
        } else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallBack;
            
            Loader.Load("LobbyScene");
        }
        
        maxPlayerReadyCount.Value -= 1;
        
        Debug.Log("Player disconnected");
    }
}
