using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public static StartManager Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }
    
    public void StartGame(string sceneName)
    {
        Loader.LoadNetwork(sceneName);
    }
}
