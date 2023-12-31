using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;

public class SpawnManager : NetworkBehaviour
{
    public GameObject animatronic1;
    public GameObject animatronic2;
    public GameObject animatronic3;
    public GameObject animatronic4;
    public GameObject animatronic5;
    public GameObject guardGameObject;

    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    public Transform spawnPoint5;
    public Transform guardSpawnPoint;
    
    public TMP_Text playerSelectionText;
    public TMP_Text abilityText;
    public Animator playerSelectionAnimation;
    
    private List<GameObject> shuffledAnimatronics = new List<GameObject>();
    
    private bool isGuard = false;
    private int assignedAnimatronicIndex = -1;
    
    private int connectedPlayerCount = 0;
    private int maxPlayerCount = 0;
    
    private bool hasSentReady = false;
    private int playersFullReadyCount = 0;
    
    private bool startedGame = false;
    private bool determinedFinalAssignment = false;
    
    void Start()
    {
        PlayerConnectedServerRpc();
        
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallBack;
        
        maxPlayerCount = ReadyManager.Instance.playerReadyCount.Value;
        ReadyManager.Instance.DestroyObject();
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ShuffleAnimatronics(); // Shuffle animatronics before assigning
            AssignAnimatronics();
            AssignGuard();
            
            Debug.Log("Game Starting");
        }
    }
    
    private void OnClientDisconnectedCallBack(ulong clientId)
    {
        if(IsServer)
            connectedPlayerCount--;
        
        Debug.Log("Player Disconnected");
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void PlayerConnectedServerRpc()
    {
        Debug.Log("Player Connected");
        
        connectedPlayerCount++;
    }

    void Update()
    {
        if(!IsServer)
            return;
        
        if (connectedPlayerCount == maxPlayerCount && !startedGame)
        {
            StartGame();
            startedGame = true;
        }
        
        if (playersFullReadyCount == maxPlayerCount && !determinedFinalAssignment)
        {
            Debug.Log("Determining Final Assignment");
            DetermineFinalAssignmentClientRpc();
            determinedFinalAssignment = true;
        }
    }

    private void AssignAnimatronics()
    {
        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < connectedClients.Count && i < shuffledAnimatronics.Count; i++)
        {
            int animatronicIndex = GetAnimatronicIndex(shuffledAnimatronics[i]);
            RpcAssignAnimatronicClientRpc(animatronicIndex, GetSpawnPointPosition(animatronicIndex),
                Quaternion.identity, connectedClients[i].ClientId);
        }
    }

    private void AssignGuard()
    {
        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

        if (shuffledAnimatronics.Count > 0)
        {
            int randomClientIndex = Random.Range(0, connectedClients.Count);
            ulong targetClientId = connectedClients[randomClientIndex].ClientId;

            RpcAssignGuardClientRpc(guardSpawnPoint.position, guardSpawnPoint.rotation, targetClientId);
        }
        else
        {
            Debug.LogError("No animatronics available for guard assignment.");
        }
    }

    private void ShuffleAnimatronics()
    {
        shuffledAnimatronics.Clear();
        shuffledAnimatronics.Add(animatronic1);
        shuffledAnimatronics.Add(animatronic2);
        shuffledAnimatronics.Add(animatronic3);
        shuffledAnimatronics.Add(animatronic4);
        shuffledAnimatronics.Add(animatronic5);

        shuffledAnimatronics = shuffledAnimatronics.OrderBy(x => Random.value).ToList();
    }

    private int GetAnimatronicIndex(GameObject animatronic)
    {
        for (int i = 0; i < 5; i++)
        {
            if (animatronic == GetAnimatronicGameObject(i))
            {
                return i;
            }
        }
        return -1; // Not found
    }

    private Vector3 GetSpawnPointPosition(int animatronicIndex)
    {
        switch (animatronicIndex)
        {
            case 0:
                return spawnPoint1.position;
            case 1:
                return spawnPoint2.position;
            case 2:
                return spawnPoint3.position;
            case 3:
                return spawnPoint4.position;
            case 4:
                return spawnPoint5.position;
            default:
                return Vector3.zero;
        }
    }

    [ClientRpc]
    private void RpcAssignAnimatronicClientRpc(int animatronicIndex, Vector3 spawnPointPosition, Quaternion spawnPointRotation, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log("Assigned Animatronic: " + animatronicIndex);
            assignedAnimatronicIndex = animatronicIndex;

            if (!hasSentReady)
                hasSentReady = true;
                RecieveReadyServerRpc();
                Debug.Log("Sent Ready");
        }
    }

    [ClientRpc]
    private void RpcAssignGuardClientRpc(Vector3 spawnPointPosition, Quaternion spawnPointRotation, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log("Spawn guard");
            isGuard = true;
        }
    }

    private GameObject GetAnimatronicGameObject(int index)
    {
        switch (index)
        {
            case 0:
                return animatronic1;
            case 1:
                return animatronic2;
            case 2:
                return animatronic3;
            case 3:
                return animatronic4;
            case 4:
                return animatronic5;
            default:
                return null;
        }
    }

    private String GetAnimatronicAbilitiesGameObject(int index)
    {
        switch (index)
        {
            case 0:
                return "This animatronic can do...";
            case 1:
                return "This animatronic can do...";
            case 2:
                return "This animatronic can do...";
            case 3:
                return "This animatronic can do...";
            case 4:
                return "This animatronic can do...";
            default:
                return null;
        }
    }

    [ServerRpc]
    public void RecieveReadyServerRpc()
    {
        playersFullReadyCount++;
    }
    
    [ClientRpc]
    public void DetermineFinalAssignmentClientRpc()
    {
        if(isGuard)
        {
            Debug.Log("You are the guard");
            guardGameObject.SetActive(true);
            playerSelectionText.text = "Guard";
            abilityText.text = "The guard is able to place traps, close doors, and view monitors to be able to play strategically to survive the night.";
        }
        else
        {
            Debug.Log("You are animatronic: " + assignedAnimatronicIndex);
            GetAnimatronicGameObject(assignedAnimatronicIndex).SetActive(true);
            playerSelectionText.text = "Animatronic " + (assignedAnimatronicIndex + 1);
            abilityText.text = GetAnimatronicAbilitiesGameObject(assignedAnimatronicIndex);
        }

        playerSelectionAnimation.SetTrigger("ShowSelection");
    }
}