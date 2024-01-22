using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ClickMovement : MonoBehaviour
{
    public Camera camera;
    public Transform target;

    private bool canShowPlayerCamera = true;
    private float playerCameraCooldown = 1f; // Set the cooldown duration in seconds

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from the camera to the mouse position
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object clicked is on the same layer as the player
                if (hit.collider.gameObject.layer == gameObject.layer)
                {
                    // Calculate the distance between the player and the clicked point
                    float distance = Vector3.Distance(transform.position, hit.point);

                    if (EnergyManager.Instance.currentEnergy >= distance)
                    {
                        target.position = hit.point;
                        EnergyManager.Instance.currentEnergy -= distance;
                    }
                }
            }
        }
    }
}