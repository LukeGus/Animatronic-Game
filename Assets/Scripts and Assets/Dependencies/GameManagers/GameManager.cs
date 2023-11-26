using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private Transform spawnPoint3;
    [SerializeField] private Transform spawnPoint4;
    
    [HideInInspector] public NetworkVariable<int> playerCount = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private void Awake()
    {     
        SubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
         NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if(IsServer)
        {
            Debug.Log("Server Disconnected:" + clientId);
            playerCount.Value -= 1;
            
            if (playerCount.Value <= 0)
            {
                Application.Quit();
            }
            
        }
        else
        {
            Loader.Load("LobbyScene");
        }
    }
    
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer)
        {
            return;
        }
        
        Transform[] spawnPoints = new Transform[] { spawnPoint1, spawnPoint2, spawnPoint3, spawnPoint4 };
    
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
        {
            ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[i];
    
            if (i < spawnPoints.Length)
            {
                Transform spawnPoint = spawnPoints[i];
                GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    
                playerCount.Value += 1;
    
                Debug.Log("Spawned Player at spawn point " + (i + 1));
            }
            else
            {
                Debug.LogWarning("Not enough spawn points for player " + (i + 1));
            }
        }
    }
} 
