using UnityEngine;

public class SlimeBall : MonoBehaviour
{   
    private Vector3 targetDirection;
    private float speed;
    private bool launched = false;

    public void Launch(Vector3 targetPosition, float moveSpeed)
    {
        targetDirection = (targetPosition - transform.position).normalized;
        speed = moveSpeed;
        launched = true;
    }

    void Update()
    {
        if (launched)
        {
            transform.position += targetDirection * speed * Time.deltaTime;
        }
    }
}