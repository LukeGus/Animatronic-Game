using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using TMPro;

public class ReadyManager : NetworkBehaviour 
{
    public static ReadyManager Instance { get; private set; }
    
    private NetworkVariable<float> playerReadyCount = new NetworkVariable<float>(0f);
    private NetworkVariable<float> maxPlayerReadyCount = new NetworkVariable<float>(0f);
    
    private NetworkVariable<float> timer = new NetworkVariable<float>(
            value: 60f,
            NetworkVariableReadPermission.Everyone);
    
    [SerializeField] private TMP_Text playerReadyCountText;
    [SerializeField] private TMP_Text timerText;
    
    [SerializeField] private float countdownTimerValue;
        
    private bool isRunning = true;
    private bool everyoneIsReadyRunning;

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
        
        if (!IsServer)
        {
            return;
        }
        
        isRunning = true;
        everyoneIsReadyRunning = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        playerReadyCount.Value += 1;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SendPlayerConnectedServerRpc(ServerRpcParams rpcParams = default)
    {
        maxPlayerReadyCount.Value += 1;
    }

    public void StartGame()
    {
        isRunning = false;
        everyoneIsReadyRunning = false;
        
        Loader.LoadNetwork("GameScene");
    }

    private void Update()
    {
        if (IsServer)
        {
            if (isRunning)
            {
                timer.Value -= Time.deltaTime;
    
                if (timer.Value <= 0f)
                {
                    StartGame();
                }
            }
            
            if (everyoneIsReadyRunning == true)
            {
                countdownTimerValue -= Time.deltaTime;
    
                if (countdownTimerValue <= 0f)
                {
                    if (playerReadyCount.Value == maxPlayerReadyCount.Value)
                    {
                        StartGame();
                    }    
                }
                
            }
        }
        
        if (timer.Value != 0)
        {
            timerText.text = timer.Value.ToString("F0");
        }
        else
        {
            timerText.text = "Starting!";
        }
        
        playerReadyCountText.text = playerReadyCount.Value.ToString("F0") + "/" + maxPlayerReadyCount.Value.ToString("F0");
    }
    
    private void OnClientDisconnectedCallBack(ulong clientID)
    {
        if (!IsServer)
        {
            return;
        } else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallBack;
            
            Loader.Load("LobbyScene");
        }
        
        maxPlayerReadyCount.Value += 1;
    }
}
