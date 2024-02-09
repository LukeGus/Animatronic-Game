using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Michsky.LSS;

public class RoleSelectManager : NetworkBehaviour
{
    public static RoleSelectManager Instance { get; private set; }
    
    [Header("Animatronic Game Objects")]
    public GameObject animatronic1;
    public GameObject animatronic2;
    public GameObject animatronic3;
    public GameObject animatronic4;
    public GameObject animatronic5;
    public GameObject guardGameObject;

    [Header("Spawn Points")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    public Transform spawnPoint5;
    public Transform guardSpawnPoint;
    
    [Header("UI")]
    public TMP_Text selectionText;
    public TMP_Text abilityText;
    public Animator playerSelectionAnimation;
    
    [Header("Managers")]
    [SerializeField] private LSS_Manager lsmManager;
    
    public static event EventHandler OnGameStartedEvent;
    
    private List<GameObject> shuffledAnimatronics = new List<GameObject>();
    
    private bool isGuard = false;
    private int assignedAnimatronicIndex = -1;
    
    private bool determinedFinalAssignment = false;
    private bool hasSentReady = false;
    private int playersFullReadyCount = 0;
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void StartSelectionProcess()
    {
        ShowSelectionClientRpc();
        
        StartGame();
    }
    
    [ClientRpc]
    public void ShowSelectionClientRpc()
    {
        playerSelectionAnimation.SetTrigger("ShowSelection");
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ShuffleAnimatronics();
            AssignAnimatronics();
            AssignGuard();
            
            OnGameStartedEvent?.Invoke(this, EventArgs.Empty);
            
            Debug.Log("Game Starting");
        }
    }

    void Update()
    {
        if(!IsServer)
            return;
        
        if (playersFullReadyCount == ReadyManager.Instance.maxPlayerReadyCount.Value && !determinedFinalAssignment && playersFullReadyCount > 0)
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

    [ServerRpc(RequireOwnership = false)]
    public void RecieveReadyServerRpc()
    {
        playersFullReadyCount++;
    }
    
    [ClientRpc]
    public void DetermineFinalAssignmentClientRpc()
    {
        Debug.Log("Determining Final Assignment");
        
        if(isGuard)
        {
            Debug.Log("You are the guard");
            guardGameObject.SetActive(true);
            selectionText.text = "Guard";
            abilityText.text = "The guard is able to place traps, close doors, and view monitors to be able to play strategically to survive the night.";

            playerSelectionAnimation.SetTrigger("ContinueSelection");
        }
        else
        {
            Debug.Log("You are animatronic: " + assignedAnimatronicIndex);
            GetAnimatronicGameObject(assignedAnimatronicIndex).SetActive(true);
            selectionText.text = "Animatronic " + (assignedAnimatronicIndex + 1);
            abilityText.text = GetAnimatronicAbilitiesGameObject(assignedAnimatronicIndex);

            playerSelectionAnimation.SetTrigger("ContinueSelection");
        }
        
        StartCoroutine(LoadSceneAfterTime());
        
        StartCoroutine(SpawnWhenLoaded());
    }
    
    public IEnumerator LoadSceneAfterTime()
    {
        yield return new WaitForSeconds(15);
        
        if(NetworkManager.Singleton.IsServer)
        {
            string gameMode = LobbyManager.Instance.finalGameMode;
                    
            LoadGameClientRpc(gameMode);
        }
    }
    
    [ClientRpc]
    public void LoadGameClientRpc(string gameMode)
    {
        lsmManager.LoadScene(gameMode);
    }
    
    public IEnumerator SpawnWhenLoaded()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Regular");
        
        Debug.Log("Spawning");
        
        spawnPoint1 = SpawnpointManager.Instance.spawnPoint1;
        spawnPoint2 = SpawnpointManager.Instance.spawnPoint2;
        spawnPoint3 = SpawnpointManager.Instance.spawnPoint3;
        spawnPoint4 = SpawnpointManager.Instance.spawnPoint4;
        spawnPoint5 = SpawnpointManager.Instance.spawnPoint5;
        guardSpawnPoint = SpawnpointManager.Instance.guardSpawnPoint;
        
        if (isGuard)
        {
            InstantiateGuardGameObjectServerRpc();
        }
        else
        {
            switch (assignedAnimatronicIndex)
            {
                case 0:
                    InstantiateAnimatronic1GameObjectServerRpc();
                    break;
                case 1:
                    InstantiateAnimatronic2GameObjectServerRpc();
                    break;
                case 2:
                    InstantiateAnimatronic3GameObjectServerRpc();
                    break;
                case 3:
                    InstantiateAnimatronic4GameObjectServerRpc();
                    break;
                case 4:
                    InstantiateAnimatronic5GameObjectServerRpc();
                    break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InstantiateAnimatronic1GameObjectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject animatronic = Instantiate(animatronic1, spawnPoint1, spawnPoint1);
        animatronic.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void InstantiateAnimatronic2GameObjectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject animatronic = Instantiate(animatronic2, spawnPoint2, spawnPoint2);
        animatronic.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void InstantiateAnimatronic3GameObjectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject animatronic = Instantiate(animatronic3, spawnPoint3, spawnPoint3);
        animatronic.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InstantiateAnimatronic4GameObjectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject animatronic = Instantiate(animatronic4, spawnPoint4, spawnPoint4);
        animatronic.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InstantiateAnimatronic5GameObjectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject animatronic = Instantiate(animatronic5, spawnPoint5, spawnPoint5);
        animatronic.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InstantiateGuardGameObjectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject guard = Instantiate(guardGameObject, guardSpawnPoint, guardSpawnPoint);
        guard.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
}
