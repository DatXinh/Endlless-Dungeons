using UnityEngine;

public class EnemyDame : MonoBehaviour
{
    public int damage; // Số sát thương gây ra cho Player
    private int FinalDamage; // Số sát thương cuối cùng sẽ được áp dụng

    private void Start()
    {
        if (LoopManager.Instance != null)
        {
            if (LoopManager.Instance.currentLoop > 0)
            {
                FinalDamage = (int)(damage * (LoopManager.Instance.currentLoop * 0.2f)); // Tính toán sát thương cuối cùng dựa trên vòng lặp hiện tại
            }
            else
            {
                FinalDamage = damage; // Sử dụng sát thương gốc nếu không có LoopManager
            }
        }
    }
    // Gây sát thương khi va chạm với Player (dùng Collider 2D)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHP playerHP = collision.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(FinalDamage);
        }
    }
}
