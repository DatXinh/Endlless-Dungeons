using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Header("Projectile Identity")]
    public bool IsPlayer = false;
    public bool IsEnemy = false;
    // Tự hủy khi chạm vào Wall, Enemy hoặc Boss
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (IsPlayer && (collision.CompareTag("Enemy") || collision.CompareTag("Boss")))
        {
            Destroy(gameObject);
        }
        else if (IsEnemy && collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
