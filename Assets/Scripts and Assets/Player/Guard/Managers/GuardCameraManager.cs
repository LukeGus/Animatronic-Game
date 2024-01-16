using System.Collections;
using UnityEngine;
using Cinemachine;

public class GuardCameraManager : MonoBehaviour
{
    public Transform monitorPositionTransform;
    public Transform defaultPositionTransform;

    public float transitionDuration;
    public float rotationSpeed; // Adjust the rotation speed as needed

    public GameObject camera;
    private Transform currentTarget;
    private bool isTransitioning = false;

    void Start()
    {
        // Default Position
        GameObject defaultPositionObject = GameObject.Find("Default Position");

        if (defaultPositionObject != null)
        {
            defaultPositionTransform = defaultPositionObject.transform;
        }
        else
        {
            Debug.LogError("GameObject with name 'Default Position' not found in the scene.");
        }

        // Monitor Position
        GameObject monitorPositionObject = GameObject.Find("Monitor Position");

        if (monitorPositionObject != null)
        {
            monitorPositionTransform = monitorPositionObject.transform;
        }
        else
        {
            Debug.LogError("GameObject with name 'Monitor Position' not found in the scene.");
        }

        if (camera == null)
        {
            Debug.LogError("Camera not found!");
            return;
        }

        // Set the initial position and rotation
        SetCameraPosition(defaultPositionTransform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTransitioning)
        {
            // Toggle between positions on each F key press
            if (currentTarget == defaultPositionTransform)
                SetCameraPosition(monitorPositionTransform);
            else
                SetCameraPosition(defaultPositionTransform);
        }
    }

    void SetCameraPosition(Transform newTransform)
    {
        StartCoroutine(TransitionCamera(newTransform));
    }

    IEnumerator TransitionCamera(Transform newTransform)
    {
        isTransitioning = true;

        float elapsedTime = 0f;
        Vector3 startPosition = camera.transform.position;
        Quaternion startRotation = camera.transform.rotation;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;

            // Smoothly interpolate position
            camera.transform.position = Vector3.Lerp(startPosition, newTransform.position, Mathf.SmoothStep(0f, 1f, t));

            // Smoothly interpolate rotation
            camera.transform.rotation = Quaternion.Lerp(startRotation, newTransform.rotation, Mathf.SmoothStep(0f, 1f, t));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the final position and rotation are set correctly
        camera.transform.position = newTransform.position;
        camera.transform.rotation = newTransform.rotation;

        currentTarget = newTransform;
        isTransitioning = false;
    }
}