using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManagerSetup : MonoBehaviour
{
    public GameObject startManagerPrefab;
    public GameObject trapManagerPrefab;
    public GameObject timerManagerPrefab;
    
    void Start()
    {
        startManagerPrefab = Instantiate(startManagerPrefab);
        startManagerPrefab.GetComponent<NetworkObject>().Spawn();
        
        trapManagerPrefab = Instantiate(trapManagerPrefab);
        trapManagerPrefab.GetComponent<NetworkObject>().Spawn();
        
        timerManagerPrefab = Instantiate(timerManagerPrefab);
        timerManagerPrefab.GetComponent<NetworkObject>().Spawn();
    }
}
