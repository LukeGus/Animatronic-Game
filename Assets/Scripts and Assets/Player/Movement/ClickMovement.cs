using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ClickMovement : MonoBehaviour
{
    public AIPath path;
    public Camera topDownCamera;
    public Camera playerCamera;
    public Transform target;

    private bool canShowPlayerCamera = true;
    private float playerCameraCooldown = 1f; // Set the cooldown duration in seconds

    void Update()
    {
        if (!path.reachedEndOfPath)
        {
            ShowPlayerCameraWithCooldown();
        }
        else
        {
            ShowTopDownCamera();
        }

        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from the camera to the mouse position
            Ray ray = topDownCamera.ScreenPointToRay(Input.mousePosition);
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

    public void ShowPlayerCameraWithCooldown()
    {
        if (canShowPlayerCamera)
        {
            ShowPlayerCamera();
            StartCoroutine(PlayerCameraCooldown());
        }
    }

    IEnumerator PlayerCameraCooldown()
    {
        canShowPlayerCamera = false;
        yield return new WaitForSeconds(playerCameraCooldown);
        canShowPlayerCamera = true;
    }

    public void ShowPlayerCamera()
    {
        playerCamera.gameObject.SetActive(true);
        topDownCamera.gameObject.SetActive(false);
    }

    public void ShowTopDownCamera()
    {
        playerCamera.gameObject.SetActive(false);
        topDownCamera.gameObject.SetActive(true);
    }
}