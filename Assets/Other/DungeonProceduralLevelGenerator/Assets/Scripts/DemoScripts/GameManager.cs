using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject _player;
    private GameObject spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindSpawner());
    }

    public IEnumerator FindSpawner() {

        yield return new WaitForSeconds(0.1f);

        spawnPoint = GameObject.FindGameObjectWithTag("spawnPoint");
        Debug.Log(spawnPoint);
        if (spawnPoint)
            Instantiate(_player, spawnPoint.transform.position, Quaternion.identity);
        else
            StartCoroutine(FindSpawner());
    }

}
