using UnityEngine;

public class EnemyDame : MonoBehaviour
{
    public int damage; // Số sát thương gây ra cho Player

    // Gây sát thương khi va chạm với Player (dùng Collider 2D)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHP playerHP = collision.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(damage);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHP playerHP = collision.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(damage);
        }
    }
}
