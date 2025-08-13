using UnityEngine;

public class HPRecover : MonoBehaviour
{
    public int healAmount = 20;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHP playerHP = collision.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
