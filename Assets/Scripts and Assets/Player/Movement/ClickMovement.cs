using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMovement : MonoBehaviour
{
    public Camera topDownCamera;public GameOb\
    
    void Update()
    {
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

                    // Print the distance to the console (you can use it as needed)
                    Debug.Log("Distance to Clicked Point: " + distance);

                    // Here, you can perform any other actions based on the clicked position
                    // For example, move the player to the clicked point:
                    // transform.position = hit.point;
                }
            }
       