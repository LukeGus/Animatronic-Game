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
        yield return new WaitForSeconds(11);
        AstarPath.active.Scan();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            { 
                float distance = Vector3.Distance(target.position, hit.point);
                
                if (EnergyManager.Instance.currentEnergy >= distance)
                {
                    target.position = hit.point;
                    EnergyManager.Instance.currentEnergy -= distance;
                }
            }
        }
    }
}