using UnityEngine;

public class CameraFL : MonoBehaviour
{
    public Transform target; // nhân vật
    public Vector3 offset;   // khoảng cách camera với nhân vật
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
