using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialLoadingManager : MonoBehaviour
{
    public static InitialLoadingManager Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        LoadLobbyScene();
    }
    
    private void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
