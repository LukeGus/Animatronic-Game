using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{

    public float Speed = 25f;
    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;

    public Vector2 MinBoundary;
    public Vector2 MaxBoundary;
    public bool isDisableOffset;

    void Start()
    {
        targetPos = transform.position;    
    }

    void FixedUpdate()
    {
        if (target)
        {
            offset.x = 0;
            offset.y = 0;

            if (!isDisableOffset)
            {
                Vector3 ddd = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Vector3 sdf = Input.mousePosition - ddd;
                float xOfs = sdf.x / ddd.x;
                float yOfs = sdf.y / ddd.y;
                offset.x = xOfs * 3;
                offset.y = yOfs * 3;
                offset.x = Mathf.Clamp(offset.x, -3, 3);
                offset.y = Mathf.Clamp(offset.y, -3, 3);
            }

            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * Speed;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
            
            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
        }
    }
}
