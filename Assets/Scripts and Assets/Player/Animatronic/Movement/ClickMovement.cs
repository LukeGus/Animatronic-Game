using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ClickMovement : MonoBehaviour
{
    public Camera camera;
    public Transform target;

    private bool canShowPlayerCamera = true;
    
    private void Start()
    {
        StartCoroutine(FullScan());
    }
    
    private IEnumerator FullScan()
    {
        yield return new WaitForSeconds(3);
        AstarPath.active.Scan();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from the camera to the mouse position
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
    
            RaycastHit[] hits = Physics.RaycastAll(ray);
    
            // Iterate through all hits
            foreach (RaycastHit hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.point);
    
                if (EnergyManager.Instance.currentEnergy >= distance)
                {
                    target.position = hit.point;
                    EnergyManager.Instance.currentEnergy -= distance;
                    
                    // Break out of the loop after handling the first hit if needed
                    break;
                }
            }
        }
    }
}