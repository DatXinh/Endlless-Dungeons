using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 3f); // Tự hủy sau 3 giây
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

