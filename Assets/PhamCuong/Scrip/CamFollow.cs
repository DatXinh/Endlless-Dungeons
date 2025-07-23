using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target.position;
            newPos.z = transform.position.z; // giữ nguyên chiều sâu camera
            transform.position = newPos;
        }
    }
}
