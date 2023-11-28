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
    
    private NetworkVariable<int> playerReadyCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<int> maxPlayerReadyCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    
    private NetworkVariable<float> mainTimer = new NetworkVariable<float>(
            value: 60f,
            NetworkVariableReadPermission.Everyone);
    
    private NetworkVariable<float> secondaryTimer = new NetworkVariable<float>(
                value: 10f,
                NetworkVariableReadPermission.Everyone);
    
    [SerializeField] private float mainTimerMax;
    [SerializeField] private float secondaryTimerMax;
    
    [SerializeField] private TMP_Text playerReadyCountText;
    [SerializeField] private TMP_Text mainTimerText;
        
    private bool mainTimerIsRunning = false;
    private bool secondaryTimerIsRunning = false;
    
    private bool hasReadyed = false;
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button leaveLobbyButton;
    
    [SerializeField] private GameObject playerReadyObject1;
    [SerializeField] private GameObject playerReadyObject2;
    [SerializeField] private GameObject playerReadyObject3;
    [SerializeField] private GameObject playerReadyObject4;

    [SerializeField] private GameObject playerSelectObject1;

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
        
        StartCoroutine(SetUpGame());
    }

    private IEnumerator SetUpGame()
    {
        yield return new WaitForSeconds(5f);

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Host");

            this.NetworkObject.Spawn();

            mainTimer.Value = mainTimerMax;
            secondaryTimer.Value = secondaryTimerMax;

            mainTimerIsRunning = true;
            secondaryTimerIsRunning = true;

            maxPlayerReadyCount.Value = LobbyManager.Instance.playerCount;

            // Make this not instantiate them all in the same spot

            /*
            for (int i = 0; i < maxPlayerReadyCount.Value; i++)
            {
                GameObject playerObject = Instantiate(playerReadyObject1, playerReadyObject1.transform.position,
                    playerReadyObject1.transform.rotation);

                playerObject.GetComponent<NetworkObject>().Spawn();
            }
            */
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
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        playerReadyCount.Value += 1;
        
        Debug.Log("Player ready");
    }

    public void StartGame()
    {
        mainTimerIsRunning = false;
        secondaryTimerIsRunning = false;
        
        Debug.Log("Starting game");
        
        string gameMode = LobbyManager.Instance.finalGameMode;
        
        Loader.LoadNetwork(gameMode);
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
            
            if (secondaryTimerIsRunning)
            {
                secondaryTimer.Value -= Time.deltaTime;
    
                if (secondaryTimer.Value <= 0f)
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
        
        Debug.Log("Player disconnected");

        if (maxPlayerReadyCount.Value == 1)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallBack;

            Loader.Load("LobbyScene");
        }    
    }
}
