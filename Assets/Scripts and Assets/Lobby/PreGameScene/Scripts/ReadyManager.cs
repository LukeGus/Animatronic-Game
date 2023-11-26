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
    
    private NetworkVariable<float> playerCount = new NetworkVariable<float>(0f);
    private NetworkVariable<float> maxPlayerCount = new NetworkVariable<float>(0f);
    
    [SerializeField] private TMP_Text playerCountText;

    [SerializeField] private TMP_Text timerText;
    
    [SerializeField] private float countdownTimerValue;
    private bool everyoneIsReadyRunning = true;
   
    private NetworkVariable<float> timer = new NetworkVariable<float>(
        value: 60f,
        NetworkVariableReadPermission.Everyone);
        
    private bool isRunning = true;

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
        if (!IsServer)
        {
            return;
        }
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallBack;
        
        isRunning = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer)
        {
            return;
        }
        
        ReceiveVote();
    }

    public void StartGame()
    {
        // Stop the timer
        isRunning = false;
    
        // Load Map (Replace this later with the gamemode)
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
                    if (playerCount.Value == maxPlayerCount.Value)
                    {
                        Debug.Log("Everyone Is Ready");
                        
                        StartGame();
                    }    
                }
                
            }
        } else
        {
            if (timer.Value != 0)
            {
                timerText.text = timer.Value.ToString("F0");
            }
            else
            {
                timerText.text = "Starting!";
            }
                
            playerCountText.text = playerCount.Value.ToString("F0") + "/" + maxPlayerCount.Value.ToString("F0");
        }    

    }
    
    public void ReceiveVote()
    {
        if (!IsServer)
        {
            return;
        }
        
        playerCount.Value += 1;
    }
      
    private void OnClientConnnectedCallback(ulong clientId)
    {
        if (!IsServer)
        {
            return;
        }
        
        maxPlayerCount.Value += 1;
    }
    
    private void OnClientDisconnectedCallBack(ulong clientID)
    {
        if (!IsServer)
        {
            return;
        }
        
        maxPlayerCount.Value -= 1;
    }
}
