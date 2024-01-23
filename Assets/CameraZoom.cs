using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float sensitivity = 1.0f;
    public float minDistance = 1.0f;
    public float maxDistance = 10.0f;

    private void Update()
    {
        // Get the scroll wheel input
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // Adjust the CameraDistance based on the scroll wheel input
        AdjustCameraDistance(scrollWheel);
    }

    private void AdjustCameraDistance(float scrollInput)
    {
        // Get the current CameraDistance
        float currentDistance = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;

        // Calculate the new CameraDistance based on the scroll input and sensitivity
        float newDistance = currentDistance - scrollInput * sensitivity;

        // Clamp the new CameraDistance to the specified range
        newDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);

        // Set the new CameraDistance
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = newDistance;
    }
}