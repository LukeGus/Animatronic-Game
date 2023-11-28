using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float transitionDuration = 2.0f;

    private void MoveCameraToTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            StartCoroutine(TransitionCamera(newTarget));
        }
    }

    private IEnumerator TransitionCamera(Transform newTarget)
    {
        Transform cameraTransform = virtualCamera.transform;
        Vector3 initialPosition = cameraTransform.position;
        Quaternion initialRotation = cameraTransform.rotation;
        Vector3 finalPosition = newTarget.position;
        Quaternion finalRotation = newTarget.rotation;
    
        float elapsedTime = 0;
        AnimationCurve positionCurve = AnimationCurve.EaseInOut(0, 0, transitionDuration, 1); // Example of using EaseInOut curve
        AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, transitionDuration, 1); // Example of using EaseInOut curve
    
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            cameraTransform.position = Vector3.Lerp(initialPosition, finalPosition, positionCurve.Evaluate(t));
            cameraTransform.rotation = Quaternion.Slerp(initialRotation, finalRotation, rotationCurve.Evaluate(t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        cameraTransform.position = finalPosition;
        cameraTransform.rotation = finalRotation;
    }
}