using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
    public static SpawnpointManager Instance { get; private set; }
    
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    public Transform spawnPoint5;
    
    public Transform guardSpawnPoint;
    
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
    }
}
