using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    private static Scene targetScene;

    public static void Load(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadNetwork(string mapName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(mapName, LoadSceneMode.Single);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}