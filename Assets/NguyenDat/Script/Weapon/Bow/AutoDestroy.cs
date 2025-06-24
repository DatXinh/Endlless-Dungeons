using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Tự hủy khi chạm vào Wall, Enemy hoặc Boss
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }

    // Nếu dùng va chạm vật lý thay vì trigger, có thể dùng hàm này:
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Boss"))
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}
