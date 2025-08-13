using UnityEngine;

public class Coin  : MonoBehaviour
{
    public int coinValue = 1; // Gán số tiền trong mỗi Prefab

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Coin_manager.Instance.AddCoin(coinValue);
            Destroy(gameObject); // Coin biến mất sau khi nhặt
        }
    }
}
