using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Edgar.Unity;

public class DungeonGenerationManager : NetworkBehaviour
{
    private DungeonGeneratorGrid2D dungeonGenerator;
    
    private GameObject  tilemapWalls;
    private GameObject tilemapCollideable;
    
    void Start()
    {
        dungeonGenerator = GetComponent<DungeonGeneratorGrid2D>();
        
        StartGenerationProcess();
    }

    public void StartGenerationProcess()
    {
        if (IsServer)
        {
            GenerateDungeonServer();
        }
    }
    
    public void GenerateDungeonServer()
    {
        int seed = Random.Range(0, 10000);

        GenerateDungeonForClientsClientRpc(seed);
        
        dungeonGenerator.UseRandomSeed = false;
        dungeonGenerator.RandomGeneratorSeed = seed;
        dungeonGenerator.Generate();
        
        AssignTileMapCollidables();
    }
    
    [ClientRpc]
    public void GenerateDungeonForClientsClientRpc(int seed)
    {
        if (IsHost) return;
        
        dungeonGenerator.UseRandomSeed = false;
        dungeonGenerator.RandomGeneratorSeed = seed;
        dungeonGenerator.Generate();
        
        AssignTileMapCollidables();
    }
    
    public void AssignTileMapCollidables()
    {
        Transform tilemaps = transform.Find("Tilemaps");

        if (tilemaps != null)
        {
            tilemapWalls = tilemaps.Find("Walls")?.gameObject;

            if (tilemapWalls != null)
            {
                tilemapCollideable = tilemaps.Find("Collideable")?.gameObject;

                if (tilemapCollideable != null)
                {
                    tilemapWalls.layer = LayerMask.NameToLayer("Obstacles");
                    tilemapCollideable.layer = LayerMask.NameToLayer("Obstacles");
                }
                else
                {
                    Debug.LogError("Could not find Collideable GameObject within Tilemaps.");
                }
            }
            else
            {
                Debug.LogError("Could not find Walls GameObject within Tilemaps.");
            }
        }
        else
        {
            Debug.LogError("Could not find Tilemaps GameObject.");
        }
    }
}
