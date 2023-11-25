using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPlayer : MonoBehaviour
{

    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}